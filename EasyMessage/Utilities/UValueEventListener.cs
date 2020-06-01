using Firebase.Database;
using System;
using System.Linq;
using EasyMessage.Entities;
using Java.Util;
using System.Collections.Generic;

namespace EasyMessage
{
    public  class UValueEventListener : Java.Lang.Object, IValueEventListener
    {
        EventHandler OnChange;
        string Username;
        List<string> names = new List<string>();

        public UValueEventListener(EventHandler OnChange, string Username)
        {
            this.OnChange = OnChange;
            this.Username = Username;
        }

        public void OnCancelled(DatabaseError error)
        {
            //
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            string u = Username.Replace(".", ",");
            if (OnChange != null)
            {
                IEnumerable<DataSnapshot> items = snapshot.Children?.ToEnumerable<DataSnapshot>();
                List<DataSnapshot> t = items.ToList();

                foreach (var item in t)
                {
                    if (item.Key.Contains(u) && item.ChildrenCount == 1)
                    {
                        //IEnumerable<DataSnapshot> items2 = item.Children.ToEnumerable<DataSnapshot>();
                        //List<DataSnapshot> t2 = items2.ToList();

                        if (item.Children.ToEnumerable<DataSnapshot>().ToList()[0].Child("contentP").GetValue(true).ToString().Contains("хочет добавить вас"))
                        {
                            IAsyncResult a = OnChange.BeginInvoke(this, new UEventArgs(true, item.Key), new AsyncCallback(AsyncCompleted), item.Key);
                            
                            //OnChange.EndInvoke(a);
                        }
                    }
                }
            }

        }

        private void AsyncCompleted(IAsyncResult ar)
        {
            if(Utils.dialogs == null)
            {
                Utils.dialogs = new List<string>();
            }
            Utils.dialogs.Add((string)ar.AsyncState);
        }

    }
}
