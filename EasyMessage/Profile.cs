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
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.profile);

            list = FindViewById<ListView>(Resource.Id.settingslist);
            username = FindViewById<TextView>(Resource.Id.username);

            username.Text = AccountsController.mainAccP.loginP;

            List<string> elements = new List<string>();
            elements.AddRange(new string[] { "Электронный адрес", "Пароль", "Логин" });

            //ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Resource.Layout.list_item, elements);
            var adapter = new ListItemAdapter(fillList());

            list.Adapter = adapter;
            // Create your application here
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