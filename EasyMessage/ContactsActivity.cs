using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Database;

namespace EasyMessage
{
    [Activity(Label = "Контакты")]
    public class ContactsActivity : AppCompatActivity, IValueEventListener
    {
        private Button send;
        private EditText receiver;
        private EditText message;
        private List<Message> messages;
        private Android.Support.V7.Widget.Toolbar tb;

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
            SetContentView(Resource.Layout.contacts);

            //receiver = FindViewById<EditText>(Resource.Id.receiver);
            //message = FindViewById<EditText>(Resource.Id.message);
            //send = FindViewById<Button>(Resource.Id.button1);
            tb = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.tooolbarCommon);

            //ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, FindViewById<DrawerLayout>(Resource.Id.drawerLayout1), Resource.String.openDrawer, Resource.String.closeDrawer);
            //toggle.SyncState();

            SetSupportActionBar(tb);
            Android.Support.V7.App.ActionBar abar = SupportActionBar;

            abar.SetHomeButtonEnabled(true);
            abar.SetDisplayShowTitleEnabled(true);
            abar.SetHomeAsUpIndicator(Resource.Drawable.menu);
            abar.SetDisplayHomeAsUpEnabled(true);

            /*send.Click += delegate
            {
                sendMessage(receiver.Text, message.Text, "kirill");
            };*/
            // Create your application here
        }

        private void sendMessage(string rec, string mes, string sendr)
        {
            //FirebaseDatabase.Instance.GetReference("chat").AddValueEventListener(this);
            //var items = await  
            FirebaseController.instance.SendMessage("7", "8", "9", this);
        }
    }
}