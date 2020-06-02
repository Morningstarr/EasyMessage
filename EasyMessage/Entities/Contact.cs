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
using SQLite;

namespace EasyMessage.Entities
{
    [Table("Contacts")]
    public class Contact
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        private string contactName;
        public string contactNameP
        {
            get { return contactName; }
            set { contactName = value; }
        }
        private string contactAddress;
        public string contactAddressP
        {
            get { return contactAddress; }
            set { contactAddress = value; }
        }
        private string dialogName;
        public string dialogNameP
        {
            get { return dialogName; }
            set { dialogName = value; }
        }
        private string picturePath;
        public string picturePathP
        {
            get { return picturePath; }
            set { picturePath = value; }
        }
    }
}