using EasyMessage;
using NUnit.Framework;

namespace NUnitTestProject1
{
    public class Tests
    {
        [Test]
        public void LoginTest()
        {
            var result = Utils.IsCorrectLogin("{kirill");
            Assert.IsFalse(result);
        }
        [Test]
        public void EmailTest()
        {
            var result = Utils.IsCorrectEmail("kirill_kovrikmail.ru");
            Assert.IsFalse(result);
        }
    }
}