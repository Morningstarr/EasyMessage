using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using EasyMessage.Adapters;
using EasyMessage.Controllers;
using EasyMessage.Entities;
using Firebase.Database;
using Newtonsoft.Json;
using Message = EasyMessage.Entities.Message;

namespace EasyMessage
{
    [Activity(Label = "Диалог", Theme = "@style/Theme.MaterialComponents.Light.DarkActionBar")]
    public class DialogActivity : AppCompatActivity, IValueEventListener
    {
        private List<Message> messageList;
        private RecyclerView recyclerList;
        private Button send;
        private EditText messageContent;
        private RecyclerViewAdapter adapter;
        private string dialog;
        private string receiver;
        private Message t;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.dialog);

            dialog = Intent.GetStringExtra("dialogName");
            receiver = Intent.GetStringExtra("receiver");
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#2196f3")));
            SupportActionBar.Title = receiver;

            if (FirebaseController.instance.app == null)
            {
                FirebaseController.instance.initFireBaseAuth();
            }
            //var r = FirebaseDatabase.Instance.GetReference("Dialog kirill_kovrik@mail,ru+kirill,kop,work@gmail,com");
            //.AddValueEventListener(this);
            var layoutManager = new LinearLayoutManager(this) { Orientation = LinearLayoutManager.Vertical };
            recyclerList = FindViewById<RecyclerView>(Resource.Id.reyclerview_message_list);
            recyclerList.SetLayoutManager(layoutManager);
            //recyclerList.HasFixedSize = true;


            await fill_list();
            adapter = new RecyclerViewAdapter(messageList);
            recyclerList.SetAdapter(adapter);
            recyclerList.ScrollToPosition(messageList.Count() - 1);

            FirebaseDatabase databaseInstance = FirebaseDatabase.GetInstance(FirebaseController.instance.app);
            var userNode = databaseInstance.GetReference("chats");

            DatabaseReference u = userNode.Child(dialog);
            u.AddValueEventListener(this);

            messageContent = FindViewById<EditText>(Resource.Id.edittext_chatbox);
            send = FindViewById<Button>(Resource.Id.send);
            List<MessageFlags> flags = new List<MessageFlags>();
            flags.Add(MessageFlags.NotEncoded);            

            send.Click += async delegate
            {
                t = new Message
                {
                    contentP = messageContent.Text,
                    senderP = AccountsController.mainAccP.emailP,
                    flags = flags,
                    receiverP = receiver,
                    timeP = DateTime.Now.ToString()
                };
                Task<bool> sendTask = MessagingController.instance.SendMessage(t, dialog, this);
                bool sent = await sendTask;
                if (sent)
                {
                    //Utils.MessageBox("ok", this);
                    //messageList.Add(t);
                    //adapter = new RecyclerViewAdapter(messageList);
                    //recyclerList.SetAdapter(adapter);
                    messageContent.Text = "";
                    //recyclerList.SmoothScrollToPosition(messageList.Count() - 1);
                    recyclerList.ScrollToPosition(messageList.Count() - 1);
                }
                else
                {
                    Utils.MessageBox("error", this);
                }
                
            };

            
            // Create your application here
        }

        private async Task<bool> fill_list()
        {
            Task<List<Message>> getMessagesTask = MessagingController.instance.GetAllMessages(dialog, this);
            messageList = await getMessagesTask;
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    if (t != null)
                    {
                        //DialogsController.instance.CreateTable();
                        //DialogsController.instance.SaveItem(new MyDialog { dialogName = dialog, lastMessage = t });
                        DialogsController.currDialP.lastMessage = t;
                    }
                    Finish();
                    return true;
                default:
                    return true;
            }
        }

        public void OnCancelled(DatabaseError error)
        {
            throw new NotImplementedException();
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            IEnumerable<DataSnapshot> items = snapshot.Children?.ToEnumerable<DataSnapshot>();
            List<DataSnapshot> t = items.ToList();
            var a = t.Last().Children.ToEnumerable<DataSnapshot>().ToList();
            var flag = a[1].Child("0").Value;
            List<MessageFlags> fls = new List<MessageFlags>();
            fls.Add((MessageFlags)Convert.ToInt32(flag.ToString()));
            Message m = new Message { contentP = a[0].Value.ToString(), flags = fls, receiverP =  a[2].Value.ToString(),
            senderP = a[3].Value.ToString(), timeP = a[4].Value.ToString() };

            if (messageList.Find(x=>x.timeP == m.timeP && x.contentP == m.contentP) == null)
            {
                if(messageList == null)
                {
                    messageList = new List<Message>();
                }
                messageList.Add(m);
                DialogsController.currDialP.lastMessage = m;
                adapter = new RecyclerViewAdapter(messageList);
                recyclerList.SetAdapter(adapter);
                recyclerList.ScrollToPosition(messageList.Count() - 1);
            }
        }

        /*private void LoadNewMessages()
        {
            throw new NotImplementedException();
        }*/
    }
}