using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using EasyMessage.Controllers;
using EasyMessage.Entities;
using AlertDialog = Android.App.AlertDialog;
using Android.Gms.Tasks;

namespace EasyMessage
{
    [Activity(Label = "Изменение данных")]
    public class EditProfileData : AppCompatActivity, IOnCompleteListener
    {
        private TextView text;
        private EditText data;
        private EditText oldpass;
        private ProgressBar progress;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.edit_profile_data);

            data = FindViewById<EditText>(Resource.Id.newData);
            text = FindViewById<TextView>(Resource.Id.description);
            oldpass = FindViewById<EditText>(Resource.Id.oldpassword);

            progress = FindViewById<ProgressBar>(Resource.Id.progressBar1);

            string name = "id";
            string PName = Intent.GetStringExtra(name);

            switch (Convert.ToInt32(PName))
            {
                case 0:
                    data.Text = AccountsController.mainAccP.loginP;
                    text.Text = "Выберите имя своей учетной записи, которое будет отображаться в Вашем публичном профиле. Изменение логина приведет к выходу из учетной записи!";
                    break;
                case 1:
                    data.Text = AccountsController.mainAccP.emailP;
                    text.Text = "Введите доступный Вам электронный адрес. На него придет письмо с подтверждением. Изменение электронного адреса приведет к выходу из учетной записи!";
                    break;
                case 2:
                    data.InputType = Android.Text.InputTypes.TextVariationPassword | Android.Text.InputTypes.ClassText;
                    oldpass.InputType = Android.Text.InputTypes.TextVariationPassword | Android.Text.InputTypes.ClassText;
                    text.SetPadding(10, 300, 10, 0);
                    data.Hint = "Новый пароль";
                    oldpass.Hint = "Старый пароль";
                    oldpass.Visibility = ViewStates.Visible;
                    text.Text = "Внимание! Изменение пароля приведет к выходу из учетной записи!";
                    break;
            }

            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#2196f3")));
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            try
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Предупреждение");
                builder.SetMessage("Вы уверены, что хотите изменить данные учетной записи?");
                builder.SetCancelable(true);
                builder.SetNegativeButton("Нет", (s, ev) =>
                {

                });
                FirebaseController.instance.initFireBaseAuth();
                switch (item.ItemId)
                {
                    case Android.Resource.Id.Home:
                        Finish();
                        return true;
                    case Resource.Id.okay:
                        if (text.Text.Contains("имя"))
                        {
                            if (Utils.IsCorrectLogin(data.Text))
                            {
                                builder.SetPositiveButton("Да", (s, ev) =>
                                {
                                    AccountsController.mainAccP.loginP = data.Text;
                                    AccountsController.instance.CreateTable();
                                    AccountsController.instance.SaveItem(AccountsController.mainAccP);
                                    progress.Visibility = ViewStates.Visible;
                                    turnOnControls(false);
                                    change_login();
                                });
                                Dialog dialog = builder.Create();
                                dialog.Show();
                                return true;
                            }
                        }
                        if (text.Text.Contains("адрес"))
                        {
                            if (Utils.IsCorrectEmail(data.Text))
                            {
                                builder.SetPositiveButton("Да", (s, ev) =>
                                {
                                    AccountsController.mainAccP.emailP = data.Text;
                                    AccountsController.instance.CreateTable();
                                    AccountsController.instance.SaveItem(AccountsController.mainAccP);
                                    progress.Visibility = ViewStates.Visible;
                                    turnOnControls(false);
                                    change_mail();
                                });
                                Dialog dialog = builder.Create();
                                dialog.Show();
                                return true;
                            }
                        }
                        if (text.Text.Contains("пароля"))
                        {
                            builder.SetPositiveButton("Да", (s, ev) =>
                            {
                                if (oldpass.Text == AccountsController.mainAccP.passwordP)
                                {
                                    AccountsController.mainAccP.passwordP = data.Text;
                                    AccountsController.instance.CreateTable();
                                    AccountsController.instance.SaveItem(AccountsController.mainAccP);
                                    progress.Visibility = ViewStates.Visible;
                                    turnOnControls(false);
                                    change_pass();
                                }
                                else
                                {
                                    oldpass.Text = "";
                                    data.Text = "";
                                    Utils.MessageBox("Некорректный пароль учетной записи!", this);
                                }
                            });
                            Dialog dialog = builder.Create();
                            dialog.Show();
                            return true;
                        }
                        return true;
                    default:
                        return base.OnOptionsItemSelected(item);
                }
            }
            catch(Exception ex)
            {
                Utils.MessageBox(ex.Message, this);
                return false;
            }
        }

        private void change_mail()
        {
            FirebaseController.instance.ResetEmail(data.Text, this);
        }

        public void change_pass()
        {
            FirebaseController.instance.ChangePass(data.Text, this);
        }

        public void change_login()
        {
            FirebaseController.instance.ChangeLogin(data.Text, this);
        }

        public void OnComplete(Task task)
        {
            if (task.IsComplete)
            {
                Toast.MakeText(this, "Completed", ToastLength.Short);
                turnOnControls(true);
                progress.Visibility = ViewStates.Invisible;
                AccountsController.mainAccP = null;
                AccountsController.instance.CreateTable();
                foreach(var acc in AccountsController.instance.deviceAccsP)
                {
                    acc.isMainP = false;
                    AccountsController.instance.SaveItem(acc);
                }
                Finish();
                Intent intent = new Intent(this, typeof(SignUp));
                intent.SetFlags(ActivityFlags.ClearTask);
                StartActivity(intent);
            }
        }

        private void turnOnControls(bool c)
        {
            data.Enabled = c;
        }
    }
}