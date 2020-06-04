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
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.dialog);
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

            FirebaseDatabase databaseInstance = FirebaseDatabase.GetInstance(FirebaseController.instance.app);
            var userNode = databaseInstance.GetReference("chats");
            DatabaseReference u = userNode.Child("Dialog kirill_kovrik@mail,ru+kirill,kop,work@gmail,com");
            u.AddValueEventListener(this);

            messageContent = FindViewById<EditText>(Resource.Id.edittext_chatbox);
            send = FindViewById<Button>(Resource.Id.send);
            List<MessageFlags> flags = new List<MessageFlags>();
            flags.Add(MessageFlags.NotEncoded);            

            send.Click += async delegate
            {
                Message t = new Message
                {
                    contentP = messageContent.Text,
                    senderP = AccountsController.mainAccP.emailP,
                    flags = flags,
                    receiverP = "kirill.kop.work@gmail.com",
                    timeP = DateTime.Now.ToString()
                };
                Task<bool> sendTask = MessagingController.instance.SendMessage(t, "Dialog kirill_kovrik@mail,ru+kirill,kop,work@gmail,com", this);
                bool sent = await sendTask;
                if (sent)
                {
                    //Utils.MessageBox("ok", this);
                    //messageList.Add(t);
                    //adapter = new RecyclerViewAdapter(messageList);
                    //recyclerList.SetAdapter(adapter);
                    send.Text = "";
                }
                else
                {
                    Utils.MessageBox("error", this);
                }
                
            };

            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#2196f3")));
            // Create your application here
        }

        private async Task<bool> fill_list()
        {
            Task<List<Message>> getMessagesTask = MessagingController.instance.GetAllMessages("Dialog kirill_kovrik@mail,ru+kirill,kop,work@gmail,com", this);
            messageList = await getMessagesTask;
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
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
                adapter = new RecyclerViewAdapter(messageList);
                recyclerList.SetAdapter(adapter);
            }
        }

        /*private void LoadNewMessages()
        {
            throw new NotImplementedException();
        }*/
    }
}