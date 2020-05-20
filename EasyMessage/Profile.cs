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
using EasyMessage.Entities;
using EasyMessage.Controllers;

namespace EasyMessage
{
    [Activity(Label = "Profile", Theme = "@style/Theme.AppCompat.Light.DarkActionBar")]
    public class Profile : Activity
    {
        private ListView list;
        private TextView username;
        private Button deleteUser;
        private Button changeUser;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.profile);

            username = FindViewById<TextView>(Resource.Id.username);
            username.Text = AccountsController.mainAccP.loginP;

            list = FindViewById<ListView>(Resource.Id.settingslist);
            var adapter = new ListItemAdapter(fillList());
            list.Adapter = adapter;

            list.ItemClick += (sender, e) =>
            {
                item_click(sender, e);
            };


            deleteUser = FindViewById<Button>(Resource.Id.button1);
            changeUser = FindViewById<Button>(Resource.Id.button2);

            deleteUser.Click += delegate
            {
                delete_user();
            };

            changeUser.Click += delegate
            {
                change_user();
            };
        }

        private void change_user()
        {
            try
            {
               
            }
            catch(Exception ex)
            {
                Utils.MessageBox(ex.Message, this);
            }
        }

        private void delete_user()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Utils.MessageBox(ex.Message, this);
            }
        }

        protected override void OnRestart()
        {
            base.OnRestart();
            username = FindViewById<TextView>(Resource.Id.username);
            username.Text = AccountsController.mainAccP.loginP;

            list = FindViewById<ListView>(Resource.Id.settingslist);
            var adapter = new ListItemAdapter(fillList());
            list.Adapter = adapter;
        }


        private void item_click(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent intent = new Intent(this, typeof(EditProfileData));
            intent.PutExtra("id", e.Id.ToString());
            StartActivity(intent);
        }

        private IList<ItemTemplate> fillList()
        {
            var list = new List<ItemTemplate>();
            list.Add(ItemTemplate.convertToItem("Нажмите чтобы изменить логин", AccountsController.mainAccP, 1));
            list.Add(ItemTemplate.convertToItem("Нажмите чтобы изменить электронный адрес", AccountsController.mainAccP, 2));
            list.Add(ItemTemplate.convertToItem("Нажмите чтобы изменить пароль", AccountsController.mainAccP, 3));

            return list;
        }
    }
}