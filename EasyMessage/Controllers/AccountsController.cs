using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using EasyMessage.Entities;
using SQLite;

namespace EasyMessage.Controllers
{
    public class AccountsController
    {
        public static AccountsController instance = new AccountsController();
        SQLiteConnection connection;

        private static Account mainAcc;
        public static Account mainAccP
        {
            get { return mainAcc; }
            set { mainAcc = value; }
        }
        private List<Account> deviceAccs = new List<Account>();
        public List<Account> deviceAccsP
        {
            get { return deviceAccs; }
            set { deviceAccs = value; }
        }

        public void CreateTable()
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
            connection = new SQLiteConnection(dbPath);
            connection.CreateTable<Account>();
        }

        public IEnumerable<Account> GetItems()
        {
            deviceAccs = connection.Table<Account>().ToList();
            return deviceAccs;
        }
        public Account GetItem(int id)
        {
            return connection.Get<Account>(id);
        }
        public int DeleteItem(int id)
        {
            return connection.Delete<Account>(id);
        }
        public int SaveItem(Account item)
        {
            if (item.Id != 0)
            {
                connection.Update(item);
                return item.Id;
            }
            else
            {
                return connection.Insert(item);
            }
        }
    }
}
