using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace EasyMessage.Encryption
{
    public class CryptoController
    {
        public static CspParameters cspp = new CspParameters();
        public static RSACryptoServiceProvider rsa;
        public static DSACryptoServiceProvider dsa;

        //public static ECDiffieHellmanCng alice;
        //public static ECDiffieHellmanCng bob;

        //public static byte[] senderKey;
        //public static byte[] recieverKey;

        public static CngKey sender;
        public static CngKey receiver;
    }
}