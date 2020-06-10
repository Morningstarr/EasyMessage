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

namespace EasyMessage.Utilities
{
    public enum AccessFlags
    {
        None = 0,
        Read = 1,
        NotRead = 2,
        Special = 3
    }
}