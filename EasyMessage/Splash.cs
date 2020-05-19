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
using EasyMessage.Controllers;
using EasyMessage.Entities;

namespace EasyMessage
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
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
                AccountsController.instance.CreateTable();
                AccountsController.instance.GetItems();
                if (AccountsController.instance.deviceAccsP.Count < 1)
                {
                    intent = new Intent(this, typeof(Registration));
                }
                else
                {
                    foreach (Account a in AccountsController.instance.deviceAccsP)
                    {
                        if (a.isMainP)
                        {
                            AccountsController.mainAccP = a;
                            
                        }
                    }
                    if (AccountsController.mainAccP == null)
                    {
                        intent = new Intent(this, typeof(SignUp));
                    }
                    else
                    {
                        intent = new Intent(this, typeof(MainActivity));
                    }
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