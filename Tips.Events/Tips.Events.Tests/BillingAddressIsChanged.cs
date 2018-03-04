using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tips.Events.Tests
{
    [TestClass]
    public class BillingAddressIsChanged
    {
        [TestMethod]
        public void AndIsShippingSameAsBillingIsTrueThenShippingIsUpdated()
        {
            var isShippingSameAsBilling = true;
            var shoppingCart = new ShoppingCart(CreateLogger(), CreateBillingAddress(), CreateShippingAddress(), isShippingSameAsBilling);

            shoppingCart.UpdateBillingAddress(CreateTargetAddress());

            AssertAddressAreEqual(CreateTargetAddress(), shoppingCart.BillingAddress);
            AssertAddressAreEqual(CreateTargetAddress(), shoppingCart.ShippingAddress);

            Assert.IsNotNull(shoppingCart.Logger);

            // Remember, BillingAddress has been changed first before the event is triggered.
            // After: ShippingAddress.City should change to the BillingAddress.City.
            Assert.IsNotNull(shoppingCart.Logger);
            Assert.AreEqual("Before; " +
                            "Sender type: Tips.Events.ShoppingCart; " +
                            "BillingAddress.City: Austin; " +
                            "ShippingAddress.City: Columbus; " +
                            "Address.City: Austin",
                shoppingCart.Logger.Logs.First());

            Assert.AreEqual("After; " +
                            "Sender type: Tips.Events.ShoppingCart; " +
                            "BillingAddress.City: Austin; " +
                            "ShippingAddress.City: Austin; " +
                            "Address.City: Austin",
                shoppingCart.Logger.Logs.Last());
        }

        [TestMethod]
        public void AndIsShippingSameAsBillingIsFalseThenShippingIsNotUpdated()
        {
            var isShippingSameAsBilling = false;
            var shoppingCart = new ShoppingCart(CreateLogger(), CreateBillingAddress(), CreateShippingAddress(), isShippingSameAsBilling);

            shoppingCart.UpdateBillingAddress(CreateTargetAddress());

            AssertAddressAreEqual(CreateTargetAddress(), shoppingCart.BillingAddress);
            AssertAddressAreEqual(CreateShippingAddress(), shoppingCart.ShippingAddress);

            // Remember, BillingAddress has been changed first before the event is triggered.
            Assert.IsNotNull(shoppingCart.Logger);
            Assert.AreEqual("Before; " +
                            "Sender type: Tips.Events.ShoppingCart; " +
                            "BillingAddress.City: Austin; " +
                            "ShippingAddress.City: Columbus; " +
                            "Address.City: Austin", 
                            shoppingCart.Logger.Logs.First());

            // After: ShippingAddress.City should NOT change.
            Assert.AreEqual("After; " +
                            "Sender type: Tips.Events.ShoppingCart; " +
                            "BillingAddress.City: Austin; " +
                            "ShippingAddress.City: Columbus; " +
                            "Address.City: Austin", 
                            shoppingCart.Logger.Logs.Last());
        }

        private void AssertAddressAreEqual(Address expectedAddress, Address actualAddress)
        {
            Assert.IsNotNull(expectedAddress);
            Assert.IsNotNull(actualAddress);
            Assert.AreEqual(expectedAddress.StreetAddress1, actualAddress.StreetAddress1);
            Assert.AreEqual(expectedAddress.StreetAddress2, actualAddress.StreetAddress2);
            Assert.AreEqual(expectedAddress.City, actualAddress.City);
            Assert.AreEqual(expectedAddress.State, actualAddress.State);
            Assert.AreEqual(expectedAddress.Country, actualAddress.Country);
            Assert.AreEqual(expectedAddress.Zip, actualAddress.Zip);
        }

        private static Logger CreateLogger()
        {
            return new Logger();
        }

        private static Address CreateBillingAddress()
        {
            return new Address()
            {
                StreetAddress1 = "123 Original Street",
                StreetAddress2 = "Suite ABC",
                City = "Miami",
                State = "FL",
                Country = "USA",
                Zip = "33133"
            };
        }

        private static Address CreateShippingAddress()
        {
            return new Address()
            {
                StreetAddress1 = "456 Delivery Court",
                StreetAddress2 = "Apartment DEF",
                City = "Columbus",
                State = "OH",
                Country = "USA",
                Zip = "43272"
            };
        }

        private static Address CreateTargetAddress()
        {
            return new Address()
            {
                StreetAddress1 = "789 Bullseye Blvd",
                StreetAddress2 = "Flat GHI",
                City = "Austin",
                State = "TX",
                Country = "USA",
                Zip = "73301"
            };
        }
    }
}
