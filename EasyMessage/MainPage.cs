using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace EasyMessage
{
    [Activity(Label = "MainPage")]
    public class MainPage : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private DrawerLayout drawer;
        private NavigationView navigation;
        private Android.Support.V7.Widget.Toolbar tb;

        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            switch (menuItem.ItemId)
            {
                case Resource.Id.nav_contacts:
                    Toast.MakeText(this, "123", ToastLength.Short).Show();
                    return true;
            }
            return base.OnOptionsItemSelected(menuItem);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.main_page);
            tb = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar1);

            drawer = FindViewById<DrawerLayout>(Resource.Id.drawerLayout1);

            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, Resource.String.openDrawer, Resource.String.closeDrawer);
            toggle.SyncState();

            navigation = FindViewById<NavigationView>(Resource.Id.navigationView2);
            navigation.SetNavigationItemSelectedListener(this);

            SetSupportActionBar(tb);
            Android.Support.V7.App.ActionBar abar = SupportActionBar;

            abar.SetHomeButtonEnabled(true);
            abar.SetDisplayShowTitleEnabled(true);
            abar.SetHomeAsUpIndicator(Resource.Drawable.menu);
            abar.SetDisplayHomeAsUpEnabled(true);
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

    }
}