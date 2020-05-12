using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace EasyMessage
{
    [Activity(Label = "SignUp")]
    public class SignUp : AppCompatActivity, IOnCompleteListener
    {
        public static FirebaseApp app;
        private FirebaseAuth auth;
        private EditText eMail;
        private EditText pss;

        public void OnComplete(Android.Gms.Tasks.Task task)
        {
            if (task.IsSuccessful)
            {
                Toast.MakeText(this, "Sign in success", ToastLength.Short).Show();
                //StartActivity(new Android.Content.Intent(this, typeof(Splash)));
                //Finish();
            }
            else
            {
                MessageBox(task.Exception.Message);
                Toast.MakeText(this, "Failed", ToastLength.Short).Show();
                //Finish();
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.sign_up);

            var ok = FindViewById<Button>(Resource.Id.btnOK);
            ok.Click += delegate
            {
                ok_Click();
            };
        }

        public async void ok_Click()
        {
            try
            {
                eMail = FindViewById<EditText>(Resource.Id.edtMail);
                pss = FindViewById<EditText>(Resource.Id.edtPass);

                FirebaseController.instance.initFireBaseAuth();
                string s = await FirebaseController.instance.LoginUser(eMail.Text, pss.Text);
                if (s != string.Empty)
                {
                    Toast.MakeText(this, "Sign in success", ToastLength.Short).Show();
                    //StartActivity(new Android.Content.Intent(this, typeof(Splash)));
                }
            }
            catch(Exception ex)
            {
                MessageBox(ex.Message);
            }
        }

        public void MessageBox(string MyMessage)
        {
            AlertDialog.Builder builder;
            builder = new AlertDialog.Builder(this);
            builder.SetTitle("Warning");
            builder.SetMessage(MyMessage);
            builder.SetCancelable(false);
            builder.SetPositiveButton("OK", delegate { });
            Dialog dialog = builder.Create();
            dialog.Show();
            return;
        }

        private void initFireBaseAuth()
        {
            var options = new FirebaseOptions.Builder()
                .SetApplicationId("1:323956276016:android:fcf09b75b366f4fa50a6f5")
                .SetApiKey("AIzaSyAOvDOj-PKpxFZcDgMO7uI4rxrP3i2GakM")
                .Build();

            if (app == null)
            {
                app = FirebaseApp.InitializeApp(Application.Context, options);

            }
            auth = FirebaseAuth.Instance;
            auth = FirebaseAuth.GetInstance(app);

            LoginUser();

        }

        private void LoginUser()
        {
            eMail = FindViewById<EditText>(Resource.Id.edtMail);
            pss = FindViewById<EditText>(Resource.Id.edtPass);

            auth.SignInWithEmailAndPassword(eMail.Text, pss.Text)
                .AddOnCompleteListener(this);
        }
    }
}