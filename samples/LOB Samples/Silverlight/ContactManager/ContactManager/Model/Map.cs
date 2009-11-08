namespace ContactManager.Model
{
    using System.Collections.ObjectModel;
    using Web;

    public static class Map
    {
        public static Contact ToContact(ContactDto dto)
        {
            var contact = new Contact
            {
                ID = dto.ID,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
            };

            contact.Address.Street1 = dto.Address.Street1;
            contact.Address.Street2 = dto.Address.Street2;
            contact.Address.City = dto.Address.City;
            contact.Address.State = dto.Address.State;
            contact.Address.Zip = dto.Address.Zip;

            foreach(var number in dto.Numbers)
            {
                contact.AddPhoneNumber(new PhoneNumber
                {
                    Number = number.Number,
                    Type = number.Type
                });
            }

            return contact;
        }

        public static ContactDto ToDto(Contact contact)
        {
            var dto = new ContactDto
            {
                ID = contact.ID,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Address = new AddressDto
                {
                    Street1 = contact.Address.Street1,
                    Street2 = contact.Address.Street2,
                    City = contact.Address.City,
                    State = contact.Address.State,
                    Zip = contact.Address.Zip
                },
                Numbers = new ObservableCollection<PhoneNumberDto>()
            };

            foreach(var number in contact.Numbers)
            {
                dto.Numbers.Add(new PhoneNumberDto
                {
                    Number = number.Number,
                    Type = number.Type
                });
            }

            return dto;
        }
    }
}