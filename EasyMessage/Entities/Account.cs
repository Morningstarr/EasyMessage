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
using SQLite;

namespace EasyMessage.Entities
{
    [Table("Accounts")]
    public class Account
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        private string login;
        public string loginP
        {
            get { return login; }
            set { login = value; }
        }
        private string email;
        public string emailP
        {
            get { return email; }
            set { email = value; }
        }
        private string password;
        public string passwordP
        {
            get { return password; }
            set { password = value; }
        }
        private bool isMain;
        public bool isMainP
        {
            get { return isMain; }
            set { isMain = value; }
        }
        private string openKeyRsa;
        public string openKeyRsaP
        {
            get { return openKeyRsa; }
            set { openKeyRsa = value; }
        }
        private string privateKeyRsa;
        public string privateKeyRsaP
        {
            get { return privateKeyRsa; }
            set { privateKeyRsa = value; }
        }
        private string openKeyTdes;
        public string openKeyTdesP
        {
            get { return openKeyTdes; }
            set { openKeyTdes = value; }
        }

    }
}