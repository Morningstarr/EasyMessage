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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyMessage
{
    [TestClass]
    public class DataTests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception),
            "Логин не должен содержать символов [ , - { } & ? ^ ; : $ # @ № ! ' \" ( = + ) % * ]")]
        public void LoginTest()
        {
            var result = Utils.IsCorrectLogin("{kirill");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception),
            "Длина логина должна составлять 6-12 символов!")]
        public void LoginLengthTest()
        {
            var result = Utils.IsCorrectLogin("kll");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception),
            "Некорректный адрес электронной почты!")]
        public void IsCorrectEmail()
        {
            var result = Utils.IsCorrectEmail("kll");
        }

        [TestMethod]
        public async void IsDialogExistsTestFalse()
        {
            var result = await FirebaseController.instance.IsDialogExists("dialogname", "dialogname2");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async void IsDialogExistsTestTrue()
        {
            var result = await FirebaseController.instance.IsDialogExists("Dialog geniuses1studio@gmail,com+kirill_kovrik@mail,ru", "contact");
            Assert.IsTrue(result);
        }


    }
}