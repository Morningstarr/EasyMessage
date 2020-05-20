using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Tasks;
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
    }

}