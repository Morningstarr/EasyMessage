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
    [Table ("Dialogs")]
    public class MyDialog
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string dialogName { get; set; }
        [Ignore]
        public Message lastMessage { get; set; }

        //public string message { get; set; } 
            //= lastMessage.receiverP;
    }
}