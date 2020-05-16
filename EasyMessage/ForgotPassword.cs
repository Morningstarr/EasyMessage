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

namespace EasyMessage
{
    [Activity(Label = "ForgotPassword")]
    public class ForgotPassword : Activity
    {
        private Button reset;
        private EditText eml;
        private TextView label;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.forgot_password);

            reset = FindViewById<Button>(Resource.Id.resetpass);
            eml = FindViewById<EditText>(Resource.Id.resmail);
            label = FindViewById<TextView>(2131230822);

            reset.Click += delegate
            {
                eml.Enabled = false;
                reset.Enabled = false;

                FirebaseController.instance.initFireBaseAuth();
                FirebaseController.instance.ResetPassword(eml.Text);

                eml.Enabled = true;
                reset.Enabled = true;
                label.Visibility = ViewStates.Visible;
                eml.Text = "";
            };
        }


    }
}