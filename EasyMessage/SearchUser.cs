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
using Android.Support.Design.Button;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using EasyMessage.Adapters;
using EasyMessage.Controllers;
using EasyMessage.Entities;
using Firebase.Auth;

namespace EasyMessage
{
    [Activity(Label = "SearchUser", Theme = "@style/Theme.MaterialComponents.Light.DarkActionBar")]
    public class SearchUser : AppCompatActivity
    {
        private EditText email;
        private MaterialButton search;
        private ProgressBar pb;
        private ListView found;
        private ContactItemAdapter adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.search_user);

            email = FindViewById<EditText>(Resource.Id.userEmail);
            search = FindViewById<MaterialButton>(Resource.Id.searchButton);
            pb = FindViewById<ProgressBar>(Resource.Id.progressSearch);
            found = FindViewById<ListView>(Resource.Id.foundUser);

            search.Click += delegate
            {
                search_user();
            };

            found.ItemClick += (sender, e) =>
            {
                item_click(sender, e);
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

        private async void item_click(object sender, AdapterView.ItemClickEventArgs e)
        {
            pb.Visibility = ViewStates.Visible;
            email.Enabled = false;
            search.Enabled = false;
            try
            {
                string n = await FirebaseController.instance.SendDialogRequest(adapter[Convert.ToInt32(e.Id)].contactAddressP);

                Utils.MessageBox("Запрос отправлен! Когда пользователь его примет, вы получите уведомление.", this);
                found.Visibility = ViewStates.Invisible;
            }
            catch(Exception ex)
            {
                Utils.MessageBox(ex.Message, this);
            }
            pb.Visibility = ViewStates.Invisible;
            email.Enabled = true;
            search.Enabled = true;
        }

        private async void search_user()
        {
            try
            {
                pb.Visibility = ViewStates.Visible;
                email.Enabled = false;
                search.Enabled = false;

                if(email.Text == AccountsController.mainAccP.emailP)
                {
                    throw new SystemException("Данный адрес электронной почты совпадает с адресом вашего аккаунта!");
                }

                if (FirebaseController.instance.app == null)
                {
                    FirebaseController.instance.initFireBaseAuth();
                }
                string s = await FirebaseController.instance.LoginUser(email.Text, "1");

            }
            catch (FirebaseAuthInvalidCredentialsException ex1)
            {
                if (ex1.Message.Contains("password is invalid"))
                {
                    Contact fnd = new Contact { contactAddressP = email.Text };
                    var list = new List<Contact>();
                    list.Add(fnd);
                    adapter = new ContactItemAdapter(list);
                    found.Adapter = adapter;
                    found.Visibility = ViewStates.Visible;
                    Utils.MessageBox("Пользователь найден! Нажмите на появившийся элемент, чтобы отправить запрос на диалог", this);
                }
                else
                {
                    if (ex1.Message.Contains("email address is badly formatted"))
                    {
                        Utils.MessageBox("Некорректный адрес электронной почты!", this);
                    }
                    else
                    {
                        Utils.MessageBox("Непредвиденная ошибка, повторите попытку позже", this);
                    }
                }
            }
            catch (FirebaseAuthInvalidUserException ex2)
            {
                if (ex2.Message.Contains("no user record"))
                {
                    Utils.MessageBox("Пользователя с таким электронным адресом не существует", this);
                }
            }
            catch(SystemException ex3)
            {
                Utils.MessageBox(ex3.Message, this);
            }
                
            pb.Visibility = ViewStates.Invisible;
            email.Enabled = true;
            search.Enabled = true;
        }

    }
}