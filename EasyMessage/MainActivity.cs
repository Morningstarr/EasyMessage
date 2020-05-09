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
        FirebaseAuth auth;

        public void OnComplete(Android.Gms.Tasks.Task task)
        {
            if (task.IsSuccessful)
            {
                Toast.MakeText(this, "Sign in success", ToastLength.Short).Show();
                Finish();
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

            //auth = FirebaseAuth.Instance;

            var ok = FindViewById<Button>(Resource.Id.btnOK);
            ok.Click += async delegate
            {
                await ok_Click();
            };
        }

        public async Task<string> ok_Click()
        {
            var eMail = FindViewById<EditText>(Resource.Id.edtMail);
            var pss = FindViewById<EditText>(Resource.Id.edtPass);
            

            var user = await FirebaseAuth.Instance.
                        CreateUserWithEmailAndPasswordAsync(eMail.Text, pss.Text);
            var token = await user.User.GetIdTokenAsync(false);
            return token.Token;
        }
    }
}