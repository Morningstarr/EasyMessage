﻿using Firebase.Database;
using System;
using System.Linq;
using EasyMessage.Entities;
using Java.Util;
using System.Collections.Generic;
using Android.App;
using EasyMessage.Utilities;
using EasyMessage.Adapters;

namespace EasyMessage
{
    public  class UValueEventListener : Java.Lang.Object, IChildEventListener
    {
        EventHandler OnChange;
        string dialogName;
        Activity context;
        List<string> names = new List<string>();

        public UValueEventListener(EventHandler OnChange, Activity _context, string _dialogName)
        {
            this.OnChange = OnChange;
            //this.Username = Username;
            context = _context;
            dialogName = _dialogName;
        }

        public void OnCancelled(DatabaseError error)
        {
            //
        }

        public void OnChildAdded(DataSnapshot snapshot, string previousChildName)
        {
            IEnumerable<DataSnapshot> items = snapshot.Children?.ToEnumerable<DataSnapshot>();
            List<DataSnapshot> t = items.ToList();
            if(t.Count > 5)
            {
                var flag = t[2].Child("0").Value;
                List<MessageFlags> fls = new List<MessageFlags>();
                fls.Add((MessageFlags)Convert.ToInt32(flag.ToString()));

                if (fls[0] != MessageFlags.Key)
                {
                    var access = t[0].Child("0").Value;
                    List<AccessFlags> acs = new List<AccessFlags>();
                    acs.Add((AccessFlags)Convert.ToInt32(access.ToString()));
                    Message temp = new Message
                    {
                        contentP = t[1].Value.ToString(),
                        flags = fls,
                        receiverP = t[3].Value.ToString(),
                        senderP = t[4].Value.ToString(),
                        timeP = t[5].Value.ToString(),
                        access = acs,
                        dialogName = dialogName
                    };
                    MessagesController.instance.CreateTable();
                    if (MessagesController.instance.FindItem(temp) == null)
                    {
                        var mess = MessagesController.instance.GetItems().ToList();
                        if (mess.Count < 50)
                        {
                            MessagesController.instance.SaveItem(temp, true);
                        }
                        else
                        {
                            MessagesController.instance.DeleteItem(mess[0].Id);
                            MessagesController.instance.SaveItem(temp, true);
                        }
                    }
                }
            }
        }

        public void OnChildChanged(DataSnapshot snapshot, string previousChildName)
        {
            //Utils.MessageBox("childChanged", context);
            //throw new NotImplementedException();
        }

        public void OnChildMoved(DataSnapshot snapshot, string previousChildName)
        {
            //.MessageBox("childMoved", context);
            //throw new NotImplementedException();
        }

        public void OnChildRemoved(DataSnapshot snapshot)
        {
            //Utils.MessageBox("childRemoved", context);
            //throw new NotImplementedException();
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (OnChange != null)
            {
                IEnumerable<DataSnapshot> items = snapshot.Children?.ToEnumerable<DataSnapshot>();
                List<DataSnapshot> t = items.ToList();

            }

        }

    }
}
