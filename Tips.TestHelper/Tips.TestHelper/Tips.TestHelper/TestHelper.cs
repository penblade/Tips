using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tips.TestHelper
{
    internal class TestHelper
    {
        public static void TestForException(Action method, Exception expectedException)
        {
            // Fail the test if the method does not throw an exception.

            // Example usage:
            // Action method = () => _service.ProcessAction(1);
            // var expectedException = new ArgumentException("The message.");
            // TestHelper.TestForException(method, expectedException);
            try
            {
                method();
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

        public static void TestForException<T>(Func<T> method, Exception expectedException)
        {
            // Fail the test if the method does not throw an exception.

            // Example usage:
            // Action method = () => _service.ProcessFunc(1);
            // var expectedException = new ArgumentException("The message.");
            // TestHelper.TestForException(method, expectedException);
            try
            {
                method();
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
    }
}
