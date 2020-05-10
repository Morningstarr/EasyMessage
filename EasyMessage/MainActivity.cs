using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Firebase.Auth;
using Android.Gms.Tasks;
using Firebase;
using System.Threading.Tasks;

namespace EasyMessage
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light.DarkActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IOnCompleteListener
    {
        public static FirebaseApp app;
        FirebaseAuth auth;

        //private FirebaseAuth.AuthStateListener mAuthListener;

        EditText eMail;
        EditText pss;

        public void OnComplete(Android.Gms.Tasks.Task task)
        {
            if (task.IsSuccessful)
            {
                Toast.MakeText(this, "Sign in success", ToastLength.Short).Show();
                //Finish();
            }
            else
            {
                Toast.MakeText(this, "Failed", ToastLength.Short).Show();
                Finish();
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            FirebaseApp.InitializeApp(Application.Context);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            

            var ok = FindViewById<Button>(Resource.Id.btnOK);
            ok.Click += delegate
            {
                ok_Click();
            };
        }

        public void ok_Click()
        {
            initFireBaseAuth();
        }

        private void initFireBaseAuth()
        {
            var options = new FirebaseOptions.Builder()
                .SetApplicationId("1:323956276016:android:fcf09b75b366f4fa50a6f5")
                .SetApiKey("AIzaSyAOvDOj-PKpxFZcDgMO7uI4rxrP3i2GakM")
                .Build();

            if(app == null)
            {
                app = FirebaseApp.InitializeApp(Application.Context, options);
                
            }
            auth = FirebaseAuth.Instance;
            auth = FirebaseAuth.GetInstance(app);

            LoginUser();

        }

        private void LoginUser()
        {
            eMail = FindViewById<EditText>(Resource.Id.edtMail);
            pss = FindViewById<EditText>(Resource.Id.edtPass);

            auth.SignInWithEmailAndPassword(eMail.Text, pss.Text)
                .AddOnCompleteListener(this);
        }
    }
}