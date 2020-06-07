using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using EasyMessage.Adapters;
using EasyMessage.Controllers;
using EasyMessage.Entities;
using EasyMessage.Utilities;
using Firebase.Database;
using Newtonsoft.Json;
using Message = EasyMessage.Entities.Message;

namespace EasyMessage
{
    [Activity(Label = "Диалог", Theme = "@style/Theme.MaterialComponents.Light.DarkActionBar")]
    public class DialogActivity : AppCompatActivity, IValueEventListener
    {
        private List<Message> messageList;
        private ProgressBar loadProgress;
        private RecyclerView recyclerList;
        private Button send;
        private EditText messageContent;
        private RecyclerViewAdapter adapter;
        private string dialog;
        private string receiver;
        private Message lastMessage;
        private int flag = 0;
        ConnectivityManager connectivityManager;
        NetworkInfo networkInfo;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.dialog);

                loadProgress = FindViewById<ProgressBar>(Resource.Id.loadMessProgress);
                messageContent = FindViewById<EditText>(Resource.Id.edittext_chatbox);
                send = FindViewById<Button>(Resource.Id.send);

                connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
                networkInfo = null;

                dialog = Intent.GetStringExtra("dialogName");
                receiver = Intent.GetStringExtra("receiver");
                flag = Intent.GetIntExtra("flag", 0);
                SupportActionBar.SetHomeButtonEnabled(true);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#2196f3")));
                SupportActionBar.Title = ContactsController.instance.ReturnContactName(receiver);

                if (FirebaseController.instance.app == null)
                {
                    FirebaseController.instance.initFireBaseAuth();
                }
                var layoutManager = new LinearLayoutManager(this) { Orientation = LinearLayoutManager.Vertical };
                recyclerList = FindViewById<RecyclerView>(Resource.Id.reyclerview_message_list);
                recyclerList.SetLayoutManager(layoutManager);

                MessagingController.instance.ReturnLastMessages(dialog, this);

                MessagesController.instance.CreateTable();
                messageList = MessagesController.instance.GetItems().ToList().FindAll(x=>x.dialogName == dialog);

                loadProgress.Visibility = ViewStates.Visible;
                bool result = await fill_list();
                loadProgress.Visibility = ViewStates.Invisible;
                adapter = new RecyclerViewAdapter(messageList);

                /*DialogsController.instance.CreateTable();
                var d = DialogsController.instance.GetItems().ToList().Find(x => x.dialogName == dialog);
                d.lastMessage = messageList.Last();
                DialogsController.instance.SaveItem(d);*/

                recyclerList.SetAdapter(adapter);
                recyclerList.ScrollToPosition(messageList.Count() - 1);

                networkInfo = connectivityManager.ActiveNetworkInfo;
                if (networkInfo != null && networkInfo.IsConnected == true)
                {
                    FirebaseDatabase databaseInstance = FirebaseDatabase.GetInstance(FirebaseController.instance.app);
                    var userNode = databaseInstance.GetReference("chats");
                    DatabaseReference u = userNode.Child(dialog);
                    u.AddValueEventListener(this);
                }

