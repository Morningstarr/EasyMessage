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
using Android.Views;
using Android.Widget;
using EasyMessage.Controllers;
using EasyMessage.Entities;
using EasyMessage.Utilities;
using Newtonsoft.Json;

namespace EasyMessage
{
    [Activity(Label = "Подробности", Theme = "@style/Theme.MaterialComponents.Light.DarkActionBar")]
    public class ContactDetails : AppCompatActivity
    {
        private Button delete;
        private Button openDialog;
        private ProgressBar deletePrg;
        private Button changeLogin;
        private TextView cName;
        private TextView cMail;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Contact c = ContactsController.currContP; /*ContactsController.instance.GetItem(*/;
            SetContentView(Resource.Layout.contact_details);

            cName = FindViewById<TextView>(Resource.Id.cName);
            cMail = FindViewById<TextView>(Resource.Id.cMail);
            changeLogin = FindViewById<Button>(Resource.Id.changeLogin);
            delete = FindViewById<Button>(Resource.Id.deleteContact);
            deletePrg = FindViewById<ProgressBar>(Resource.Id.deleteProgress);
            openDialog = FindViewById<Button>(Resource.Id.openDialog);

            delete.Click += async delegate
            {
                disable_controls(false);
                deletePrg.Visibility = ViewStates.Visible;
                Task<bool> deleteTask = FirebaseController.instance.DeleteContact(AccountsController.mainAccP.emailP, ContactsController.currContP.contactAddressP,
                    this);
                bool result = await deleteTask;
                ContactsController.instance.CreateTable();
                List<AccessFlags> acs = new List<AccessFlags>();
                acs.Add(AccessFlags.Deleted);
                List<MessageFlags> fls = new List<MessageFlags>();
                fls.Add(MessageFlags.NotEncoded);
                string dial = ContactsController.currContP.dialogNameP;
                if (dial == null)
                {
                    DialogsController.instance.CreateTable();
                    var ds = DialogsController.instance.GetItems().ToList();
                    var changedMail = ContactsController.currContP.contactAddressP.Replace(".", ",");
                    dial = ds.Find(x => x.dialogName.Contains(changedMail)).dialogName;
                }
                await MessagingController.instance.SendMessage(new Entities.Message
                {
                    contentP = "Contact data deleted",
                    senderP = AccountsController.mainAccP.emailP,
                    access = acs,
                    flags = fls,
                    receiverP = ContactsController.currContP.contactAddressP,
                    timeP = DateTime.Now.ToString()                    
                },  dial, this);
                int a = ContactsController.instance.DeleteItem(ContactsController.currContP.Id);
                ContactsController.currContP.deletedP = true;
                deletePrg.Visibility = ViewStates.Invisible;
                disable_controls(true);
                Finish();
            };

            changeLogin.Click += delegate
            {
                Intent i = new Intent(this, typeof(EditContactData));
                i.SetFlags(ActivityFlags.NoAnimation);
                StartActivity(i);
            };

            openDialog.Click += delegate
            {
                Intent i = new Intent(this, typeof(DialogActivity));
                i.SetFlags(ActivityFlags.NoAnimation);
                string dial = ContactsController.currContP.dialogNameP;
                MyDialog temp = null;
                if (dial != null)
                {
                    temp = DialogsController.instance.FindDialog(dial);
                }
                else
                {
                    DialogsController.instance.CreateTable();
                    var ds = DialogsController.instance.GetItems().ToList();
                    var changedMail = ContactsController.currContP.contactAddressP.Replace(".", ",");
                    temp = ds.Find(x => x.dialogName.Contains(changedMail));
                }
                DialogsController.currDialP = temp;
                i.PutExtra("dialogName", temp.dialogName);
                i.PutExtra("receiver",
                    temp.receiverP == AccountsController.mainAccP.emailP ? temp.senderP : temp.receiverP); 
                StartActivity(i);
            };

            cName.Text = c.contactNameP;
            cMail.Text = c.contactAddressP;

            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#2196f3")));
            // Create your application here
        }

        public void disable_controls(bool fl)
        {
            delete.Enabled = fl;
            openDialog.Enabled = fl;
            changeLogin.Enabled = fl;
        }

        protected override void OnResume()
        {
            Contact c = ContactsController.currContP;
            cName = FindViewById<TextView>(Resource.Id.cName);
            cMail = FindViewById<TextView>(Resource.Id.cMail);
            cName.Text = c.contactNameP;
            cMail.Text = c.contactAddressP;
            base.OnResume();
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
    }
}