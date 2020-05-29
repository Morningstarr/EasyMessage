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
using Android.Support.Design.Button;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using EasyMessage.Adapters;
using EasyMessage.Entities;

namespace EasyMessage
{
    [Activity(Label = "SearchUser", Theme = "@style/Theme.MaterialComponents.Light.DarkActionBar")]
    public class SearchUser : AppCompatActivity
    {
        private EditText email;
        private MaterialButton search;
        private ProgressBar pb;
        private ListView found;
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

            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#2196f3")));
            // Create your application here
        }

        private void search_user()
        {
            pb.Visibility = ViewStates.Visible;
            email.Enabled = false;
            search.Enabled = false;

            Contact fnd = FirebaseController.instance.IsUserRegistered(email.Text, this);
            if (found == null)
            {
                Utils.MessageBox("Пользователя с таким электронным адресом не существует", this);
            }
            else
            {
                var list = new List<Contact>();
                list.Add(fnd);
                var adapter = new ContactItemAdapter(list);
                found.Adapter = adapter;
                found.Visibility = ViewStates.Visible;
            }
            pb.Visibility = ViewStates.Invisible;
            email.Enabled = true;
            search.Enabled = true;
        }

    }
}