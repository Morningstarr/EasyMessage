using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using EasyMessage.Controllers;
using EasyMessage.Entities;

namespace EasyMessage
{
    [Activity(Label = "Registration")]
    public class Registration : Activity
    {
        private EditText email;
        private EditText login;
        private EditText password;
        private EditText repass;
        private Button conf;
        private ImageButton ava;
        private TextView enter;
        private CheckBox chb1;
        private CheckBox chb2;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.registration);

            enter = FindViewById<TextView>(Resource.Id.enter);
            conf = FindViewById<Button>(Resource.Id.confirm);
            ava = FindViewById<ImageButton>(Resource.Id.avatr);
            chb1 = FindViewById<CheckBox>(Resource.Id.chb1);
            chb2 = FindViewById<CheckBox>(Resource.Id.chb2);
            password = FindViewById<EditText>(Resource.Id.pass);
            repass = FindViewById<EditText>(Resource.Id.repass);

            enter.Click += delegate
            {
                sign_in();
            };
            ava.Click += delegate
            {
                choose_avatar();
            };
            conf.Click += delegate
            {
                confirm_Click();
            };
            chb1.CheckedChange += delegate
            {
                Utils.passHide(chb1, password);
            };
            chb2.CheckedChange += delegate
            {
                Utils.passHide(chb2, repass);
            };
            // Create your application here
        }

        private void sign_in()
        {
            Intent intent = new Intent(this, typeof(SignUp));
            //intent.SetFlags(ActivityFlags.NewTask);
            StartActivity(intent);
            //Finish();
        }

        private void choose_avatar()
        {
            throw new NotImplementedException();
        }

        private async void confirm_Click()
        {
            ProgressBar pb = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            pb.Visibility = ViewStates.Visible;

            email = FindViewById<EditText>(Resource.Id.email);
            login = FindViewById<EditText>(Resource.Id.login);
            password = FindViewById<EditText>(Resource.Id.pass);
            repass = FindViewById<EditText>(Resource.Id.repass);
            
            edit_controls(false);

            try
            {
                if (password.Text.Length > 7)
                {
                    if (password.Text == repass.Text)
                    {
                        Utils.IsCorrectEmail(email.Text);
                        Utils.IsCorrectLogin(login.Text);
                        FirebaseController.instance.initFireBaseAuth();
                        string s = await FirebaseController.instance.Register(email.Text, password.Text, login.Text);
                        if (s != string.Empty)
                        {
                            Toast.MakeText(this, "Register success", ToastLength.Short).Show();

                            //AccountsController.instance.deviceAccsP.Add(new Account { emailP = email.Text, loginP = login.Text, passwordP = password.Text });
                            AccountsController.instance.CreateTable();
                            AccountsController.instance.SaveItem(new Account { emailP = email.Text, loginP = login.Text, passwordP = password.Text });

                            Intent intent = new Intent(this, typeof(SignUp));
                            intent.SetFlags(ActivityFlags.NewTask);
                            StartActivity(intent);
                            Finish();
                        }
                    }
                    else
                    {
                        throw new Exception("Пароли должны совпадать!");
                    }
                }
                else
                {
                    throw new Exception("Длина пароля должна быть больше 8 символов!");
                }
            }
            catch(Exception ex)
            {
                Utils.MessageBox(ex.Message, this);
                pb.Visibility = ViewStates.Invisible;
                edit_controls(true);
            }
            edit_controls(true);
        }

        public void edit_controls(bool c)
        {
            email.Enabled = c;
            login.Enabled = c;
            password.Enabled = c;
            repass.Enabled = c;
            conf.Enabled = c;
            ava.Enabled = c;
            enter.Enabled = c;
        }
    }
}