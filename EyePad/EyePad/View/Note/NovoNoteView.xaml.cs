using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Linq;
using EyePad.ViewModel;

namespace EyePad.View.Note
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NovoNoteView : ContentPage
    {
        //private int noteId = 0;
        #region fields
        HttpClient visionApiClient;
        byte[] photo;
        //ObservableCollection<string> values;
        String resultado;
        #endregion

        public static NoteViewModel NoteVM { get; set; }

        public NovoNoteView()
        {
            InitializeComponent();
            //stack1.HeightRequest = App.Current.MainPage.Width - stack0.HeightRequest;
            stack1.HeightRequest = 400;
        }
        async void OnFoto(object sender, EventArgs e)
        {
            byte[] photoByteArray = null;

            try
            {
                photoByteArray = await TakePhoto();
                //await StartCamera();

            }
            catch (Exception exc)
            {
                Debug.WriteLine("-----------------------------------");
                Debug.WriteLine(exc.Message);
            }

            if (photoByteArray != null)
            {
                Debug.WriteLine("tirou foto");
                Debug.WriteLine(photoByteArray);
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
                Ocr(photoByteArray);
                //bool handwritten = (sender == ButtonTakeHandwritten);
                //await Navigation.PushAsync(new OcrResultsPage(photoByteArray, handwritten));
            }
        }


        async Task<byte[]> TakePhoto()
        {
            Debug.WriteLine("---TakePhoto--------------------------------");
            MediaFile photoMediaFile = null;
            byte[] photoByteArray = null;

            if (CrossMedia.Current.IsCameraAvailable)
            {
                var mediaOptions = new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Small,
                    AllowCropping = true,
                    SaveToAlbum = true,
                    Name = $"{DateTime.UtcNow}.jpg"
                };
                photoMediaFile = await CrossMedia.Current.TakePhotoAsync(mediaOptions);
                photoByteArray = MediaFileToByteArray(photoMediaFile);
            }
            else
            {
                await DisplayAlert("Error", "No camera found", "OK");
                Debug.WriteLine($"ERROR: No camera found");
                //Console.WriteLine($"ERROR: No camera found");
            }
            Debug.WriteLine("--return---------------");
            return photoByteArray;
        }
        /// <summary>
        /// Convert the media file to a byte array.
        /// </summary>
        byte[] MediaFileToByteArray(MediaFile photoMediaFile)
        {
            using (var memStream = new MemoryStream())
            {
                photoMediaFile.GetStream().CopyTo(memStream);
                return memStream.ToArray();
            }
        }



        async private void Ocr(byte[] photo)
        {
            Debug.WriteLine("--begin ocr---------------");
            this.photo = photo;
            visionApiClient = new HttpClient();
            visionApiClient.DefaultRequestHeaders.Add(AppConstants.OcpApimSubscriptionKey, AppConstants.ComputerVisionApiKey);

            String result = await FetchPrintedWordList();
            txtText.Text = result;

            Debug.WriteLine(result);

        }

        async Task<string> FetchPrintedWordList()
        {
            Debug.WriteLine("--FetchPrintedWordList---------------");

            ObservableCollection<string> wordList = new ObservableCollection<string>();
            if (photo != null)
            {
                Debug.WriteLine("--photo != null---------------");
                HttpResponseMessage response = null;
                using (var content = new ByteArrayContent(photo))
                {
                    Debug.WriteLine("--begin enviou--------------");
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response = await visionApiClient.PostAsync(AppConstants.ComputerVisionApiOcrUrl, content);
                    Debug.WriteLine("--end enviou--------------");
                    Debug.WriteLine(response);
                }
                string ResponseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(ResponseString);
                JObject json = JObject.Parse(ResponseString);
                IEnumerable<JToken> lines = json.SelectTokens("$.regions[*].lines[*]");

                Debug.WriteLine(lines);

                if (lines != null)
                {
                    foreach (JToken line in lines)
                    {
                        IEnumerable<JToken> words = line.SelectTokens("$.words[*].text");
                        if (words != null)
                        {
                            Debug.WriteLine(string.Join(" ", words.Select(x => x.ToString())));
                            resultado += string.Join(" ", words.Select(x => x.ToString())) + " ";
                            wordList.Add(string.Join(" ", words.Select(x => x.ToString())));
                        }
                    }
                }
            }
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            return resultado;
        }

    
    }
}
