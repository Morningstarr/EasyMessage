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

namespace EasyMessage.Entities
{
    class ItemTemplate
    {
        public string s_value { get; set; }
        public string s_description { get; set; }

        public static ItemTemplate convertToItem(string description, Account a, int index)
        {
            ItemTemplate i = new ItemTemplate();
            i.s_description = description;
            if (index == 1) {
                i.s_value = a.loginP;
            }
            if (index == 2)
            {
                i.s_value = a.emailP;
            }
            if (index == 3)
            {
                i.s_value = a.passwordP;
            }

            return i;
        }
    }
}