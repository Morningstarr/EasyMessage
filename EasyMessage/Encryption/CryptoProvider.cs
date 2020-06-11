﻿using System;
using System.Collections.Generic;
using System.IO;
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
    class CryptoProvider
    {
        TripleDESCryptoServiceProvider tDes = new TripleDESCryptoServiceProvider();
        public byte[] initVect_p { get; set; }
        public string initText_p { get; set; }

        public string initFileName_p { get; set; }
        public string finalText_p { get; set; }

        public byte[] initKey_p { get; set; }

        public static void Encrupt(string dat, string userKey, string k, CryptoProvider cc)
        {
            TripleDESCryptoServiceProvider tDes = new TripleDESCryptoServiceProvider();
            byte[] data;
            byte[] encryptedKey;

            var crypto = new RSACryptoServiceProvider();
            crypto.FromXmlString(userKey);
            encryptedKey = crypto.Encrypt(tDes.Key, false);  

            byte[] LenK = new byte[4];
            byte[] LenIV = new byte[4];

            int lKey = encryptedKey.Length;
            LenK = BitConverter.GetBytes(lKey);
            int lIV = tDes.IV.Length;
            LenIV = BitConverter.GetBytes(lIV);
            data = cc.TextToMemory(dat, tDes.Key, tDes.IV);

            byte[] alld = new byte[8 + encryptedKey.Length + tDes.IV.Length + data.Length];
            Array.Copy(LenK, 0, alld, 0, LenK.Length);
            Array.Copy(LenIV, 0, alld, LenK.Length, LenIV.Length);
            Array.Copy(encryptedKey, 0, alld, LenK.Length + LenIV.Length, encryptedKey.Length);
            Array.Copy(tDes.IV, 0, alld, LenK.Length + LenIV.Length + encryptedKey.Length, tDes.IV.Length);
            Array.Copy(data, 0, alld, LenK.Length + LenIV.Length + encryptedKey.Length + tDes.IV.Length, data.Length);

            string s = Convert.ToBase64String(alld);

            byte[] initdata = Convert.FromBase64String(s);

            byte[] key;
            byte[] keylenb = new byte[4];
            int keylen;
            int vectlen;
            byte[] initvec;
            byte[] vectlenb = new byte[4];
            byte[] dt;

            MemoryStream ms = new MemoryStream(initdata);

            ms.Read(keylenb, 0, 4);
            keylen = BitConverter.ToInt32(keylenb, 0);
            ms.Read(vectlenb, 0, 4);
            vectlen = BitConverter.ToInt32(vectlenb, 0);
            key = new byte[keylen];
            ms.Read(key, 0, keylen);
            initvec = new byte[vectlen];
            ms.Read(initvec, 0, vectlen);
            data = new byte[ms.Length - (8 + keylen + vectlen)];
            ms.Read(data, 0, (int)ms.Length - (8 + keylen + vectlen));

            byte[] decryptedKey;

            var decrypt = new RSACryptoServiceProvider();
            decrypt.FromXmlString(k);
            //decryptedKey = RSADecrypt(key, CryptoController.rsa.ExportParameters(true), false);
            decryptedKey = decrypt.Decrypt(key, false);
           
            string initText_p = cc.TextFromMemory(data, decryptedKey, initvec);
            //return initText_p;c
        }

        public string Encrypt(string dat, string userKey)
        {
            //UnicodeEncoding ByteConverter = new UnicodeEncoding();

            byte[] data;
            byte[] encryptedKey;


            var crypto = new RSACryptoServiceProvider();
            crypto.FromXmlString(userKey);
            encryptedKey = crypto.Encrypt(tDes.Key, false);
            
            byte[] LenK = new byte[4];
            byte[] LenIV = new byte[4];

            int lKey = encryptedKey.Length;
            LenK = BitConverter.GetBytes(lKey);
            int lIV = tDes.IV.Length;
            LenIV = BitConverter.GetBytes(lIV);
            data = TextToMemory(dat, tDes.Key, tDes.IV);

            byte[] alld = new byte[8 + encryptedKey.Length + tDes.IV.Length + data.Length];
            Array.Copy(LenK, 0, alld, 0, LenK.Length);
            Array.Copy(LenIV, 0, alld, LenK.Length, LenIV.Length);
            Array.Copy(encryptedKey, 0, alld, LenK.Length + LenIV.Length, encryptedKey.Length);
            Array.Copy(tDes.IV, 0, alld, LenK.Length + LenIV.Length + encryptedKey.Length, tDes.IV.Length);
            Array.Copy(data, 0, alld, LenK.Length + LenIV.Length + encryptedKey.Length + tDes.IV.Length, data.Length);


            return Convert.ToBase64String(alld)/* ByteConverter.GetString(alld)*/;

             //finalText_p;
        }

        private byte[] TextToMemory(string Data, byte[] Key, byte[] IV)
        {
            tDes.Mode = CipherMode.CBC;
            try
            {
                MemoryStream mStream = new MemoryStream();

                CryptoStream cStream = new CryptoStream(mStream, new TripleDESCryptoServiceProvider().CreateEncryptor(Key, IV), CryptoStreamMode.Write);
                initVect_p = IV;

                byte[] toEncrypt = Encoding.UTF8.GetBytes(Data);

                cStream.Write(toEncrypt, 0, toEncrypt.Length);
                cStream.FlushFinalBlock();

                byte[] ret = mStream.ToArray();

                cStream.Close();
                mStream.Close();

                return ret;
            }
            catch (CryptographicException)
            {
                //MessageBox.Show("Error!");
                return null;
            }
        }

        public string Decrypt(string dat, string k)
        {
            //UnicodeEncoding ByteConverter = new UnicodeEncoding();

            byte[] initdata = Convert.FromBase64String(dat) /*ByteConverter.GetBytes(dat)*/;

            byte[] key;
            byte[] keylenb = new byte[4];
            int keylen;
            int vectlen;
            byte[] initvec;
            byte[] vectlenb = new byte[4];
            byte[] data;

            MemoryStream ms = new MemoryStream(initdata);

            ms.Read(keylenb, 0, 4);
            keylen = BitConverter.ToInt32(keylenb, 0);
            ms.Read(vectlenb, 0, 4);
            vectlen = BitConverter.ToInt32(vectlenb, 0);
            key = new byte[keylen];
            ms.Read(key, 0, keylen);
            initvec = new byte[vectlen];
            ms.Read(initvec, 0, vectlen);
            data = new byte[ms.Length - (8 + keylen + vectlen)];
            ms.Read(data, 0, (int)ms.Length - (8 + keylen + vectlen));

            byte[] decryptedKey;

            try
            {
                var decrypt = new RSACryptoServiceProvider();
                decrypt.FromXmlString(k);
                //decryptedKey = RSADecrypt(key, CryptoController.rsa.ExportParameters(true), false);
                decryptedKey = decrypt.Decrypt(key, false);
            }
            catch (Exception exc)
            {
                //MessageBox.Show(exc.Message);
                return "";
            }

            initText_p = TextFromMemory(data, decryptedKey, initvec).TrimEnd('\0');
            return initText_p;

        }

        private string TextFromMemory(byte[] Data, byte[] Key, byte[] IV)
        {
            tDes.Mode = CipherMode.CBC;
            try
            {
                MemoryStream msDecrypt = new MemoryStream(Data);

                CryptoStream csDecrypt = new CryptoStream(msDecrypt, new TripleDESCryptoServiceProvider().CreateDecryptor(Key, IV), CryptoStreamMode.Read);

                byte[] fromEncrypt = new byte[Data.Length];

                csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

                return Encoding.UTF8.GetString(fromEncrypt);
            }
            catch (CryptographicException ex)
            {
                //Utils.MessageBox("E);
                return null;
            }
        }

        public static byte[] RSAEncrypt(byte[] DataToEncrypt/*, RSAParameters RSAKeyInfo, bool DoOAEPPadding*/)
        {
            try
            {
                byte[] encryptedData;
                //CryptoController.rsa.ImportParameters(RSAKeyInfo);
                encryptedData = CryptoController.rsa.Encrypt(DataToEncrypt, false);
                return encryptedData;
            }
            catch (CryptographicException)
            {
                //MessageBox.Show("Ошибка при шифровании ключа!");
                return null;
            }

        }

        public static byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] decryptedData;
                CryptoController.rsa.ImportParameters(RSAKeyInfo);
                decryptedData = CryptoController.rsa.Decrypt(DataToDecrypt, DoOAEPPadding);
                return decryptedData;
            }
            catch (CryptographicException)
            {
                //MessageBox.Show("Ошибка при дешифровании ключа!");
                return null;
            }
        }

        public static byte[] RSADecrypt(byte[] DataToDecrypt, string key)
        {
            try
            {
                byte[] decryptedData;
                var rsaPrivate = new RSACryptoServiceProvider();
                rsaPrivate.FromXmlString(key);
                decryptedData = rsaPrivate.Decrypt(DataToDecrypt, false);
                return decryptedData;
            }
            catch (CryptographicException)
            {
                //MessageBox.Show("Ошибка при дешифровании ключа!");
                return null;
            }
        }

        public static List<string> GenerateRSAKeys()
        {
            RSACryptoServiceProvider rSACrypto = new RSACryptoServiceProvider();
            string open = rSACrypto.ToXmlString(false);
            string privat = rSACrypto.ToXmlString(true);
            List<string> keys = new List<string>();
            keys.Add(open);
            keys.Add(privat);
            return keys;
        }

        public static void EncryptDecrypt(string data, string publicKey, string privateKey)
        {
            byte[] dat = Encoding.UTF8.GetBytes(data);
            var publ = new RSACryptoServiceProvider();
            publ.FromXmlString(publicKey);

            var encrypted = publ.Encrypt(dat, false);

            var priv = new RSACryptoServiceProvider();
            priv.FromXmlString(privateKey);

            var decrypted = priv.Decrypt(encrypted, false);
            string resultdata = Encoding.UTF8.GetString(decrypted);
        }

        #region sign-verify
        /*public string Sign(string dat)
        {
            try
            {
                byte[] sourcebytes = Encoding.UTF8.GetBytes(dat);
                byte[] hashbytes;
                using (var sha = SHA1.Create())
                {
                    hashbytes = sha.ComputeHash(sourcebytes);
                }
                if (App.dsa == null)
                {
                    App.dsa = new DSACryptoServiceProvider();
                }
                App.dsa.FromXmlString(App.currentAcc.privateKeyD_p);
                byte[] signed = App.dsa.SignHash(hashbytes, HashAlgorithmName.SHA1.Name);
                string outt = Convert.ToBase64String(signed);
                byte[] bs = Convert.FromBase64String(outt);

                MessageBox.Show("Подпись успешно создана!");
                return outt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Создание подписи было провалено!");
                return "";
            }
        }

        public string Sign()
        {
            try
            {
                byte[] sourcebytes = Encoding.UTF8.GetBytes(initText_p);
                byte[] hashbytes;
                using (var sha = SHA512.Create())
                {
                    hashbytes = sha.ComputeHash(sourcebytes);
                }
                if (App.dsa == null)
                {
                    App.dsa = new DSACryptoServiceProvider();
                }
                App.dsa.FromXmlString(App.currentAcc.openKeyD_p);
                byte[] signed = App.dsa.SignHash(hashbytes, HashAlgorithmName.SHA512.Name);
                string outt = Convert.ToBase64String(signed);
                byte[] bs = Convert.FromBase64String(outt);

                MessageBox.Show("Подпись успешно создана!");
                return outt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Создание подписи было провалено!");
                return "";
            }
        }



        public bool Verify(string initHash, string text, string key)
        {
            try
            {
                if (App.dsa == null)
                {
                    App.dsa = new DSACryptoServiceProvider();
                }
                App.dsa.FromXmlString(key);
                byte[] hashbytes;
                using (var sha = SHA1.Create())
                {
                    hashbytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
                }

                if (App.dsa.VerifyHash(hashbytes, HashAlgorithmName.SHA1.Name, Convert.FromBase64String(initHash)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }*/
        #endregion
    }
}