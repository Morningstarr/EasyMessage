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
using Android.Views;
using Android.Widget;
using EasyMessage.Controllers;
using EasyMessage.Entities;
using Newtonsoft.Json;

namespace EasyMessage
{
    [Activity(Label = "Подробности", Theme = "@style/Theme.MaterialComponents.Light.DarkActionBar")]
    public class ContactDetails : AppCompatActivity
    {
        private Button delete;
        private Button save;
        private Button openDialog;
        private Button changeLogin;
        private TextView cName;
        private TextView cMail;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Contact c = ContactsController.currContP; /*ContactsController.instance.GetItem(*/;
            SetContentView(Resource.Layout.contact_details);

            cName = FindViewById<TextView>(Resource.Id.cName);
            cMail = FindViewById<TextView>(Resource.Id.cMail);
            changeLogin = FindViewById<Button>(Resource.Id.changeLogin);

            changeLogin.Click += delegate
            {
                Intent i = new Intent(this, typeof(EditContactData));
                i.SetFlags(ActivityFlags.NoAnimation);
                //i.PutExtra("account", AccountsController.mainAccP.emailP);
                //i.PutExtra("oldName", cName.Text);
                //i.PutExtra("id", c.Id);
                StartActivity(i);
            };

            cName.Text = c.contactNameP;
            cMail.Text = c.contactAddressP;

            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#2196f3")));
            // Create your application here
        }

        protected override void OnResume()
        {
            Contact c = ContactsController.currContP;
            cName = FindViewById<TextView>(Resource.Id.cName);
            cMail = FindViewById<TextView>(Resource.Id.cMail);
            cName.Text = c.contactNameP;
            cMail.Text = c.contactAddressP;
            base.OnResume();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                default:
                    return true;
            }

        }
    }
}