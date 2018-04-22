using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tips.TestHelper
{
    [TestClass]
    public class ServiceProcessActionTest
    {
        private readonly Service _service;

        public ServiceProcessActionTest()
        {
            _service = new Service();
        }

        [TestMethod]
        public void WhenOption0ThenException_OldWay()
        {
            var expectedException = new SystemException("I will NOT process that.  Eww."); ;

            try
            {
                _service.ProcessAction(0);
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

            Action method = delegate { _service.ProcessAction(0); };
            var expectedException = new SystemException("I will NOT process that.  Eww.");

            TestHelper.TestForException(method, expectedException);
        }

        [TestMethod]
        public void WhenOption1ThenException()
        {
            // Expression lambda
            // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/lambda-expressions

            Action method = () => _service.ProcessAction(1);
            var expectedException = new ArgumentException("Your connection is cutting out... must be the rain.");

            TestHelper.TestForException(method, expectedException);
        }

        [TestMethod]
        public void WhenOption2ThenException()
        {
            // Local function
            // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/local-functions

            void Method() => _service.ProcessAction(2);
            var expectedException = new ArgumentNullException($"option");

            TestHelper.TestForException(Method, expectedException);
        }

        [TestMethod]
        public void WhenOption3ThenException()
        {
            // Inline Lamda
            TestHelper.TestForException(() => _service.ProcessAction(3), new ApplicationException("Nope, not going to let you do that."));
        }

        [TestMethod]
        public void WhenOption3ThenNoException()
        {
            _service.ProcessAction(4);
        }
    }
}
