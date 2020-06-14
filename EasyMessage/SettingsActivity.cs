using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using EasyMessage.Adapters;
using EasyMessage.Controllers;
using EasyMessage.Entities;
using AlertDialog = Android.App.AlertDialog;

namespace EasyMessage
{
    [Activity(Label = "Настройки")]
    public class SettingsActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener, IOnCompleteListener
    {
        private DrawerLayout drawer;
        private Android.Support.V7.Widget.Toolbar tb;
        private NavigationView navigation;
        private ListView settingsList;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.settings);

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

            settingsList = FindViewById<ListView>(Resource.Id.setts);
            var adapter = new SettingsItemAdapter(fillList());
            settingsList.Adapter = adapter;

            settingsList.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                if (e.Position == 0)
                {
                    change_user();
                }
                if (e.Position == 1)
                {
                    delete_user();
                }
                if (e.Position == 2)
                {
                    System.Environment.Exit(0);
                }
            };

            SetSupportActionBar(tb);
            Android.Support.V7.App.ActionBar abar = SupportActionBar;

            
            abar.SetHomeButtonEnabled(true);
            abar.SetDisplayShowTitleEnabled(true);
            abar.SetHomeAsUpIndicator(Resource.Drawable.menu);
            abar.SetDisplayHomeAsUpEnabled(true);
        }

        private IList<SettingsItem> fillList()
        {
            var list = new List<SettingsItem>();
            list.Add(new SettingsItem { sett_description = "Нажмите, чтобы сменить учетную запись", sett_value = "Сменить аккаунт" });
            list.Add(new SettingsItem { sett_description = "Нажмите, чтобы удалить учетную запись", sett_value = "Удалить аккаунт" });
            list.Add(new SettingsItem { sett_description = "Нажмите, чтобы выйти", sett_value = "Выход" });

            return list;
        }

        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            Intent intent;
            drawer.CloseDrawer(GravityCompat.Start);
            switch (menuItem.ItemId)
            {
                case Resource.Id.nav_settings:
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

        private void change_user()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Предупреждение");
            builder.SetMessage("Вы уверены, что хотите сменить учетную запись?");
            builder.SetCancelable(true);
            builder.SetNegativeButton("Нет", (s, ev) =>
            {

            });
            try
            {
                builder.SetPositiveButton("Да", (s, ev) =>
                {
                    AccountsController.mainAccP = null;
                    AccountsController.instance.CreateTable();
                    //находить по id только текущего пользователя (тоже самое в EditProfile)
                    var acc = AccountsController.instance.deviceAccsP.Find(x => x.isMainP == true);
                    acc.isMainP = false;
                    AccountsController.instance.SaveItem(acc);
                    FirebaseController.instance.initFireBaseAuth();
                    FirebaseController.instance.LogOut();
                    ContactsController.instance.CreateTable();
                    foreach (var item in ContactsController.instance.GetItems())
                    {
                        ContactsController.instance.DeleteItem(item.Id);
                    }
                    DialogsController.instance.CreateTable();
                    foreach (var d in DialogsController.instance.GetItems().ToList())
                    {
                        DialogsController.instance.DeleteItem(d.Id);
                    }
                    MessagesController.instance.CreateTable();
                    foreach (var m in MessagesController.instance.GetItems().ToList())
                    {
                        if (m.decodedP == null)
                        {
                            MessagesController.instance.DeleteItem(m.Id);
                        }
                    }
                    Finish();
                    Intent intent = new Intent(this, typeof(SignUp));
                    intent.SetFlags(ActivityFlags.ClearTask);
                    StartActivity(intent);

                });
                Dialog dialog = builder.Create();
                dialog.Show();
                return;
            }
            catch (Exception ex)
            {
                Utils.MessageBox(ex.Message, this);
            }
        }

        private void delete_user()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Предупреждение");
            builder.SetMessage("Вы уверены, что хотите удалить учетную запись?");
            builder.SetCancelable(true);
            builder.SetNegativeButton("Нет", (s, ev) =>
            {

            });
            try
            {
                builder.SetPositiveButton("Да", (s, ev) =>
                {
                    //pbar.Visibility = ViewStates.Visible;
                    AccountsController.instance.CreateTable();
                    AccountsController.instance.DeleteItem(AccountsController.mainAccP.Id);
                    AccountsController.mainAccP = null;
                    foreach (var item in ContactsController.instance.GetItems())
                    {
                        ContactsController.instance.DeleteItem(item.Id);
                    }
                    DialogsController.instance.CreateTable();
                    foreach (var d in DialogsController.instance.GetItems().ToList())
                    {
                        DialogsController.instance.DeleteItem(d.Id);
                    }
                    MessagesController.instance.CreateTable();
                    foreach (var m in MessagesController.instance.GetItems().ToList())
                    {
                        MessagesController.instance.DeleteItem(m.Id);
                    }
                    FirebaseController.instance.initFireBaseAuth();
                    FirebaseController.instance.DeleteUser(this);

                });
                Dialog dialog = builder.Create();
                dialog.Show();
                return;
            }
            catch (Exception ex)
            {
                Utils.MessageBox(ex.Message, this);
            }
        }

        public void OnComplete(Task task)
        {
            //throw new NotImplementedException();
        }
    }
}