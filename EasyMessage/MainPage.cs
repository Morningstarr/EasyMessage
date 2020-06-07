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
using Firebase.Database;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Net;
using EasyMessage.Utilities;

namespace EasyMessage
{
    [Activity(Label = "Easy Message")]
    public class MainPage : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener, IValueEventListener
    {
        private DrawerLayout drawer;
        private NavigationView navigation;
        private Android.Support.V7.Widget.Toolbar tb;
        private Button check;
        private List<Message> messages = new List<Message>();
        private DialogItemAdapter adapter;
        private OldDialogItemAdapter adapterOld;
        private ListView dialogs;
        private ProgressBar checkProgress;
        private List<MyDialog> dialogg = new List<MyDialog>();
        private List<MyDialog> oldDialogs = new List<MyDialog>();
        public delegate bool DisplayHandler();
        private ListView oldDialogsList;
        ConnectivityManager connectivityManager;
        NetworkInfo networkInfo;

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.main_page_menu, menu);
            return true;
        }

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

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
                networkInfo = null;

                DialogsController.instance.CreateTable();
                oldDialogs = DialogsController.instance.GetItems().ToList();

                SetContentView(Resource.Layout.main_page);
                tb = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.tooolbarCommon);

                drawer = FindViewById<DrawerLayout>(Resource.Id.drawerLayout1);

                ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, Resource.String.openDrawer, Resource.String.closeDrawer);
                toggle.SyncState();

                navigation = FindViewById<NavigationView>(Resource.Id.navigationMain);
                navigation.SetNavigationItemSelectedListener(this);

                checkProgress = FindViewById<ProgressBar>(Resource.Id.checkProgress);
                dialogs = FindViewById<ListView>(Resource.Id.dialogsList);
                oldDialogsList = FindViewById<ListView>(Resource.Id.oldDialogsList);

                SetSupportActionBar(tb);
                Android.Support.V7.App.ActionBar abar = SupportActionBar;

                abar.SetHomeButtonEnabled(true);
                abar.SetDisplayShowTitleEnabled(true);
                abar.SetHomeAsUpIndicator(Resource.Drawable.menu);
                abar.SetDisplayHomeAsUpEnabled(true);

                check = FindViewById<Button>(Resource.Id.check);

                check.Click += async delegate
                {
                    networkInfo = connectivityManager.ActiveNetworkInfo;
                    if (networkInfo != null && networkInfo.IsConnected == true)
                    {
                        try
                        {
                            Task<List<MyDialog>> sizeTask = FirebaseController.instance.FindDialogs("Dialog " + AccountsController.mainAccP.emailP + "+", this);
                            checkProgress.Visibility = ViewStates.Visible;
                            check.Enabled = false;
                            dialogs.Enabled = false;
                            oldDialogsList.Enabled = false;
                            dialogg = await sizeTask;
                            if (dialogg == null)
                            {
                                Utils.MessageBox("Нет новых запросов!", this);
                            }
                            else
                            {
                                refresh_dialogs();
                            }
                            checkProgress.Visibility = ViewStates.Invisible;
                            check.Enabled = true;
                            dialogs.Enabled = true;
                            oldDialogsList.Enabled = true;
                        }
                        catch (Exception ex)
                        {
                            Utils.MessageBox("Невозможно загрузить запросы. Проверьте подключение к интернету", this);
                            checkProgress.Visibility = ViewStates.Invisible;
                            check.Enabled = true;
                            dialogs.Enabled = true;
                            oldDialogsList.Enabled = true;
                        }
                    }
                    else
                    {
                        Utils.MessageBox("Невозможно загрузить запросы. Проверьте подключение к интернету", this);
                    }
                };


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

                checkProgress.Visibility = ViewStates.Visible;
                await fillOld();
                if (oldDialogs != null)
                {
                    /**/
                    oldDialogs = oldDialogs.OrderByDescending(x => x.accessFlag).ToList().OrderByDescending(x=>Convert.ToDateTime(x.timeP)).ToList();
                    adapterOld = new OldDialogItemAdapter(oldDialogs);
                }
                else
                {
                    /**/
                    oldDialogs = oldDialogs.OrderByDescending(x => x.accessFlag).ToList().OrderByDescending(x => Convert.ToDateTime(x.timeP)).ToList();
                    oldDialogs = new List<MyDialog>();
                    adapterOld = new OldDialogItemAdapter(oldDialogs);
                }
                oldDialogsList.Adapter = adapterOld;
                checkProgress.Visibility = ViewStates.Invisible;

                oldDialogsList.ItemClick += (sender, e) =>
                {
                    Intent i = new Intent(this, typeof(DialogActivity));
                    i.SetFlags(ActivityFlags.NoAnimation);
                    MyDialog temp = adapterOld[Convert.ToInt32(e.Id)];
                    DialogsController.currDialP = temp;
                    i.PutExtra("dialogName", temp.dialogName);
                    i.PutExtra("receiver",
                        temp.receiverP == AccountsController.mainAccP.emailP ? temp.senderP : temp.receiverP); //изменил (убрал .lastMessage) возможно поэтому что-то сломалось
                    i.PutExtra("flag", 1);
                    StartActivity(i);
                };
            }
            catch(Exception ex)
            {
                //Utils.MessageBox(ex.Message, this);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (DialogsController.currDialP != null)
            { 
                DialogsController.instance.CreateTable();
                oldDialogs = DialogsController.instance.GetItems().ToList();
                /**/
                oldDialogs = oldDialogs.OrderByDescending(x => x.accessFlag).ToList().OrderByDescending(x => Convert.ToDateTime(x.timeP)).ToList();
                adapterOld = new OldDialogItemAdapter(oldDialogs);
                oldDialogsList.Adapter = adapterOld;
                DialogsController.currDialP = null;
            }

        }

        private async Task<bool> fillOld()
        {
            networkInfo = connectivityManager.ActiveNetworkInfo;
            if (networkInfo != null && networkInfo.IsConnected == true)
            {
                try
                {
                    Task<List<MyDialog>> oldTask = FirebaseController.instance.FindOldDialogs(AccountsController.mainAccP.emailP, this);
                    oldDialogs = await oldTask;

                    if (oldDialogs != null)
                    {
                        DialogsController.instance.CreateTable();
                        foreach (var d in oldDialogs)
                        {
                            d.FillFields();
                            DialogsController.instance.SaveItem(d);
                        }

                        var newDialos = oldDialogs.FindAll(x => Convert.ToInt32(x.lastMessage.flags[0]) == 3);
                        if (newDialos != null)
                        {
                            ContactsController.instance.CreateTable();
                            foreach (var d in newDialos)
                            {
                                if (d.lastMessage.receiverP != AccountsController.mainAccP.emailP)
                                {
                                    if (ContactsController.instance.FindContact(d.lastMessage.receiverP) == null)
                                    {
                                        ContactsController.instance.SaveItem(new Contact
                                        {
                                            contactAddressP = d.lastMessage.receiverP,
                                            contactNameP = "user name",
                                            deletedP = false,
                                            dialogNameP = d.dialogName
                                        });
                                    }
                                }
                                else
                                {
                                    if (d.lastMessage.senderP != AccountsController.mainAccP.emailP)
                                    {
                                        if (ContactsController.instance.FindContact(d.lastMessage.senderP) == null)
                                        {
                                            ContactsController.instance.SaveItem(new Contact
                                            {
                                                contactAddressP = d.lastMessage.senderP,
                                                contactNameP = "user name",
                                                deletedP = false,
                                                dialogNameP = d.dialogName
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }


                    if (FirebaseController.instance.app == null)
                    {
                        FirebaseController.instance.initFireBaseAuth();
                    }
                    FirebaseDatabase databaseInstance = FirebaseDatabase.GetInstance(FirebaseController.instance.app);
                    var userNode = databaseInstance.GetReference("chats");
                    foreach (var dialog in oldDialogs)
                    {
                        DatabaseReference u = userNode.Child(dialog.dialogName);
                        u.AddValueEventListener(this);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Utils.MessageBox("Сетевая ошибка! Проверьте подключение к интернету и повторите запрос.", this);
                    return false;
                }
            }
            else
            {
                Utils.MessageBox("Обновление невозможно. Проверьте подключение к интернету и повторите запрос.", this);
                return false;
            }
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
                case 2131230894:
                    checkProgress.Visibility = ViewStates.Visible;
                    fillOld();
                    checkProgress.Visibility = ViewStates.Invisible;
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public void OnCancelled(DatabaseError error)
        {
            //throw new NotImplementedException();
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            try
            {
                IEnumerable<DataSnapshot> items = snapshot.Children?.ToEnumerable<DataSnapshot>();
                List<DataSnapshot> t = items.ToList();
                var a = t.Last().Children.ToEnumerable<DataSnapshot>().ToList();

                var access = a[0].Child("0").Value;
                List<AccessFlags> acs = new List<AccessFlags>();
                acs.Add((AccessFlags)Convert.ToInt32(access.ToString()));

                var flag = a[2].Child("0").Value;
                List<MessageFlags> fls = new List<MessageFlags>();
                fls.Add((MessageFlags)Convert.ToInt32(flag.ToString()));

                Message m = new Message
                {
                    contentP = a[1].Value.ToString(),
                    flags = fls,
                    access = acs,
                    receiverP = a[3].Value.ToString(),
                    senderP = a[4].Value.ToString(),
                    timeP = a[5].Value.ToString()
                };

                MyDialog md = oldDialogs.Find(x => x.contentP == m.contentP && x.timeP == m.timeP);
                if (md == null)
                {
                    string sender = m.senderP.Replace(".", ",");
                    string receiver = m.receiverP.Replace(".", ",");
                    string s1 = "Dialog " + sender + "+" + receiver;
                    string s2 = "Dialog " + receiver + "+" + sender;
                    int temp = oldDialogs.FindIndex(x => x.dialogName == s1);
                    if (temp < 0)
                    {
                        temp = oldDialogs.FindIndex(x => x.dialogName == s2);
                        if (temp < 0)
                        {

                        }
                        else
                        {
                            oldDialogs[temp].lastMessage = m;
                            oldDialogs[temp].accessFlag = Convert.ToInt32(m.access[0]);
                            oldDialogs[temp].contentP = m.contentP;
                            oldDialogs[temp].messageFlag = Convert.ToInt32(m.flags[0]);
                            oldDialogs[temp].receiverP = m.receiverP;
                            oldDialogs[temp].senderP = m.senderP;
                            oldDialogs[temp].timeP = m.timeP;

                            DialogsController.instance.CreateTable();
                            DialogsController.instance.UpdateItem(oldDialogs[temp].dialogName, oldDialogs[temp].lastMessage);

                            /**/
                            oldDialogs = oldDialogs.OrderByDescending(x => x.accessFlag).ToList().OrderByDescending(x => Convert.ToDateTime(x.timeP)).ToList();
                            adapterOld = new OldDialogItemAdapter(oldDialogs, true);
                            oldDialogsList.Adapter = adapterOld;
                        }
                    }
                    else
                    {
                        oldDialogs[temp].lastMessage = m;
                        oldDialogs[temp].accessFlag = Convert.ToInt32(m.access[0]);
                        oldDialogs[temp].contentP = m.contentP;
                        oldDialogs[temp].messageFlag = Convert.ToInt32(m.flags[0]);
                        oldDialogs[temp].receiverP = m.receiverP;
                        oldDialogs[temp].senderP = m.senderP;
                        oldDialogs[temp].timeP = m.timeP;

                        DialogsController.instance.CreateTable();
                        DialogsController.instance.UpdateItem(oldDialogs[temp].dialogName, oldDialogs[temp].lastMessage);

                        /**/
                        oldDialogs = oldDialogs.OrderByDescending(x => x.accessFlag).ToList().OrderByDescending(x => Convert.ToDateTime(x.timeP)).ToList();
                        adapterOld = new OldDialogItemAdapter(oldDialogs, true);
                        oldDialogsList.Adapter = adapterOld;
                    }
                }
            }
            catch(Exception ex)
            {
                Utils.MessageBox("Сетевая ошибка!", this);
            }
            
        }
    }
}