using System.Collections.Generic;

namespace Tests.Caliburn.Fakes.Model
{
    public class Customer
    {
        public Customer()
        {
            MailingAddress = new Address();
            BillingAddress = new Address();
            Orders = new List<Order>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public int IQ { get; set; }

        public Address MailingAddress { get; private set; }
        public Address BillingAddress { get; private set; }

        public List<Order> Orders { get; set; }

        public Order this[int index]
        {
            get { return Orders[index]; }
        }
    }
}