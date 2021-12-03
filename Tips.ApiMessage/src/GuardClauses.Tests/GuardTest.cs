using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.GuardClauses;

namespace Tips.GuardClauses.Tests
{
    [TestClass]
    public class GuardTest
    {
        [TestMethod]
        [DynamicData(nameof(SetupAgainstNullThrowsException), DynamicDataSourceType.Method)]
        public void AgainstNullThrowsException(object input, string parameterName)
        {
            var expectedException = new ArgumentNullException(parameterName);
            var actualException = Assert.ThrowsException<ArgumentNullException>(() => Guard.AgainstNull(input, parameterName));
            Assert.AreEqual(expectedException.Message, actualException.Message);
        }

        private static IEnumerable<object[]> SetupAgainstNullThrowsException()
        {
            var anonymous = new { };
            anonymous = null;

            yield return new object[] { null, "null" };
            yield return new object[] { anonymous, "anonymousObject" };
            yield return new object[] { null, "tuple" };
            yield return new object[] { null, "string" };
            yield return new object[] { null, "nullableInt" };
            yield return new object[] { null, "nullableBool" };
            yield return new object[] { null, "exception" };
            yield return new object[] { null, "stringList" };
        }

        [TestMethod]
        [DynamicData(nameof(SetupAgainstNullDoesNothingTest), DynamicDataSourceType.Method)]
        public void AgainstNullDoesNothingTest(object input, string parameterName)
        {
            Guard.AgainstNull(input, parameterName);
        }

        private static IEnumerable<object[]> SetupAgainstNullDoesNothingTest()
        {
            var anonymous = new { };

            yield return new object[] { anonymous, "anonymousObject" };
            yield return new object[] { new Tuple<string, int>("test", 5), "tuple" };
            yield return new object[] { "null", "string" };
            yield return new object[] { (int?)1, "nullableInt" };
            yield return new object[] { (bool?)false, "nullableBool" };
            yield return new object[] { new Exception("testException"), "exception" };
            yield return new object[] { new List<string>(), "stringList" };
        }
    }
}
