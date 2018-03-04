using System;
using System.Collections.Generic;
using System.Text;

namespace Tips.Events
{
    public class Logger
    {
        public List<string> Logs { get; }

        public Logger()
        {
            Logs = new List<string>();
        }

        public void OnBillingAddressChangedBefore(object sender, Address address)
        {
            OnBillingAddressChanged(sender, address, "Before");
        }

        public void OnBillingAddressChangedAfter(object sender, Address address)
        {
            OnBillingAddressChanged(sender, address, "After");
        }

        private void OnBillingAddressChanged(object sender, Address address, string status)
        {
            // You could access the variables via "this" if we were in the
            // ShoppingCart because it has the EventHandler.

            // In this case we're in a different class, so we need to cast
            // the sender to the appropriate type, do a null check,
            // and then perform an action.
            if (!(sender is ShoppingCart cart)) throw new ArgumentNullException($"{nameof(sender)} was not a {nameof(ShoppingCart)}.");

            var billingAddressCity = cart.BillingAddress.City;
            var shippingAddressCity = cart.ShippingAddress.City;

            Logs.Add($"{status}; " +
                     $"Sender type: {sender.GetType()}; " +
                     $"BillingAddress.City: {billingAddressCity}; " +
                     $"ShippingAddress.City: {shippingAddressCity}; " +
                     $"Address.City: {address.City}");
        }
    }
}
