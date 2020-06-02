﻿using System;
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
    public class ContactsController
    {
        public static ContactsController instance = new ContactsController();
        SQLiteConnection connection;

        //private static C mainAcc;
        /*public static Con mainAccP
        {
            get { return mainAcc; }
            set { mainAcc = value; }
        }*/
        private List<Contact> contactList = new List<Contact>();
        public List<Contact> contactListP
        {
            get { return contactList; }
            set { contactList = value; }
        }

        public void CreateTable()
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
            connection = new SQLiteConnection(dbPath);
            connection.CreateTable<Contact>();
        }

        public IEnumerable<Contact> GetItems()
        {
            contactList = connection.Table<Contact>().ToList();
            return contactList;
        }
        public Contact GetItem(int id)
        {
            return connection.Get<Contact>(id);
        }
        public int DeleteItem(int id)
        {
            return connection.Delete<Contact>(id);
        }
        public int SaveItem(Contact item)
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