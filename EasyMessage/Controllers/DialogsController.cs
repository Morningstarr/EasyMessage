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
using Message = EasyMessage.Entities.Message;

namespace EasyMessage.Controllers
{
    public class DialogsController
    {
        public static DialogsController instance = new DialogsController();
        SQLiteConnection connection;

        private static MyDialog currDial;
        public static MyDialog currDialP
        {
            get { return currDial; }
            set { currDial = value; }
        }
        private List<MyDialog> dialogsList = new List<MyDialog>();
        public List<MyDialog> dialogsListP
        {
            get { return dialogsList; }
            set { dialogsList = value; }
        }

        public void CreateTable()
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
            connection = new SQLiteConnection(dbPath);
            connection.CreateTable<MyDialog>();
        }

        public IEnumerable<MyDialog> GetItems()
        {
            dialogsList = connection.Table<MyDialog>().ToList();
            return dialogsList;
        }
        public MyDialog GetItem(int id)
        {
            return connection.Get<MyDialog>(id);
        }
        public int DeleteItem(int id)
        {
            return connection.Delete<MyDialog>(id);
        }
        public int SaveItem(MyDialog item, bool t)
        {

            return connection.Insert(item);

        }
        public int SaveItem(MyDialog item)
        {
            dialogsList = connection.Table<MyDialog>().ToList();
            if (item.Id != 0)
            {
                connection.Update(item);
                return item.Id;
            }
            else
            {
                if (dialogsList.Find(x => x.dialogName == item.dialogName) == null)
                {
                    return connection.Insert(item);
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}