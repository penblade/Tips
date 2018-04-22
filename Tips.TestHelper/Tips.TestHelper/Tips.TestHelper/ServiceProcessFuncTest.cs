using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tips.TestHelper
{
    [TestClass]
    public class ServiceProcessFuncTest
    {
        private readonly Service _service;

        public ServiceProcessFuncTest()
        {
            _service = new Service();
        }

        [TestMethod]
        public void WhenOption0ThenException_OldWay()
        {
            var expectedException = new SystemException("I've got a paperclip and bubblegum, we'll fix this.");;

            try
            {
                _service.ProcessFunc(0);
                Assert.Fail("An exception was not thrown as expected.");
            }
            catch (Exception e)
            {
                // If the exception thrown was from the Assert.Fail, then rethrow.
                if (e.GetType() == typeof(AssertFailedException)) throw;

                Assert.AreEqual(expectedException.GetType(), e.GetType());
                Assert.AreEqual(expectedException.Message, e.Message);
            }
        }

        [TestMethod]
        public void WhenOption0ThenException()
        {
            // Anonymous method
            // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/anonymous-methods

            Action method = delegate { _service.ProcessFunc(0); };
            var expectedException = new SystemException("I've got a paperclip and bubblegum, we'll fix this.");

            TestHelper.TestForException(method, expectedException);
        }

        [TestMethod]
        public void WhenOption1ThenException()
        {
            // Expression lambda
            // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/lambda-expressions

            Action method = () => _service.ProcessFunc(1);
            var expectedException = new ArgumentException("What were you thinking?");

            TestHelper.TestForException(method, expectedException);
        }

        [TestMethod]
        public void WhenOption2ThenException()
        {
            // Local function
            // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/local-functions

            void Method() => _service.ProcessFunc(2);
            var expectedException = new ArgumentNullException($"option");

            TestHelper.TestForException(Method, expectedException);
        }

        [TestMethod]
        public void WhenOption3ThenException()
        {
            // Inline Lamda
            TestHelper.TestForException(() => _service.ProcessFunc(3), new ApplicationException("That's just wrong.  Stop.  Just stop."));
        }

        [TestMethod]
        public void WhenOption3ThenNoException()
        {
            Assert.IsTrue(_service.ProcessFunc(4));
        }
    }
}
