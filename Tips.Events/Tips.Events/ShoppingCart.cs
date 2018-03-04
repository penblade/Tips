using System;

namespace Tips.Events
{
    public class ShoppingCart
    {
        public EventHandler<Address> BillingAddressEventHandler;

        public Logger Logger { get; }
        public Address BillingAddress { get; }
        public Address ShippingAddress { get; }
        public bool IsShippingSameAsBilling { get; }

        public ShoppingCart(Logger logger, Address billingAddress, Address shippingAddress, bool isShippingSameAsBilling)
        {
            Logger = logger ?? throw new ArgumentNullException($"{nameof(logger)} is null.");

            BillingAddress = billingAddress ?? throw new ArgumentNullException($"{nameof(billingAddress)} is null.");
            ShippingAddress = shippingAddress ?? throw new ArgumentNullException($"{nameof(shippingAddress)} is null.");

            IsShippingSameAsBilling = isShippingSameAsBilling;

            // When the event handler is called we want both of these methods to be called.
            BillingAddressEventHandler += Logger.OnBillingAddressChangedBefore;
            BillingAddressEventHandler += OnBillingAddressChanged;
            BillingAddressEventHandler += Logger.OnBillingAddressChangedAfter;
        }

        public void UpdateBillingAddress(Address address)
        {
            UpdateAddress(address, BillingAddress);

            // The billing address is changed, call the event handler.
            BillingAddressEventHandler(this, address);
        }

        public void UpdateShippingAddress(Address address)
        {
            UpdateAddress(address, ShippingAddress);
        }

        private void UpdateAddress(Address sourceAddress, Address targetAddress)
        {
            if (sourceAddress == null) throw new ArgumentNullException($"{nameof(sourceAddress)} is null.");
            if (targetAddress == null) throw new ArgumentNullException($"{nameof(targetAddress)} is null.");

            targetAddress.StreetAddress1 = sourceAddress.StreetAddress1;
            targetAddress.StreetAddress2 = sourceAddress.StreetAddress2;
            targetAddress.City = sourceAddress.City;
            targetAddress.State = sourceAddress.State;
            targetAddress.Country = sourceAddress.Country;
            targetAddress.Zip = sourceAddress.Zip;
        }

        private void OnBillingAddressChanged(object sender, Address address)
        {
            // Only update the billing address if shipping is same as billing.
            if (IsShippingSameAsBilling) UpdateShippingAddress(address);
        }
    }
}
