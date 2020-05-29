using Firebase.Database;
using System;

namespace EasyMessage
{
    public  class UValueEventListener : Java.Lang.Object, IValueEventListener
    {
        EventHandler OnChange;
        string Username;

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
            //throw new Exception("here.");
            if (OnChange != null && snapshot.HasChild(Username))
            {
                OnChange.Invoke(this, new UEventArgs(true));

            }
        }
    }
}
