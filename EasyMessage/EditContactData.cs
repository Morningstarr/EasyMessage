using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using EasyMessage.Controllers;
using EasyMessage.Entities;
using Firebase.Database;

namespace EasyMessage
{
    [Activity(Label = "Изменить имя", Theme = "@style/Theme.MaterialComponents.Light.DarkActionBar")]
    public class EditContactData : AppCompatActivity, IOnCompleteListener
    {
        private EditText newName;
        private ProgressBar progress;
        private string oldName;
        //private int id;
        private Button okbutton;
        private bool flag;
        public void OnComplete(Android.Gms.Tasks.Task task)
        {
            //throw new NotImplementedException();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.edit_contact_data);

            //oldName = Intent.GetStringExtra("oldName");
            //id = Intent.GetIntExtra("id", 0);
            
            newName = FindViewById<EditText>(Resource.Id.newName);
            progress = FindViewById<ProgressBar>(Resource.Id.prg);

            okbutton = FindViewById<Button>(Resource.Id.okbutton);
            okbutton.Click += async delegate
            {
                progress.Visibility = ViewStates.Visible;
                Task<bool> taskChange = FirebaseController.instance.ChangeContactName(AccountsController.mainAccP.emailP, 
                    newName.Text, ContactsController.currContP.contactAddressP, this);
                flag = await taskChange;
                ContactsController.instance.CreateTable();
                Contact cont = ContactsController.instance.GetItem(ContactsController.currContP.Id);
                cont.contactNameP = newName.Text;
                ContactsController.instance.SaveItem(cont);
                ContactsController.currContP = cont;
                Utils.MessageBox("Успешно!", this);
                progress.Visibility = ViewStates.Invisible;
                Finish();
            };

            newName.Text = ContactsController.currContP.contactNameP;

            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#2196f3")));
            // Create your application here
        }

    }
}