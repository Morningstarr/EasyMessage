using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Database;
using Newtonsoft.Json;
using EasyMessage.Entities;
using Message = EasyMessage.Entities.Message;
//using FirebaseQuery = Firebase.Database.Query;

namespace EasyMessage.Controllers
{
    public class MessagingController
    {
        public static MessagingController instance = new MessagingController();
        public FirebaseClient client = null;
        //private FirebaseDatabase database;
        //public 

        /*public async Task<bool> SendKey(string dialogName, Activity context)
        {
            if (FirebaseController.instance.app == null)
            {
                FirebaseController.instance.initFireBaseAuth();
            }
            try
            {
                client = new Firebase.Database.FirebaseClient("https://easymessage-1fa08.firebaseio.com/chats/");

                var p = await client.Child(dialogName).PostAsync(JsonConvert.SerializeObject(new Message { contentP = AccountsController.mainAccP.openKeyRsaP,
                }));
                if (p != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Newtonsoft.Json.JsonReaderException exc)
            {
                return false;
            }
            catch (Exception ex)
            {
                Utils.MessageBox(ex.Message, context);
                return false;
            }
        }*/

        public void ReturnLastMessages(string dialogName, Activity context)
        {
            List<Message> results = new List<Message>();
            if (FirebaseController.instance.app == null)
            {
                FirebaseController.instance.initFireBaseAuth();
            }
            try
            {
                var referenceChats = FirebaseDatabase.Instance.GetReference("chats");
                Query query = referenceChats.Child(dialogName);
                var userListener = new UValueEventListener((sender, e) =>
                {
                    bool result = (e as UEventArgs).value;
                    if (result)
                    {

                    }
                }, context, dialogName);
                query.AddChildEventListener(userListener);
            }
            catch (Exception ex)
            {
                Utils.MessageBox(ex.Message, context);
            }
        }

        public async Task<bool> UpdateFlag(string key, string dialogName, Activity context)
        {
            if (FirebaseController.instance.app == null)
            {
                FirebaseController.instance.initFireBaseAuth();
            }
            try
            {
                //client = new Firebase.Database.FirebaseClient("https://easymessage-1fa08.firebaseio.com/chats/" + dialogName + "/");
                FirebaseDatabase databaseInstance = FirebaseDatabase.GetInstance(FirebaseController.instance.app);
                var userNode = databaseInstance.GetReference("chats");
                var dialogNode = userNode.Child(dialogName);
                await dialogNode.Child(key).Child("access").Child("0").SetValueAsync(1);
                return true;
            }
            catch (Newtonsoft.Json.JsonReaderException exc)
            {
                return false;
            }
            catch (Exception ex)
            {
                Utils.MessageBox(ex.Message, context);
                return false;
            }
        }

        public async Task<bool> SendMessage(Message m, string dialogName, Activity context)
        {
            if (FirebaseController.instance.app == null)
            {
                FirebaseController.instance.initFireBaseAuth();
            }
            try
            {
                client = new Firebase.Database.FirebaseClient("https://easymessage-1fa08.firebaseio.com/chats/");

                var p = await client.Child(dialogName).PostAsync(JsonConvert.SerializeObject(m));
                if (p != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Newtonsoft.Json.JsonReaderException exc)
            {
                return false;
            }
            catch (Exception ex)
            {
                Utils.MessageBox(ex.Message, context);
                return false;
            }
        }

        public async Task<List<Message>> GetAllMessages(string dialogName, Activity context)
        {
            List<Message> messageList = new List<Message>();
            if (FirebaseController.instance.app == null)
            {
                FirebaseController.instance.initFireBaseAuth();
            }
            try
            {
                client = new Firebase.Database.FirebaseClient("https://easymessage-1fa08.firebaseio.com/chats/");
                var p = await client.Child(dialogName).OnceAsync<Message>();
                var enumerator = p.GetEnumerator();
                enumerator.MoveNext();
                while (enumerator.Current != null)
                {
                    Message temp = enumerator.Current.Object;
                    if (temp.flags[0] != MessageFlags.Key)
                    {
                        messageList.Add(temp);
                    }
                    enumerator.MoveNext();
                }
                return messageList;
            }
            catch (Newtonsoft.Json.JsonReaderException exc)
            {
                return messageList;
            }
            catch (Exception ex)
            {
                Utils.MessageBox(ex.Message, context);
                return messageList;
            }
        }
    }
}