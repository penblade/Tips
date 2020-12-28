using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Support.Tests
{
    public class Verify
    {
        /// <summary>
        /// Fail the test if the method does not throw an exception.
        /// 
        /// Example usage:
        /// Action method = () => _service.ProcessAction(1);
        /// var expectedException = new ArgumentException("The message.");
        /// AssertHelper.ThrowsException(method, expectedException);
        /// </summary>
        /// <param name="method"></param>
        /// <param name="expectedException"></param>
        public static void ThrowsException(Action method, Exception expectedException)
        {
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

        /// <summary>
        /// Fail the test if the method does not throw an exception.
        ///
        /// Example usage:
        /// Action method = () => _service.ProcessFunc(1);
        /// var expectedException = new ArgumentException("The message.");
        /// AssertHelper.ThrowsException(method, expectedException);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="expectedException"></param>
        public static void ThrowsException<T>(Func<T> method, Exception expectedException)
        {
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

        /// <summary>
        /// Fail the test if the method does not throw an exception.
        ///
        /// Example usage:
        /// Action method = () => _service.ProcessFunc(1);
        /// var expectedException = new ArgumentException("The message.");
        /// AssertHelper.ThrowsExceptionAsync(method, expectedException);
        /// </summary>
        /// <param name="method"></param>
        /// <param name="expectedException"></param>
        /// <returns></returns>
        public static async Task ThrowsExceptionAsync(Func<Task> method, Exception expectedException)
        {
            try
            {
                await method();
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
