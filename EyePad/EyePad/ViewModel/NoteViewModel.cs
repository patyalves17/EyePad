using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using EyePad.Model;
using System.Diagnostics;

namespace EyePad.ViewModel
{
    public class NoteViewModel: INotifyPropertyChanged
    {
        
        public Note NoteModel { get; set; }
        public string Note { get { return App.NoteVM.Note; } }
        public List<Note> ListaNotes;

        public ObservableCollection<Note> Notes { get; set; } = new ObservableCollection<Note>();

        public event PropertyChangedEventHandler PropertyChanged;

        // UI Events
        public OnEditarNoteCMD OnEditarNoteCMD { get; }
        public OnDeleteNoteCMD OnDeleteNoteCMD { get; }
        public OnSaveNoteCMD OnSaveNoteCMD { get; }

        public ICommand OnAddNoteCMD { get; private set; }
        public ICommand OnSairCMD { get; private set; }
       

        private Note selecionado;
        public Note Selecionado
        {
            get { return selecionado; }
            set
            {
                selecionado = value as Note;
                EventPropertyChanged();
            }
        }

        private void EventPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
       




        public NoteViewModel()
        {
            NoteRepository repository = NoteRepository.Instance;

            OnEditarNoteCMD = new OnEditarNoteCMD(this);
            OnDeleteNoteCMD = new OnDeleteNoteCMD(this);
            OnSaveNoteCMD = new OnSaveNoteCMD(this);

            OnAddNoteCMD = new Command(OnAddNote);
            OnSairCMD = new Command(OnSair);


            ListaNotes = new List<Note>();            
            Carregar();
        }
        public void Carregar()
        {
            ListaNotes = NoteRepository.GetNotes().ToList();
            Notes.Clear();

            for (int index = 0; index < ListaNotes.Count; index++)
            {                
                var item = ListaNotes[index];

                if (index + 1 > Notes.Count)                                  
                    Notes.Insert(index, item);                 
                    
            }
                     
           
            //AplicarFiltro();
        }
        public List<Note> CarregarNotes()
        {
            ListaNotes = NoteRepository.GetNotes().ToList();
            return ListaNotes;
            //AplicarFiltro();
        }

        private async void OnSair()
        {
            await App.Current.MainPage.Navigation.PopAsync();
        }

      

        private void OnAddNote()
        {           
            App.NoteVM.Selecionado = new Model.Note();

            App.Current.MainPage.Navigation.PushAsync(
                new View.Note.NovoNoteView() { BindingContext = App.NoteVM });
            
        }
        public async void EditNote()
        {
            await App.Current.MainPage.Navigation.PushAsync(
                new View.Note.NovoNoteView() { BindingContext = App.NoteVM });
        }

        public void SaveNote(Note paramNote)
        {
            Debug.WriteLine("--SaveNote---------------------------------");
            Debug.WriteLine(paramNote.Title);
            Debug.WriteLine(paramNote.Date);
            Debug.WriteLine(paramNote.Text);
            Debug.WriteLine(paramNote.ToString());
            if ((paramNote == null) || (string.IsNullOrWhiteSpace(paramNote.Title)))
                App.Current.MainPage.DisplayAlert("Alert", "The field Title is required", "OK");
            else if (NoteRepository.SaveNote(paramNote) > 0)
                App.Current.MainPage.Navigation.PopAsync();
            else
                App.Current.MainPage.DisplayAlert("Error", "Sorry, something is wrong =(", "OK");
        }


        public async void RemoveNote()
        {
            if (await App.Current.MainPage.DisplayAlert("Alert?",
                                                        string.Format("Are you sure you want to remove {0}?", Selecionado.Title), "Yes", "No"))
            {
                if (NoteRepository.RemoverNote(Selecionado.Id) > 0)
                {
                    ListaNotes.Remove(Selecionado);
                    Carregar();
                }
                else
                    await App.Current.MainPage.DisplayAlert(
                        "Falhou", "Sorry, something is wrong =(", "OK");
            }
        }
       
    }



    public class OnSaveNoteCMD : ICommand
    {
        private NoteViewModel noteVM;
        public OnSaveNoteCMD(NoteViewModel paramVM)
        {
            noteVM = paramVM;
        }
        public event EventHandler CanExecuteChanged;
        public void AdicionarCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter)
        {
            Note noteParam = (Note)parameter;

            DateTime dt = DateTime.Now.ToLocalTime();
            string dt_string =   dt.ToString("dd/MM/yyyy");
            noteParam.Date = dt_string+" ";

            noteVM.SaveNote(noteParam);

            //noteVM.Adicionar(parameter as Note);
        }
    }


    public class OnEditarNoteCMD : ICommand
    {
        private NoteViewModel noteVM;
        public OnEditarNoteCMD(NoteViewModel paramVM)
        {
            noteVM = paramVM;
        }
        public event EventHandler CanExecuteChanged;
        public void EditarCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        public bool CanExecute(object parameter) => (parameter != null);
        public void Execute(object parameter)
        {
            App.NoteVM.Selecionado = parameter as Note;
            noteVM.EditNote();
        }
    }

    public class OnDeleteNoteCMD : ICommand
    {
        private NoteViewModel noteVM;
        public OnDeleteNoteCMD(NoteViewModel paramVM)
        {
            noteVM = paramVM;
        }
        public event EventHandler CanExecuteChanged;
        public void DeleteCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        public bool CanExecute(object parameter) => (parameter != null);
        public void Execute(object parameter)
        {
            App.NoteVM.Selecionado = parameter as Note;
            noteVM.RemoveNote();
        }
    }


}
