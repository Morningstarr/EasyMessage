using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using EasyMessage.Controllers;
using EasyMessage.Entities;


namespace EasyMessage
{
    [Activity(Label = "SignUp")]
    public class SignUp : AppCompatActivity, IOnCompleteListener
    {
        private EditText eMail;
        private EditText pss;
        private Button ok;
        private ProgressBar pbar;
        private TextView fpass;
        private TextView reg;

        public void OnComplete(Android.Gms.Tasks.Task task)
        {
            if (task.IsSuccessful)
            {
                Toast.MakeText(this, "Sign in success", ToastLength.Short).Show();
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                bool isUser = prefs.GetBoolean("bool_value", false);
                if (!isUser)
                {
                    ISharedPreferencesEditor editor = prefs.Edit();
                    editor.PutBoolean("bool_value", true);
                }

                AccountsController.instance.deviceAccsP.Find(x => x.emailP == eMail.Text).isMainP = true;
            }
            else
            {
                Utils.MessageBox(task.Exception.Message, this);
                Toast.MakeText(this, "Failed", ToastLength.Short).Show();
                //Finish();
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.sign_up);

            fpass = FindViewById<TextView>(Resource.Id.fpass);
            ok = FindViewById<Button>(Resource.Id.btnOK);
            reg = FindViewById<TextView>(Resource.Id.reg);

            ok.Click += delegate
            {
                ok_Click();
            };
            fpass.Click += delegate
            {
                forgot_pass();
            };
            reg.Click += delegate
            {
                register();
            };
        }

        public async void ok_Click()
        {
            try
            {
                eMail = FindViewById<EditText>(Resource.Id.edtMail);
                pss = FindViewById<EditText>(Resource.Id.edtPass);
                pbar = FindViewById<ProgressBar>(Resource.Id.progressBar2);
                
                pbar.Visibility = ViewStates.Visible;

                edit_contols(false);

                FirebaseController.instance.initFireBaseAuth();
                string s = await FirebaseController.instance.LoginUser(eMail.Text, pss.Text);
                if (s != string.Empty)
                {
                    Toast.MakeText(this, "Sign in success", ToastLength.Short).Show();
                    Intent intent = new Intent(this, typeof(MainActivity));
                    AccountsController.instance.CreateTable();
                    AccountsController.instance.GetItems();
                    if (AccountsController.instance.deviceAccsP.Find(x => x.emailP == eMail.Text) == null)
                    {
                        AccountsController.instance.deviceAccsP.Add(new Account { emailP = eMail.Text, passwordP = pss.Text });
                    }
                    var p = AccountsController.instance.deviceAccsP.Find(x => x.emailP == eMail.Text);
                    p.isMainP = true;
                    AccountsController.instance.SaveItem(p);
                    intent.SetFlags(ActivityFlags.NewTask);
                    StartActivity(intent);
                    Finish();
                }
            }
            catch(Exception ex)
            {
                Utils.MessageBox(ex.Message, this);
                edit_contols(true);
                pbar.Visibility = ViewStates.Invisible;
            }
            edit_contols(true);
            pbar.Visibility = ViewStates.Invisible;
        }

        public void register()
        {
            Intent intent = new Intent(this, typeof(Registration));
            //intent.SetFlags(ActivityFlags.NewTask);
            StartActivity(intent);
        }

        public void forgot_pass()
        {
            Intent intent = new Intent(this, typeof(ForgotPassword));
            //intent.SetFlags(ActivityFlags.NewTask);
            StartActivity(intent);
            //Finish();
        }

        public void edit_contols(bool c)
        {
            ok.Enabled = c;
            eMail.Enabled = c;
            pss.Enabled = c;
            reg.Enabled = c;
        }
    }
}