namespace ContactManager.Web
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.ServiceModel;

    [ServiceContract]
    public interface IContactService
    {
        [OperationContract]
        List<ContactDto> GetAllContacts();

        [OperationContract]
        void UpdateContact(ContactDto contact);
    }

    [DataContract]
    public class ContactDto
    {
        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public AddressDto Address { get; set; }

        [DataMember]
        public List<PhoneNumberDto> Numbers { get; set; }
    }

    [DataContract]
    public class AddressDto
    {
        [DataMember]
        public string Street1 { get; set; }

        [DataMember]
        public string Street2 { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string Zip { get; set; }
    }

    [DataContract]
    public class PhoneNumberDto
    {
        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public PhoneNumberType Type { get; set; }
    }

    [DataContract]
    public enum PhoneNumberType
    {
        [EnumMember]
        Home = 0,

        [EnumMember]
        Cell = 1,

        [EnumMember]
        Work = 2,

        [EnumMember]
        Fax = 3,

        [EnumMember]
        Other = 4
    }
}