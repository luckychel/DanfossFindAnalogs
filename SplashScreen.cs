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
using Android.Graphics;

namespace CompetitorTool
{
    [Activity(
        MainLauncher = true
        , Theme = "@style/SplashScreen"
        , Icon = "@drawable/Icon"
        , Label = "Competitor Tool"
        , NoHistory = true
    )]
    public class SplashScreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.RequestedOrientation = Android.Content.PM.ScreenOrientation.Sensor;

            StartActivity(typeof(MainActivity));
        }
    }
}