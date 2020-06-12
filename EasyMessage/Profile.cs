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
using Android.Support.V7.App;
using Android.Gms.Tasks;
using AlertDialog = Android.App.AlertDialog;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V4.View;
using EasyMessage.Adapters;

namespace EasyMessage
{
    [Activity(Label = "Профиль")]
    public class Profile : AppCompatActivity, IOnCompleteListener, NavigationView.IOnNavigationItemSelectedListener
    {
        private ListView list;
        private TextView username;
        private ProgressBar pbar;
        private DrawerLayout drawer;
        private Android.Support.V7.Widget.Toolbar tb;
        private NavigationView navigation;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.profile);

            pbar = FindViewById<ProgressBar>(Resource.Id.progressBar3);
        
            username = FindViewById<TextView>(Resource.Id.username);
            username.Text = AccountsController.mainAccP.loginP;

            list = FindViewById<ListView>(Resource.Id.settingslist);
            var adapter = new ListItemAdapter(fillList());
            list.Adapter = adapter;

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
            var header = navigation.GetHeaderView(0);
            TextView name = header.FindViewById<TextView>(Resource.Id.nav_bar_name);
            name.Text = AccountsController.mainAccP.loginP;
            TextView ml = header.FindViewById<TextView>(Resource.Id.nav_bar_addr);
            ml.Text = AccountsController.mainAccP.emailP;

            SetSupportActionBar(tb);
            Android.Support.V7.App.ActionBar abar = SupportActionBar;

            abar.SetHomeButtonEnabled(true);
            abar.SetDisplayShowTitleEnabled(true);
            abar.SetHomeAsUpIndicator(Resource.Drawable.menu);
            abar.SetDisplayHomeAsUpEnabled(true);
        }

        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            Intent intent;
            drawer.CloseDrawer(GravityCompat.Start);
            switch (menuItem.ItemId)
            {
                case Resource.Id.nav_profile:
                    return true;
                case Resource.Id.nav_dialogs:
                    intent = new Intent(this, typeof(MainPage));
                    intent.AddFlags(ActivityFlags.NoAnimation);
                    intent.AddFlags(ActivityFlags.ClearTop);
                    StartActivity(intent);
                    return true;
                case Resource.Id.nav_contacts:
                    intent = new Intent(this, typeof(ContactsActivity));
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
                default:
                    return base.OnOptionsItemSelected(item);
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

        public void disableControls()
        {
            list.Enabled = false;
        }

        public void OnComplete(Task task)
        {
            if (task.IsComplete)
            {
                Finish();
                Intent intent = new Intent(this, typeof(SignUp));
                intent.SetFlags(ActivityFlags.ClearTask);
                StartActivity(intent);
            }
        }
    }
}