                send.Click += async delegate
                {
                    if (messageContent.Text != "" && messageContent.Text != " ")
                    {
                        List<MessageFlags> flags = new List<MessageFlags>();
                        flags.Add(MessageFlags.NotEncoded);

                        List<AccessFlags> acess = new List<AccessFlags>();
                        acess.Add(AccessFlags.NotRead);
                        lastMessage = new Message
                        {
                            contentP = messageContent.Text,
                            senderP = AccountsController.mainAccP.emailP,
                            flags = flags,
                            access = acess,
                            receiverP = receiver,
                            timeP = DateTime.Now.ToString()
                        };
                        Task<bool> sendTask = MessagingController.instance.SendMessage(lastMessage, dialog, this);
                        bool sent = await sendTask;
                        if (sent)
                        {
                            messageContent.Text = "";
                            recyclerList.ScrollToPosition(messageList.Count() - 1);
                        }
                        else
                        {
                            Utils.MessageBox("error", this);
                        }
                    }

                };
            }
            catch(Exception ex)
            {
                Utils.MessageBox("Произошла сетевая ошибка! Проверьте подключение к интернету и повторите позже.", this);
            }

            
            // Create your application here
        }

        private async Task<bool> fill_list()
        {
            networkInfo = connectivityManager.ActiveNetworkInfo;
            if (networkInfo != null && networkInfo.IsConnected == true)
            {
                loadProgress.Visibility = ViewStates.Visible;
                Task<List<Message>> getMessagesTask = MessagingController.instance.GetAllMessages(dialog, this);
                messageList = await getMessagesTask;
                foreach (var mes in messageList)
                {
                    if (mes.senderP != AccountsController.mainAccP.emailP && mes.access != null)
                    {
                        mes.access[0] = AccessFlags.Read;
                    }
                }
                loadProgress.Visibility = ViewStates.Invisible;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    flag = -1;
                    Finish();
                    return true;
                default:
                    return true;
            }
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            flag = -1;
        }

        public void OnCancelled(DatabaseError error)
        {
            //throw new NotImplementedException();
        }

        public async void OnDataChange(DataSnapshot snapshot)
        {
            try
            {
                IEnumerable<DataSnapshot> items = snapshot.Children?.ToEnumerable<DataSnapshot>();
                List<DataSnapshot> t = items.ToList();

                if (flag == 0)
                {
                    var a = t.Last().Children.ToEnumerable<DataSnapshot>().ToList();
                    var flag = a[2].Child("0").Value;
                    List<MessageFlags> fls = new List<MessageFlags>();
                    fls.Add((MessageFlags)Convert.ToInt32(flag.ToString()));

                    var access = a[0].Child("0").Value;
                    List<AccessFlags> acs = new List<AccessFlags>();
                    acs.Add((AccessFlags)Convert.ToInt32(access.ToString()));

                    if (a[4].Value.ToString() != AccountsController.mainAccP.emailP && Convert.ToInt32(access.ToString()) == 2)
                    {
                        await MessagingController.instance.UpdateFlag(t.Last().Key, dialog, this);
                        DialogsController.instance.CreateTable();
                        DialogsController.instance.UpdateItem(dialog);
                        DialogsController.currDialP = new MyDialog();
                    }

                    Message m = new Message
                    {
                        contentP = a[1].Value.ToString(),
                        flags = fls,
                        receiverP = a[3].Value.ToString(),
                        senderP = a[4].Value.ToString(),
                        timeP = a[5].Value.ToString(),
                        access = acs
                    };

                    if (messageList.Find(x => x.timeP == m.timeP && x.contentP == m.contentP) == null)
                    {
                        if (messageList == null)
                        {
                            messageList = new List<Message>();
                        }
                        messageList.Add(m);
                        DialogsController.currDialP = new MyDialog();
                        /*DialogsController.currDialP = new MyDialog
                        {
                            dialogName = dialog,
                            lastMessage = m,
                            contentP = m.contentP,
                            senderP = m.senderP,
                            receiverP = m.receiverP,
                            timeP = m.timeP,
                            accessFlag = Convert.ToInt32(m.access[0]),
                            messageFlag = Convert.ToInt32(m.flags[0])
                        };*/

                        adapter = new RecyclerViewAdapter(messageList);
                        recyclerList.SetAdapter(adapter);
                        recyclerList.ScrollToPosition(messageList.Count() - 1);
                    }
                }
                else
                {
                    if (flag == 1)
                    {
                        List<DataSnapshot> results = new List<DataSnapshot>();
                        List<DataSnapshot> temp = t.FindAll(x => x.Children.ToEnumerable<DataSnapshot>().ToList().Count > 5);
                        foreach (var i in temp)
                        {
                            var a = i.Children.ToEnumerable<DataSnapshot>().ToList();
                            if (a.Find(x => x.Key == "senderP"
                                && x.Value.ToString() != AccountsController.mainAccP.emailP) != null &&
                                a.Find(x => x.Key == "access" && Convert.ToInt32(x.Child("0").Value.ToString()) == 2) != null)
                            {
                                results.Add(i);
                            }
                        }
                        foreach (var k in results)
                        {
                            //flag = -1;
                            await MessagingController.instance.UpdateFlag(k.Key, dialog, this);
                            DialogsController.instance.CreateTable();
                            DialogsController.instance.UpdateItem(dialog);
                            DialogsController.currDialP = new MyDialog();
                        }
                        flag = 0;
                    }
                }
            }
            catch(Exception ex)
            {
                Utils.MessageBox(ex.Message, this);
            }
        }

    }
}