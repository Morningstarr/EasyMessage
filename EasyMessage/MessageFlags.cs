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

namespace EasyMessage
{
    public enum MessageFlags
    {
        Encoded = 5,
        NotEncoded = 1,
        Request = 2,
        Response = 3,
        Denied = 4
    }
}