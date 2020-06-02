using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Gms.Tasks;
using EasyMessage.Controllers;
using EasyMessage.Entities;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Message = EasyMessage.Entities.Message;

namespace EasyMessage
{
    public class FirebaseController
    {
        public static FirebaseController instance = new FirebaseController();

        public FirebaseApp app;
        private FirebaseAuth auth;
        private List<string> dialogNames = new List<string>();
        
        //Firebase.Database database = 
        //private FirebaseDatabase database;
        private FirebaseClient client;

        public void initFireBaseAuth()
        {
            var options = new Firebase.FirebaseOptions.Builder()
                .SetApplicationId("1:323956276016:android:fcf09b75b366f4fa50a6f5")
                .SetApiKey("AIzaSyAOvDOj-PKpxFZcDgMO7uI4rxrP3i2GakM")
                .SetDatabaseUrl("https://easymessage-1fa08.firebaseio.com/")
                .Build();

            if (app == null)
            {
                app = FirebaseApp.InitializeApp(Application.Context, options);
            }

            auth = FirebaseAuth.GetInstance(app);

            if(auth == null)
            {
                throw new Exception("Authentication Error!");
            }
        }

        public async Task<string> LoginUser(string eMail, string password)
        {
            IAuthResult user = await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(eMail, password);
            var token = await user.User.GetIdTokenAsync(false);
            return token.Token;
        }

        public async Task<string> Register(string eMail, string password, string name)
        {
            IAuthResult user = await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(eMail, password);
            UserProfileChangeRequest.Builder profileUpdates = new UserProfileChangeRequest.Builder();
            profileUpdates.SetDisplayName(name);
            UserProfileChangeRequest updates = profileUpdates.Build();
            await user.User.UpdateProfileAsync(updates);
            var token = await user.User.GetIdTokenAsync(false);
            return token.Token;
        }

        public async Task<string> SendDialogRequest(string contactAddressP)
        {
            string userlogin = contactAddressP.Replace(".", ",");
            string mylogin = AccountsController.mainAccP.emailP.Replace(".", ",");
            string dialogName = "Dialog " + userlogin + "+" + mylogin;
            List<MessageFlags> flags = new List<MessageFlags>();
            flags.Add(MessageFlags.Request);
            if (app == null)
            {
                initFireBaseAuth();
            }

            FirebaseDatabase databaseInstance = FirebaseDatabase.GetInstance(app);
            DatabaseReference userNode = databaseInstance.GetReference(dialogName);
            client = new Firebase.Database.FirebaseClient("https://easymessage-1fa08.firebaseio.com/chats/");
            var messages3 = await client.Child(dialogName).PostAsync(JsonConvert.SerializeObject(
                new Message(contactAddressP, AccountsController.mainAccP.emailP, "Пользователь " + AccountsController.mainAccP.emailP + 
                " хочет добавить вас в список контактов", flags)));

            /*string json = "{'JSON': { \"" + dialogName + "\" : { \"contentP\" : \"Пользователь " + AccountsController.mainAccP.emailP + " " +
                "хочет добавить вас в список контактов\",  \"receiverP\" : \"" + contactAddressP +"\", \"senderP\" : \"" + 
                AccountsController.mainAccP.emailP + "\", \"timeP\" : \"" + DateTime.Now.ToString() + "\"}}}";*/

            return dialogName;
        }

        public async Task<bool> IsDialogExists(string v1, string v2)
        {
            if (app == null)
            {
                initFireBaseAuth();
            }

            client = new Firebase.Database.FirebaseClient("https://easymessage-1fa08.firebaseio.com/");

            var p = await client.Child("chats").OnceAsync<object>();
            var d = p.GetEnumerator();
            d.MoveNext();

            while (d.Current != null)
            {
                if (d.Current.Key.Contains(v1) || d.Current.Key.Contains(v2))
                {
                    return true;
                }
                d.MoveNext();
            }
            return false;
        }

