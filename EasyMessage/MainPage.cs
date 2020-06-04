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
using EasyMessage.Controllers;
using EasyMessage.Adapters;
using EasyMessage.Entities;
using Message = EasyMessage.Entities.Message;
using System.Threading.Tasks;
using System.Threading;
using AlertDialog = Android.App.AlertDialog;

namespace EasyMessage
{
    [Activity(Label = "Easy Message")]
    public class MainPage : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private DrawerLayout drawer;
        private NavigationView navigation;
        private Android.Support.V7.Widget.Toolbar tb;
        private Button check;
        private List<string> newDialogs = new List<string>();
        private List<Message> messages = new List<Message>();
        private DialogItemAdapter adapter;
        private ListView dialogs;
        private ProgressBar checkProgress;
        private List<MyDialog> dialogg = new List<MyDialog>();
        public delegate bool DisplayHandler();

        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            Intent intent;
            drawer.CloseDrawer(GravityCompat.Start);
            switch (menuItem.ItemId)
            {
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
                case Resource.Id.nav_dialogs:
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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.main_page);
            tb = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.tooolbarCommon);

            drawer = FindViewById<DrawerLayout>(Resource.Id.drawerLayout1);

            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, Resource.String.openDrawer, Resource.String.closeDrawer);
            toggle.SyncState();

            navigation = FindViewById<NavigationView>(Resource.Id.navigationMain);
            navigation.SetNavigationItemSelectedListener(this);

            checkProgress = FindViewById<ProgressBar>(Resource.Id.checkProgress);
            dialogs = FindViewById<ListView>(Resource.Id.dialogsList);

            dialogs.ItemClick += (sender, e) =>
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Предупреждение");
                builder.SetMessage("Разрешить начать диалог с данным пользователем?");
                builder.SetCancelable(true);
                builder.SetNegativeButton("Нет", async (s, ev) =>
                {
                    Message c = adapter[Convert.ToInt32(e.Id)];
                    MyDialog temp = dialogg.Find(x => x.lastMessage == c);
                    Task<bool> denialTask = FirebaseController.instance.SendDialogDenial(temp.dialogName, c.senderP);
                    bool answer = await denialTask;
                    checkProgress.Visibility = ViewStates.Visible;
                    if (answer)
                    {
                        dialogg.Remove(temp);
                        messages.Remove(c);
                        refresh_dialogs();
                        Utils.MessageBox("Успешно!", this);
                    }
                    else
                    {
                        Utils.MessageBox("Ошибка! Повторите позже.", this);
                    }
                    checkProgress.Visibility = ViewStates.Invisible;
                });
                builder.SetPositiveButton("Да", async (s, ev) =>
                {
                    Message c = adapter[Convert.ToInt32(e.Id)];
                    MyDialog temp = dialogg.Find(x => x.lastMessage == c);
                    Task<bool> responseTask = FirebaseController.instance.SendDialogResponse(temp.dialogName, c.senderP);
                    checkProgress.Visibility = ViewStates.Visible;
                    bool _answer = await responseTask;
                    if (_answer) 
                    {
                        dialogg.Remove(temp);
                        messages.Remove(c);
                        refresh_dialogs();
                        ContactsController.instance.CreateTable();
                        ContactsController.instance.SaveItem(new Contact { contactAddressP = c.senderP, dialogNameP = temp.dialogName, contactNameP = "user name" }, false);
                        Task<int> firstIdtask = FirebaseController.instance.ReturnLastId(AccountsController.mainAccP.emailP, this);
                        int firstId = await firstIdtask;
                        FirebaseController.instance.AddContact(c.senderP, AccountsController.mainAccP.emailP, firstId + 1);
                        Task<int> secondIdtask = FirebaseController.instance.ReturnLastId(c.senderP, this);
                        int secondId = await secondIdtask;
                        FirebaseController.instance.AddContact(AccountsController.mainAccP.emailP, c.senderP, secondId + 1);
                        Utils.MessageBox("Успешно!", this);
                    }
                    else
                    {
                        Utils.MessageBox("Ошибка! Повторите позже.", this);
                    }
                    checkProgress.Visibility = ViewStates.Invisible;
                    
                });
                Dialog dialog = builder.Create();
                dialog.Show();
                return;
            };

            SetSupportActionBar(tb);
            Android.Support.V7.App.ActionBar abar = SupportActionBar;

            abar.SetHomeButtonEnabled(true);
            abar.SetDisplayShowTitleEnabled(true);
            abar.SetHomeAsUpIndicator(Resource.Drawable.menu);
            abar.SetDisplayHomeAsUpEnabled(true);

            check = FindViewById<Button>(Resource.Id.check);

            check.Click += async delegate
            {
                Task<List<MyDialog>> sizeTask = FirebaseController.instance.FindDialogs("Dialog " + AccountsController.mainAccP.emailP + "+", this);
                checkProgress.Visibility = ViewStates.Visible;
                check.Enabled = false;
                dialogg = await sizeTask;
                if (dialogg == null || dialogg.Count == 0)
                {
                    Utils.MessageBox("Нет новых запросов!", this);  
                }
                else
                {
                    refresh_dialogs();
                }
                checkProgress.Visibility = ViewStates.Invisible;
                check.Enabled = true;
            };
        }

        private void refresh_dialogs()
        {
            adapter = new DialogItemAdapter(fillList());
            dialogs.Adapter = adapter;
        }

        private IList<Message> fillList()
        {
            foreach(var p in dialogg)
            {
                messages.Add(p.lastMessage);
            }
            return messages;
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
                        //navigation.
                    }
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

    }
}