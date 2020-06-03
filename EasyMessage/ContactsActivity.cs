using System;
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
using Newtonsoft.Json;

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
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ContactsController.instance.CreateTable();
            contacts = ContactsController.instance.GetItems().ToList();

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

        protected override void OnRestart()
        {
            base.OnResume();

            list = FindViewById<ListView>(Resource.Id.contactList);
            var temp = contacts.Where(x => x.Id == ContactsController.currContP.Id).ToList()[0];
            contacts.Remove(temp);
            contacts.Add(ContactsController.currContP);
            adapter = new ContactItemAdapter(fillList());
            list.Adapter = adapter;
        }

        private void search_click()
        {
            StartActivity(new Intent(this, typeof(SearchUser)));
        }

        private void item_click(object sender, AdapterView.ItemClickEventArgs e)
        {
            Contact c = adapter[Convert.ToInt32(e.Id)];
            ContactsController.currContP = c;
            Intent i = new Intent(this, typeof(ContactDetails));
            //i.PutExtra("contact", JsonConvert.SerializeObject(c));
            i.SetFlags(ActivityFlags.NoAnimation);
            StartActivity(i);
        }

        private IList<Contact> fillList()
        {
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