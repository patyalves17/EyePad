using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace EyePad.Droid
{
    [Activity(Label = "EyePad",
        Icon = "@drawable/icon",
        Theme = "@style/splashscreen",
        MainLauncher = true,
        NoHistory = true)]
    public class SplashScreen : Activity
    {
        protected override void OnResume()
        {
            base.OnResume();
            StartActivity(typeof(MainActivity));
        }
    }
}