using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MonoDroidUnitTesting;

namespace EasyMessage
{
    [Activity(Label = "MonoDroidUnitTesting Example")]
    public class TestActivity : GuiTestRunnerActivity
    {
        protected override TestRunner CreateTestRunner()
        {
            var runner = new TestRunner();
            // Run all tests from this assembly
            runner.AddTests(Assembly.GetExecutingAssembly());
            return runner;
        }
    }
}