namespace ContactManager.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This is a FAKE implementation.  Do not build your service like this.
    /// </summary>
    public class ContactService : IContactService
    {
        private static readonly List<ContactDto> _contacts = new List<ContactDto>();

        static ContactService()
        {
            CreateDummyData();
        }

        public List<ContactDto> GetAllContacts()
        {
            return _contacts;
        }

        public void UpdateContact(ContactDto contact)
        {
            var found = (from existing in _contacts
                         where contact.ID == existing.ID
                         select existing).FirstOrDefault();

            if(found != null)
                _contacts.Remove(found);

            _contacts.Add(contact);
        }

        private static void CreateDummyData()
        {
            var rob = new ContactDto
            {
                ID = Guid.NewGuid(),
                FirstName = "Rob",
                LastName = "Eisenberg",
                Address = new AddressDto
                {
                    Street1 = "1234 Main Street",
                    City = "Some City",
                    State = "A State",
                    Zip = "12345"
                },
                Numbers = new List<PhoneNumberDto>
                {
                    new PhoneNumberDto {Number = "(123) 456-7890", Type = PhoneNumberType.Cell},
                    new PhoneNumberDto {Number = "(980) 765-4321", Type = PhoneNumberType.Work}
                }
            };

            _contacts.Add(rob);
        }
    }
}