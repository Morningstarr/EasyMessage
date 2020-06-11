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
using SQLite;
using Message = EasyMessage.Entities.Message;

namespace EasyMessage.Adapters
{
    public class MessagesController
    {
        public static MessagesController instance = new MessagesController();
        SQLiteConnection connection;

        private static Message currMess;
        public static Message currMessP
        {
            get { return currMess; }
            set { currMess = value; }
        }
        private List<Message> messagesList = new List<Message>();
        public List<Message> messagesListP
        {
            get { return messagesList; }
            set { messagesList = value; }
        }

        public void CreateTable()
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
            connection = new SQLiteConnection(dbPath);
            connection.CreateTable<Message>();
        }

        public IEnumerable<Message> GetItems()
        {
            messagesList = connection.Table<Message>().ToList();
            return messagesList;
        }
        public Message GetItem(int id)
        {
            return connection.Get<Message>(id);
        }
        public int DeleteItem(int id)
        {
            return connection.Delete<Message>(id);
        }
        public int SaveItem(Message item, bool t)
        {

            return connection.Insert(item);

        }

        public Message FindItem(Message item)
        {
            messagesList = connection.Table<Message>().ToList();
            return messagesList.Find(x => x.contentP == item.contentP && x.timeP == item.timeP);
        }
        public Message FindItemI(Message item)
        {
            messagesList = connection.Table<Message>().ToList();
            return messagesList.Find(x => x.contentP == item.contentP && x.timeP == item.timeP);
        }
        public int SaveItem(Message item)
        {
            messagesList = connection.Table<Message>().ToList();
            if (item.Id != 0)
            {
                connection.Update(item);
                return item.Id;
            }
            else
            {
                if (messagesList.Find(x => x.dialogName == item.dialogName && x.contentP == item.contentP && x.timeP == item.timeP) == null)
                {
                    return connection.Insert(item);
                }
                else
                {
                    return 0;
                }
            }
        }

        public int SaveItem(Message item, int i)
        {
            if (messagesList.Find(x => x.contentP == item.contentP && x.timeP == item.timeP) == null)
            {
                return connection.Insert(item);
            }
            else
            {
                return /*connection.Update(item)*/ 0;
            }

        }
    }
}