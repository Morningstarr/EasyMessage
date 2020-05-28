using System;

namespace EasyMessage
{
    public class UEventArgs : EventArgs
    {
        public UEventArgs(bool value)
        {
            this.value = value;
        }

        public bool value { get; set; }
    }
}