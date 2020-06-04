using System;
using System.Collections.Generic;

namespace EasyMessage
{
    public class UEventArgs : EventArgs
    {
        public UEventArgs(bool value, string dialogNames)
        {
            this.value = value;
            this.dialogNames.Add(dialogNames);
        }

        public bool value { get; set; }
        public List<string> dialogNames = new List<string>();
        //dialogNames { get; set; }
    }
}