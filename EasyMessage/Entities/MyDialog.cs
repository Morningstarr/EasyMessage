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

        public string senderP { get; set; }
        public string receiverP { get; set; }
        public string contentP { get; set; }
        public string timeP { get; set; }
        public int messageFlag { get; set; }
        public int accessFlag { get; set; }

        public MyDialog()
        {

        }


        public MyDialog(string _dialogName, Message _lastMessage)
        {
            dialogName = _dialogName;
            lastMessage = _lastMessage;
            senderP = _lastMessage.senderP;
            receiverP = _lastMessage.receiverP;
            contentP = _lastMessage.contentP;
            timeP = _lastMessage.timeP;
            messageFlag = Convert.ToInt32(_lastMessage.flags[0]);
            accessFlag = Convert.ToInt32(_lastMessage.access[0]);
        }

        public MyDialog FillFields(/*string _dialogName*/)
        {
            accessFlag = Convert.ToInt32(lastMessage.access[0]);
            //dialogName = _dialogName;
            lastMessage = lastMessage;
            senderP = lastMessage.senderP;
            receiverP = lastMessage.receiverP;
            contentP = lastMessage.contentP;
            timeP = lastMessage.timeP;
            messageFlag = Convert.ToInt32(lastMessage.flags[0]);

            return this;
        }

        //public string message { get; set; } 
            //= lastMessage.receiverP;
    }
}