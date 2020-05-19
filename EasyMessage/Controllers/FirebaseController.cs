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
using Firebase.Auth;

namespace EasyMessage
{
    public class FirebaseController
    {
        public static FirebaseController instance = new FirebaseController();

        private FirebaseApp app;
        private FirebaseAuth auth;

        public void initFireBaseAuth()
        {
            var options = new FirebaseOptions.Builder()
                .SetApplicationId("1:323956276016:android:fcf09b75b366f4fa50a6f5")
                .SetApiKey("AIzaSyAOvDOj-PKpxFZcDgMO7uI4rxrP3i2GakM")
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

        public async Task<string> Register(string eMail, string password)
        {
            IAuthResult user = await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(eMail, password);
            var token = await user.User.GetIdTokenAsync(false);
            return token.Token;
        }

        public async void ResetPassword(string eMail)
        {
            await FirebaseAuth.Instance.SendPasswordResetEmailAsync(eMail);
        }

        public async void ResetEmail(string newm)
        {
            FirebaseUser user = FirebaseAuth.Instance.CurrentUser;
            await user.UpdateEmailAsync(newm);
        }

        public async void ChangePass(string newpass)
        {
            FirebaseUser user = FirebaseAuth.Instance.CurrentUser;
            await user.UpdatePasswordAsync(newpass);
        }
    }

}