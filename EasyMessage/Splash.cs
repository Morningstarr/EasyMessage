using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Java.Lang;

namespace EasyMessage
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light.NoActionBar", MainLauncher = true)]
    public class Splash : AppCompatActivity
    {
        private Animation mEnlargeAnimation;
        private ImageView spImage;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.splash);

            spImage = FindViewById<ImageView>(Resource.Id.splashImage);
            mEnlargeAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.enlarge);

            spImage.Animation = mEnlargeAnimation;

            spImage.Animation.AnimationEnd += delegate
            {
                Intent intent;
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                bool isUser = prefs.GetBoolean("bool_value", false);
                if (isUser)
                {
                    intent = new Intent(this, typeof(SignUp));
                }
                else
                {
                    intent = new Intent(this, typeof(Registration));
                }
                intent.SetFlags(ActivityFlags.NewTask);
                StartActivity(intent);
                Finish();
            };

            spImage.StartAnimation(mEnlargeAnimation);
           
        }

        protected override void OnPause()
        {
            base.OnPause();
            spImage.ClearAnimation();
        }
    }
}