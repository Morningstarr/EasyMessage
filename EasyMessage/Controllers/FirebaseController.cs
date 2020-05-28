using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Gms.Tasks;
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

        private FirebaseApp app;
        private FirebaseAuth auth;
        
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

        public async void SendMessage(string rec, string cont, string sendr, IValueEventListener context)
        {
            try
            {
                client = new Firebase.Database.FirebaseClient("https://easymessage-1fa08.firebaseio.com/chats/");

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

                var messages3 = await client.Child("mail,test21@mail,ru").PostAsync(JsonConvert.SerializeObject(new Message("kirill.kop.work@gmail.com", "mail.test21@mail.ru", "Привет333")));

                var messages4 = await client.Child("mail,test21@mail,ru").PostAsync(JsonConvert.SerializeObject(new Message("kirill.kop.work@gmail.com", "mail.test21@mail.ru", "Привет444")));

                var messages5 = await client.Child("mail,test21@mail,ru").OnceAsync<Message>();

                foreach(var s in messages5)
                {
                    Utils.MessageBox(s.Object.contentP, (Activity)context);
                }
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
            catch(Exception ex)
            {
                Utils.MessageBox(ex.Message, (Activity)context);
            }

        }
    }

}