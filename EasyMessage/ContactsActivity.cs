﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Button;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using EasyMessage.Adapters;
using EasyMessage.Controllers;
using EasyMessage.Entities;
using Firebase.Database;

namespace EasyMessage
{
    [Activity(Label = "Контакты")]
    public class ContactsActivity : AppCompatActivity, IValueEventListener, NavigationView.IOnNavigationItemSelectedListener
    {
        private List<Contact> contacts;
        private ListView list;
        private DrawerLayout drawer;
        private NavigationView navigation;
        private Android.Support.V7.Widget.Toolbar tb;
        private ContactItemAdapter adapter;
        private MaterialButton search;
        public void OnCancelled(DatabaseError error)
        {
            //throw new NotImplementedException();
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            Utils.MessageBox("changed", this);
        }
        

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ContactsController.instance.CreateTable();
            contacts = ContactsController.instance.GetItems().ToList();
            if (contacts == null || contacts.Count == 0)
            {
                Task<List<Contact>> contactsTask = FirebaseController.instance.GetAllContacts(AccountsController.mainAccP.emailP, this);
                contacts = await contactsTask;
                foreach(var cont in contacts)
                {
                    ContactsController.instance.SaveItem(cont);
                }
            }

            SetContentView(Resource.Layout.contacts);

            search = FindViewById<MaterialButton>(Resource.Id.searchUser);
            list = FindViewById<ListView>(Resource.Id.contactList);
            adapter = new ContactItemAdapter(fillList());
            list.Adapter = adapter;

            search.Click += delegate
            {
                search_click();
            };

            list.ItemClick += (sender, e) =>
            {
                item_click(sender, e);
            };

            tb = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.tooolbarCommon);

            drawer = FindViewById<DrawerLayout>(Resource.Id.drawerLayout1);

            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, Resource.String.openDrawer, Resource.String.closeDrawer);
            toggle.SyncState();

            navigation = FindViewById<NavigationView>(Resource.Id.navigationMain);
            navigation.SetNavigationItemSelectedListener(this);

            SetSupportActionBar(tb);
            Android.Support.V7.App.ActionBar abar = SupportActionBar;

            abar.SetHomeButtonEnabled(true);
            abar.SetDisplayShowTitleEnabled(true);
            abar.SetHomeAsUpIndicator(Resource.Drawable.menu);
            abar.SetDisplayHomeAsUpEnabled(true);

            
            
        }

        private void search_click()
        {
            StartActivity(new Intent(this, typeof(SearchUser)));
        }

        private void item_click(object sender, AdapterView.ItemClickEventArgs e)
        {
            Contact c = adapter[Convert.ToInt32(e.Id)];
            Toast.MakeText(this, c.contactNameP, ToastLength.Short).Show();
        }

        private IList<Contact> fillList()
        {
            /*var list = new List<Contact>();
            list.Add(new Contact { contactAddressP = "kirill_kovrik@mail.ru", contactNameP = "kirill" });
            list.Add(new Contact { contactAddressP = "kirill.kop.work@gmail.com", contactNameP = "kolya" });
            list.Add(new Contact { contactAddressP = "geniuses1studio@gmail.com", contactNameP = "katya" });*/

            return contacts;
        }

        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            Intent intent;
            drawer.CloseDrawer(GravityCompat.Start);
            switch (menuItem.ItemId)
            {
                case Resource.Id.nav_contacts:
                    return true;
                case Resource.Id.nav_dialogs:
                    intent = new Intent(this, typeof(MainPage));
                    intent.AddFlags(ActivityFlags.NoAnimation);
                    intent.AddFlags(ActivityFlags.ClearTop);
                    StartActivity(intent);
                    return true;
                case Resource.Id.nav_settings:
                    intent = new Intent(this, typeof(SettingsActivity));
                    intent.AddFlags(ActivityFlags.NoAnimation);
                    intent.AddFlags(ActivityFlags.ClearTop);
                    StartActivity(intent);
                    return true;
                case Resource.Id.nav_profile:
                    intent = new Intent(this, typeof(Profile));
                    intent.AddFlags(ActivityFlags.NoAnimation);
                    intent.AddFlags(ActivityFlags.ClearTop);
                    StartActivity(intent);
                    return true;
            }
            return base.OnOptionsItemSelected(menuItem);
        }

        public override void OnBackPressed()
        {
            DrawerLayout drw = FindViewById<DrawerLayout>(Resource.Id.drawerLayout1);

            if (drw.IsDrawerOpen(GravityCompat.Start))
            {
                drw.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case 16908332:
                    if (drawer.IsDrawerOpen(GravityCompat.Start))
                    {
                        drawer.CloseDrawer(GravityCompat.Start);
                    }
                    else
                    {
                        drawer.OpenDrawer(GravityCompat.Start);
                    }
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void sendMessage(string rec, string mes, string sendr)
        {
            //FirebaseDatabase.Instance.GetReference("chat").AddValueEventListener(this);
            //var items = await  
            FirebaseController.instance.SendMessage("7", "8", "9", this);
        }
    }
}