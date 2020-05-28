using System;

namespace EasyMessage
{
    public class UserEventArgs : EventArgs
    {
        public UserEventArgs(bool value)
        {
            this.value = value;
        }

        public bool value { get; set; }
    }
}