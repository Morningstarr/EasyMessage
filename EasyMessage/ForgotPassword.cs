using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace EasyMessage
{
    [Activity(Label = "Восстановление пароля")]
    public class ForgotPassword : AppCompatActivity, IOnCompleteListener
    {
        private Button reset;
        private EditText eml;
        private TextView label;
        private ProgressBar progress;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.forgot_password);

            reset = FindViewById<Button>(Resource.Id.resetpass);
            eml = FindViewById<EditText>(Resource.Id.resmail);
            label = FindViewById<TextView>(Resource.Id.labels);
            progress = FindViewById<ProgressBar>(Resource.Id.progressforgot);

            reset.Click += delegate
            {
                reset_password();
            };

            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#2196f3")));
        }

        private void reset_password()
        {
            try
            {
                eml.Enabled = false;
                reset.Enabled = false;
                progress.Visibility = ViewStates.Visible;

                FirebaseController.instance.initFireBaseAuth();
                FirebaseController.instance.ResetPassword(eml.Text, this);
            }
            catch(Exception ex)
            {
                Utils.MessageBox(ex.Message, this);
            }
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

        public void OnComplete(Task task)
        {
            if (task.IsSuccessful)
            {
                eml.Enabled = true;
                reset.Enabled = true;
                label.Visibility = ViewStates.Visible;
                progress.Visibility = ViewStates.Invisible;
                eml.Text = "";
            }
            else
            {
                eml.Enabled = true;
                reset.Enabled = true;
                progress.Visibility = ViewStates.Invisible;
                Utils.MessageBox("Непредвиденная ошибка, проверьте соединение с сетью!", this);
            }
        }
    }
}