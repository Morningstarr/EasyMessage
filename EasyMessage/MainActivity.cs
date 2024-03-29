﻿using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Firebase.Auth;
using Android.Gms.Tasks;
using Firebase;
using System.Threading.Tasks;

namespace EasyMessage
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light.DarkActionBar")]
    public class MainActivity : AppCompatActivity, IOnCompleteListener
    {
        private Button stmain;
        public void OnComplete(Android.Gms.Tasks.Task task)
        {
            if (task.IsSuccessful)
            {
                Toast.MakeText(this, "Sign in success", ToastLength.Short).Show();
                StartActivity(new Android.Content.Intent(this, typeof(Splash)));
                //Finish();
            }
            else
            {
                Toast.MakeText(this, "Failed", ToastLength.Short).Show();
                Finish();
            }
        }

        public override void OnBackPressed()
        {
            Finish();          
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            stmain = FindViewById<Button>(Resource.Id.startmain);
            //ActionBar.SetHomeButtonEnabled(true);
            //ActionBar.SetDisplayHomeAsUpEnabled(true);

            var ok = FindViewById<Button>(Resource.Id.btnOK);
            stmain.Click += delegate
            {
                StartActivity(new Android.Content.Intent(this, typeof(MainPage)));
            };
            ok.Click += delegate
            {
                ok_Click();
            };
        }

        public void ok_Click()
        {
            StartActivity(new Android.Content.Intent(this, typeof(Profile)));
        }
    }
}