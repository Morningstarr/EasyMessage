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
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Text.Method;

namespace EasyMessage
{
    [Activity(Label = "Вход", Theme = "@style/Theme.MaterialComponents.Light")]
    public class SignUp : AppCompatActivity, IOnCompleteListener
    {
        private EditText eMail;
        private EditText pss;
        private Button ok;
        private ProgressBar pbar;
        private TextView fpass;
        private TextView reg;
        private CheckBox chBox;

        public void OnComplete(Android.Gms.Tasks.Task task)
        {
            if (task.IsSuccessful)
            {
                Toast.MakeText(this, "Sign in success", ToastLength.Short).Show();
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
            chBox = FindViewById<CheckBox>(Resource.Id.checkBox1);
            pss = FindViewById<EditText>(Resource.Id.edtPass);

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
            chBox.CheckedChange += delegate
            {
                Utils.passHide(chBox, pss);
            };
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#2196f3")));
        }

        

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
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
                    Intent intent = new Intent(this, typeof(MainDetail));
                    AccountsController.instance.CreateTable();
                    AccountsController.instance.GetItems();
                    if (AccountsController.instance.deviceAccsP.Find(x => x.emailP == eMail.Text) == null)
                    {
                        AccountsController.instance.deviceAccsP.Add(new Account { emailP = eMail.Text, passwordP = pss.Text, loginP = FirebaseController.instance.GetCurrentUser().DisplayName });
                    }
                    var p = AccountsController.instance.deviceAccsP.Find(x => x.emailP == eMail.Text);
                    p.isMainP = true;
                    AccountsController.instance.SaveItem(p);
                    AccountsController.mainAccP = p;
                    Finish();
                    intent.SetFlags(ActivityFlags.ClearTask);
                    StartActivity(intent);
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
            fpass.Enabled = c;
        }
    }
}