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
using EasyMessage.Utilities;
using SQLite;

namespace EasyMessage.Entities
{
    
    public class Message
    {
        //public string receiver;
        //[Json ]
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string dialogName { get; set; }
        public string receiverP { get; set; }

        //public string sender;
        public string senderP { get; set; }
        /*{
            get { return sender; }
            set { sender = value; }
        }*/
        //public string content;
        public string contentP { get; set; }
        /*public string contentP
        {
            get { return content; }
            set { content = value; }
        }*/
        //public string time;
        public string timeP { get; set; }
        public int flagsP { get; set; }
        public int accessP { get; set; }
        /*public string timeP
        {
            get { return time; }
            set { time = value; }
        }*/
        [Ignore]
        public List<MessageFlags> flags { get; set; }
        [Ignore]
        public List<AccessFlags> access { get; set; }

        public Message() { }
        public Message(string rec, string send, string cont, List<MessageFlags> fl, List<AccessFlags> ac)
        {
            receiverP = rec;
            senderP = send;
            contentP = cont;
            timeP = DateTime.Now.ToString();
            flags = fl;
            access = ac;
        }

        public static string convertToJsonString(Message s)
        {
            return "{'JSON': {\"sender\": \"" + s.senderP + "\",\"receiver\" :\"" + s.receiverP + "\",\"content\" :\"" + s.contentP + "\", \"time\" : \"" + s.timeP +  "\" }}";
        }

        public static Java.Util.HashMap HashModel(Message t)
        {
            var properties = t.GetType().GetProperties();
            Java.Util.HashMap hashset = new Java.Util.HashMap();
            System.Collections.IDictionary hDict = new Dictionary<string, object>();
            foreach (var item in properties)
            {
                hDict.Add(item.Name, item.GetValue(t, null));
            }
            hashset.PutAll(hDict);
            return hashset;
        }
    }
}