        public async Task<bool> SendDialogResponse(string dialogName, string receiverAddress)
        {
            if (app == null)
            {
                initFireBaseAuth();
            }
            client = new Firebase.Database.FirebaseClient("https://easymessage-1fa08.firebaseio.com/chats/");
            List<MessageFlags> flags = new List<MessageFlags>();
            flags.Add(MessageFlags.Response);
            var messages3 = await client.Child(dialogName).PostAsync(JsonConvert.SerializeObject(
                new Message(receiverAddress, AccountsController.mainAccP.emailP, "Пользователь " + AccountsController.mainAccP.emailP +
                " принял Ваш запрос", flags)));

            if (messages3.Key != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public async Task<bool> SendDialogDenial(string dialogName, string senderP)
        {
            if (app == null)
            {
                initFireBaseAuth();
            }
            client = new Firebase.Database.FirebaseClient("https://easymessage-1fa08.firebaseio.com/chats/");
            List<MessageFlags> flags = new List<MessageFlags>();
            flags.Add(MessageFlags.Denied);
            var messages3 = await client.Child(dialogName).PostAsync(JsonConvert.SerializeObject(
                new Message(senderP, AccountsController.mainAccP.emailP, "Пользователь " + AccountsController.mainAccP.emailP +
                " отклонил Ваш запрос", flags)));

            if (messages3.Key != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ResetPassword(string eMail, IOnCompleteListener c)
        {
            FirebaseAuth.Instance.SendPasswordResetEmail(eMail).AddOnCompleteListener(c);
        }

        public void ResetEmail(string newm, IOnCompleteListener c)
        {
            FirebaseUser user = FirebaseAuth.Instance.CurrentUser;
            user.UpdateEmail(newm).AddOnCompleteListener(c);
        }

        public void ChangePass(string newpass, IOnCompleteListener c)
        {
            FirebaseUser user = FirebaseAuth.Instance.CurrentUser;
            user.UpdatePassword(newpass).AddOnCompleteListener(c);
        }

        public void LogOut()
        {
            FirebaseAuth.Instance.SignOut();
        }

        public void DeleteUser(IOnCompleteListener c)
        {
            FirebaseUser user = FirebaseAuth.Instance.CurrentUser;
            user.Delete().AddOnCompleteListener(c);
        }

        public FirebaseUser GetCurrentUser()
        {
            return FirebaseAuth.Instance.CurrentUser;
        } 

        public void ChangeLogin(string text, IOnCompleteListener c)
        {
            FirebaseUser user = FirebaseAuth.Instance.CurrentUser;
            if (user != null)
            {
                UserProfileChangeRequest.Builder profileUpdates = new UserProfileChangeRequest.Builder();
                profileUpdates.SetDisplayName(text);
                UserProfileChangeRequest updates = profileUpdates.Build();
                user.UpdateProfile(updates).AddOnCompleteListener(c);
            }
            else
            {
                throw new Exception("Current user is null");
            }
        }

        public async Task<List<MyDialog>> FindDialogs(string userMail, Activity context)
        {
            #region userListener
            //if(app == null)
            //{
            //    initFireBaseAuth();
            //}
            //DatabaseReference oRoot = FirebaseDatabase.Instance.Reference.Root;
            //DatabaseReference oUsernamesRef = oRoot.Child("chats");
            //bool result = false;

            //UValueEventListener userListener = new UValueEventListener((sender, e) =>
            //{
            //    result = (e as UEventArgs).value;
            //    if (result)
            //    {
            //        //dialogNames.Add((e as UEventArgs).dialogNames);
            //        dialogNames = (e as UEventArgs).dialogNames;
            //    }

            //}, userMail);

            //oUsernamesRef.AddListenerForSingleValueEvent(userListener);

            //return dialogNames;
            #endregion
            try
            {
                List<MyDialog> dict = new List<MyDialog>();
                client = new Firebase.Database.FirebaseClient("https://easymessage-1fa08.firebaseio.com/");

                var p = await client.Child("chats").OnceAsync<object>();
                var d = p.GetEnumerator();
                d.MoveNext();

                string userlogin = userMail.Replace(".", ",");
                while (d.Current != null)
                {
                    if (d.Current.Key.Contains(userlogin))
                    {
                        string s = d.Current.Object.ToString().Substring(d.Current.Object.ToString().IndexOf(":") + 1);
                        var t = JsonConvert.DeserializeObject<Message>(s.Substring(0, s.Length - 1));
                        dict.Add(new MyDialog { dialogName = d.Current.Key, lastMessage = t });
                    }
                    d.MoveNext();
                }
                return dict;
            }
            catch(Newtonsoft.Json.JsonReaderException exc)
            {
                return null;
            }
            catch (Exception ex)
            {
                Utils.MessageBox(ex.Message, context);
                return null;
            }
        }

        public void SendMessage(string rec, string cont, string sendr, Activity context)
        {
            //var messages2 = await client.Child("mail,test21@mail,ru")
            //.PostAsync(JsonConvert.SerializeObject(new MyDialog { dialogName = "Dialog geniuses1studio@gmail, com + kirill_kovrik@mail, ru",
            //lastMessage = new Message { contentP = "хочет доабвить вас", receiverP = "geniuses1studio@gmail.com", senderP = "kirill_kovrik@mail.ru",
            //timeP = DateTime.Now.ToString() } }));
            //IEnumerator<> enumerator = t.
            //Message mess = JsonConvert.DeserializeObject<Message>(t.ToString());
            //JsonConvert.
            //var m = t as JObject;//.Value<Message>();
            //var r = m.Children();
            //JToken token = r as JToken;
            //var message = r.Value<JToken>();
            //dict.Add(d.Current.Key, t);
            /*if ((d.Current.Object).ToString().Split("contentP", StringSplitOptions.None).Length - 1 < 2 && 
                (d.Current.Object).ToString().Split("contentP", StringSplitOptions.None).Length - 1 > 0)
            {
                list.Add(d.Current.Key);
                d.MoveNext();
            }*/
            /*string json = "{'JSON': { \"mail,test21@mail,ru\" : { \"init\" : \"yes\" }}}";
            JObject newObject = JObject.Parse(json);
            JObject JsonData = (JObject)newObject["JSON"];
            string jsonX = JsonConvert.SerializeObject(JsonData);

            var request = WebRequest.CreateHttp("https://easymessage-1fa08.firebaseio.com/chats/.json");
            request.Method = "PATCH";
            request.ContentType = "JSON";
            var buffer = Encoding.UTF8.GetBytes(jsonX);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            var response = request.GetResponse();
            json = (new StreamReader(response.GetResponseStream())).ReadToEnd();

            if (app == null)
            {
                initFireBaseAuth();
            }
            FirebaseDatabase databaseInstance = FirebaseDatabase.GetInstance(app);

            FirebaseDatabase.Instance.GetReference("chats").AddValueEventListener(context);*/

            //var messages2 = await client.Child("mail,test21@mail,ru").PostAsync(JsonConvert.SerializeObject(new Message("kirill.kop.work@gmail.com", "mail.test21@mail.ru", "Привет222")));

            //var messages3 = await client.Child("mail,test21@mail,ru").PostAsync(JsonConvert.SerializeObject(new Message("kirill.kop.work@gmail.com", "mail.test21@mail.ru", "Привет333")));

            //var messages4 = await client.Child("mail,test21@mail,ru").PostAsync(JsonConvert.SerializeObject(new Message("kirill.kop.work@gmail.com", "mail.test21@mail.ru", "Привет444")));

            //var messages5 = await client.Child("mail,test21@mail,ru").OnceAsync<Message>();

            /*foreach(var s in messages5)
            {
                Utils.MessageBox(s.Object.contentP, (Activity)context);
            }*/
            //Message user = new Message("user1234", "abc", "xyz");

            //Getting Instance of Firebase realtime database

            //Getting Reference to a User node, (it will be created if not already there) 
            //DatabaseReference oRoot = FirebaseDatabase.Instance.Reference.Root;
            //DatabaseReference oUsernamesRef = oRoot.Child("chats");

            /*var userListener = new UValueEventListener((sender, e) =>
            {
                bool result = (e as UEventArgs).value;
                if (result)
                {
                    Utils.MessageBox("The username you selected already exists. Please choose a different one.", (Activity)context);
                    //txtUsername.Background = GetDrawable(Resource.Drawable.edittext_modified_states_error);
                }
            }, "mail,test21@mailru");
            oUsernamesRef.AddListenerForSingleValueEvent(userListener);*/
            //oUsernamesRef.OrderByChild(txtUsername.Text.ToLower()).EqualTo(txtUsername.Text.ToLower()).AddListenerForSingleValueEvent(oListener);


            //oUsernamesRef.OrderByChild("username").EqualTo("mail,test21@mail,ru").AddListenerForSingleValueEvent(userListener);
            /*string json = "{'JSON': { \"mail,test21@mail,ru\" : {\"Hour\": \"10\",\"Minute\" :\"15\",\"Seconds\" :\"26\" }}}";
            JObject newObject = JObject.Parse(json);
            JObject JsonData = (JObject)newObject["JSON"];
            string jsonX = JsonConvert.SerializeObject(JsonData);*/
            //Writing the User class object to that reference
            //var p = JsonConvert.SerializeObject();


            //var p = user;
            //userNode.OrderByKey().Ref.ToArray<object>;
            /*List<string> l = new List<string>();
            var observable = client
            .Child("chats")
            .AsObservable<Message>()
            .Subscribe(d => l.Add(d.Key));*/


            //Firebase.Database.FirebaseObject<Message> messages1 = await client.Child("chats").Child("mail,test21@mail,ru").PostAsync(new Message("kirill.kop.work@gmail.com", "mail.test21@mail.ru", "Привет"));
            //FirebaseObject<Message> messages2 = await client.Child("chats").Child("mail,test21@mail,ru").PostAsync(new Message("mail.test21@mail.ru", "kirill.kop.work@gmail.com", "Хай"));
            //await client.Child("chats").Child(messages.Key).PutAsync("mail.test21@mail.ru");

            //messages[0].
            //
            //client.Child("-M8GzQJhMwow3y5NtR3E").

            //var items2 = await client.Child("-M8GzQJhMwow3y5NtR3E").OnceAsync<Message>();
            //FirebaseUser user = FirebaseAuth.Instance.CurrentUser;
            /*var observable = client
                    .Child("-M8GzQJhMwow3y5NtR3E")
                    .AsObservable<Message>()
                    .Subscribe(d => Console.WriteLine(d.Key));*/
        }
    }

}