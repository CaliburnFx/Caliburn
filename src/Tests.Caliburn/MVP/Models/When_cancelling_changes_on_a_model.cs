namespace Tests.Caliburn.MVP.Models
{
    using Fakes.ViewModel;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class When_cancelling_changes_on_a_model : TestBase
    {
        private Contact _contact;

        protected override void given_the_context_of()
        {
            _contact = new Contact();
        }

        [Test]
        public void will_not_be_dirty_and_reverts_simple_values()
        {
            _contact.FirstName = "Rob";
            _contact.BeginEdit();
            _contact.FirstName = "Christopher";

            Assert.That(_contact.IsDirty, Is.True);

            _contact.CancelEdit();

            Assert.That(_contact.FirstName, Is.EqualTo("Rob"));
            Assert.That(_contact.IsDirty, Is.False);
        }

        [Test]
        public void will_not_be_dirty_and_reverts_simple_values_on_an_association()
        {
            _contact.Address.Street1 = "1234 Main Street";
            _contact.Address.City = "Tallahassee";
            _contact.Address.State = "FL";

            _contact.BeginEdit();

            _contact.Address.Street1 = "1234 Sesame Street";
            _contact.Address.City = "Somethere";
            _contact.Address.State = "Out There";

            Assert.That(_contact.IsDirty, Is.True);
            Assert.That(_contact.Address.IsDirty, Is.True);

            _contact.CancelEdit();

            Assert.That(_contact.IsDirty, Is.False);
            Assert.That(_contact.Address.IsDirty, Is.False);

            Assert.That(_contact.Address.Street1, Is.EqualTo("1234 Main Street"));
            Assert.That(_contact.Address.City, Is.EqualTo("Tallahassee"));
            Assert.That(_contact.Address.State, Is.EqualTo("FL"));
        }

        [Test]
        public void will_not_be_dirty_and_remoces_values_added_to_a_collection()
        {
            _contact.Numbers.Add(
                new PhoneNumber {Type = PhoneNumberType.Cell, Number = "1234567890"}
                );

            _contact.BeginEdit();

            _contact.Numbers.Add(
                new PhoneNumber {Type = PhoneNumberType.Cell, Number = "1234567890"}
                );

            Assert.That(_contact.IsDirty, Is.True);
            Assert.That(_contact.GetValue(Contact.NumbersProperty).IsDirty, Is.True);

            _contact.CancelEdit();

            Assert.That(_contact.IsDirty, Is.False);
            Assert.That(_contact.GetValue(Contact.NumbersProperty).IsDirty, Is.False);
            Assert.That(_contact.Numbers, Has.Count(1));
        }

        [Test]
        public void will_not_be_dirty_after_committing_values_on_a_child_of_a_collection()
        {
            _contact.Numbers.Add(new PhoneNumber());

            _contact.BeginEdit();

            _contact.Numbers[0].Type = PhoneNumberType.Cell;
            _contact.Numbers[0].Number = "0987654321";

            Assert.That(_contact.Numbers[0].IsDirty, Is.True);
            Assert.That(_contact.IsDirty, Is.True);
            Assert.That(_contact.GetValue(Contact.NumbersProperty).IsDirty, Is.True);

            _contact.CancelEdit();

            Assert.That(_contact.Numbers[0].IsDirty, Is.False);
            Assert.That(_contact.IsDirty, Is.False);
            Assert.That(_contact.GetValue(Contact.NumbersProperty).IsDirty, Is.False);

            Assert.That(_contact.Numbers[0].Number, Is.Null);
        }
    }
}