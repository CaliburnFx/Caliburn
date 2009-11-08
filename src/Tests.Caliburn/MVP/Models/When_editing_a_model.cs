namespace Tests.Caliburn.MVP.Models
{
    using Fakes.ViewModel;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class When_editing_a_model : TestBase
    {
        private Contact _contact;

        protected override void given_the_context_of()
        {
            _contact = new Contact();
            _contact.BeginEdit();
        }

        [Test]
        public void will_be_dirty_when_setting_simple_values()
        {
            Assert.That(_contact.IsDirty, Is.False);

            _contact.FirstName = "Rob";
            _contact.LastName = "Eisenberg";

            Assert.That(_contact.IsDirty, Is.True);
        }

        [Test]
        public void will_be_dirty_when_setting_simple_values_on_an_association()
        {
            Assert.That(_contact.IsDirty, Is.False);
            Assert.That(_contact.Address.IsDirty, Is.False);

            _contact.Address.Street1 = "1234 Main Street";
            _contact.Address.City = "Tallahassee";
            _contact.Address.State = "FL";

            Assert.That(_contact.IsDirty, Is.True);
            Assert.That(_contact.Address.IsDirty, Is.True);
        }

        [Test]
        public void will_be_dirty_when_adding_values_to_a_collection()
        {
            Assert.That(_contact.IsDirty, Is.False);
            Assert.That(_contact.GetValue(Contact.NumbersProperty).IsDirty, Is.False);

            _contact.Numbers.Add(
                new PhoneNumber {Type = PhoneNumberType.Cell, Number = "1234567890"}
                );

            Assert.That(_contact.IsDirty, Is.True);
            Assert.That(_contact.GetValue(Contact.NumbersProperty).IsDirty, Is.True);
        }

        [Test]
        public void will_be_dirty_when_setting_values_on_a_child_of_a_collection()
        {
            Assert.That(_contact.IsDirty, Is.False);
            Assert.That(_contact.GetValue(Contact.NumbersProperty).IsDirty, Is.False);

            _contact.Numbers.Add(new PhoneNumber());

            Assert.That(_contact.Numbers[0].IsDirty, Is.False);

            _contact.Numbers[0].Type = PhoneNumberType.Cell;
            _contact.Numbers[0].Number = "0987654321";

            Assert.That(_contact.Numbers[0].IsDirty, Is.True);

            Assert.That(_contact.IsDirty, Is.True);
            Assert.That(_contact.GetValue(Contact.NumbersProperty).IsDirty, Is.True);
        }
    }
}