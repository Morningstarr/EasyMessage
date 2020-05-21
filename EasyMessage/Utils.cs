using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text.Method;
using Android.Views;
using Android.Widget;

namespace EasyMessage
{
    public static class Utils
    {
        private static List<char> symbols;
        
        private static void FillSymbols()
        {
            symbols.Add('[');
            symbols.Add(']');
            symbols.Add(',');
            symbols.Add(';');
            symbols.Add(':');
            symbols.Add('$');
            symbols.Add('#');
            symbols.Add('@');
            symbols.Add('!');
            symbols.Add('^');
            symbols.Add('&');
            symbols.Add('?');
            symbols.Add('-');
            symbols.Add('(');
            symbols.Add(')');
            symbols.Add('=');
            symbols.Add('{');
            symbols.Add('}');
            symbols.Add('%');
            symbols.Add('\'');
            symbols.Add('\"');
            symbols.Add('*');
            symbols.Add(',');
        }
        public static void MessageBox(string MyMessage, Context c)
        {
            AlertDialog.Builder builder;
            builder = new AlertDialog.Builder(c);
            builder.SetTitle("Предупреждение");
            builder.SetMessage(MyMessage);
            builder.SetCancelable(false);
            builder.SetPositiveButton("OK", delegate { });
            Dialog dialog = builder.Create();
            dialog.Show();
            return;
        }

        public static bool IsCorrectEmail(string email)
        {
            Regex regex = new Regex(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,6}\b");
            MatchCollection matches = regex.Matches(email);
            if (matches.Count > 0)
            {
                return true;
            }
            else
            {
                throw new Exception("Некорректный адрес электронной почты!");
            }
        }

        public static bool IsCorrectLogin(string login)
        {
            symbols = new List<char>();
            FillSymbols();
            if(login.Length > 5 && login.Length < 13)
            {
                for (int i = 0; i < symbols.Count; i++) 
                {
                    if (login.Contains(symbols[i])) 
                    {
                        throw new Exception("Логин не должен содержать символов [ , - { } & ? ^ ; : $ # @ № ! ' \" ( = + ) % * ]");
                    }
                }
                return true;
            }
            else
            {
                throw new Exception("Длина логина должна составлять 6-12 символов!");
            }
        }

        public static void passHide(CheckBox c, EditText pss)
        {
            if (c.Checked)
            {
                pss.InputType = Android.Text.InputTypes.TextVariationPassword;
                pss.TransformationMethod = new SingleLineTransformationMethod();
            }
            else
            {
                pss.InputType = Android.Text.InputTypes.TextVariationVisiblePassword;
                pss.TransformationMethod = new PasswordTransformationMethod();
            }
        }

    }
}