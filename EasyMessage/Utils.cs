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
    public static class Utils
    {
        public static void MessageBox(string MyMessage, Context c)
        {
            AlertDialog.Builder builder;
            builder = new AlertDialog.Builder(c);
            builder.SetTitle("Warning");
            builder.SetMessage(MyMessage);
            builder.SetCancelable(false);
            builder.SetPositiveButton("OK", delegate { });
            Dialog dialog = builder.Create();
            dialog.Show();
            return;
        }
    }
}