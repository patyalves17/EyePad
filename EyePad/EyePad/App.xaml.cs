using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using EyePad.Model;
using EyePad.ViewModel;

namespace EyePad
{
    public partial class App : Application
    {
        
        public static NoteViewModel NoteVM { get; set; }


        public App()
        {
            InitializeComponent();
            InitializeApplication();
            //MainPage = new EyePadPage();
            MainPage = new NavigationPage(new View.Note.MainPage() { BindingContext= App.NoteVM} );
        }

        private void InitializeApplication()
        {
            if (NoteVM == null) NoteVM = new NoteViewModel();
        }

       
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
