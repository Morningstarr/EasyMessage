using Firebase.Database;
using System;

namespace EasyMessage
{
    public  class UserValueEventListener : Java.Lang.Object, IValueEventListener
    {
        EventHandler OnChange;
        string Username;

        public UserValueEventListener(EventHandler OnChange, string Username)
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
                OnChange.Invoke(this, new UserEventArgs(true));

            }
        }
    }
}
