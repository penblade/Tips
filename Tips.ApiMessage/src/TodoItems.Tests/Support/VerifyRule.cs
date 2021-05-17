using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TodoItems.Tests.Support
{
    internal class VerifyRule
    {
        public static void VerifyIsAssignableFrom<TBaseClass, TClassUnderTest>() => Assert.IsTrue(typeof(TBaseClass).IsAssignableFrom(typeof(TClassUnderTest)));
    }
}
