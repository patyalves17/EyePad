using System;
namespace EyePad
{
    public static class AppConstants
    {
        
        #region computer vision api
        public const string OcpApimSubscriptionKey = "Ocp-Apim-Subscription-Key";
        /// <summary>
        /// Url of the Computer Vision API OCR method for printed text
        /// [language=unk] Text in image is in unknow. 
        /// [detectOrientation=true] Improve results by detecting orientation
        /// </summary>
        public static string ComputerVisionApiOcrUrl = "https://brazilsouth.api.cognitive.microsoft.com/vision/v1.0/ocr?language=unk&detectOrientation=true";

        /// <summary>
        /// Url of the Computer Vision API handwritten text recognition method
        /// [handwriting=true] Text in image is handwritten. Set to false for printed text.
        /// </summary>
        public static string ComputerVisionApiHandwritingUrl = "https://brazilsouth.api.cognitive.microsoft.com/vision/v1.0/recognizeText?handwriting=true";


        /// <summary>
        /// User's API Key for the Computer Vision API. Not a constant because it can get set in the app 
        /// if a user enters a key on the screen that allows key input.
        /// </summary>
        public static string ComputerVisionApiKey = "d7416942d72a48ee88f3599fae165a0d";
        //public static string ComputerVisionApiKey = "cd2d12265e7a4b8487b08e7ebce9919b";//testes
        #endregion
    }
}
