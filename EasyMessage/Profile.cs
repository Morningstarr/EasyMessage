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
        private Button deleteUser;
        private Button changeUser;
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

            deleteUser = FindViewById<Button>(Resource.Id.button1);
            changeUser = FindViewById<Button>(Resource.Id.button2);
            deleteUser.Text = "Удалить учетную запись";
            changeUser.Text = "Сменить учетную запись";

            deleteUser.Click += delegate
            {
                delete_user();
            };

            changeUser.Click += delegate
            {
                change_user();
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
                    foreach(var d in DialogsController.instance.GetItems().ToList())
                    {
                        DialogsController.instance.DeleteItem(d.Id);
                    }
                    MessagesController.instance.CreateTable();
                    foreach(var m in MessagesController.instance.GetItems().ToList())
                    {
                        MessagesController.instance.DeleteItem(m.Id);
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
            catch(Exception ex)
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
                    pbar.Visibility = ViewStates.Visible;
                    disableControls();
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
            deleteUser.Enabled = false;
            changeUser.Enabled = false;
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