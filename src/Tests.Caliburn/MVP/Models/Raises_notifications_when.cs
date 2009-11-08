namespace Tests.Caliburn.MVP.Models
{
    using Fakes.ViewModel;
    using global::Caliburn.ModelFramework;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class Raises_notifications_when : TestBase
    {
        private Contact _contact;

        protected override void given_the_context_of()
        {
            _contact = new Contact();
        }

        [Test]
        public void initializing_a_property_value()
        {
            bool changeWasRaised = false;
            bool noOtherEventsFired = true;

            _contact.PropertyChanged +=
                (s, e) =>{
                    if(e.PropertyName == "FirstName") changeWasRaised = true;
                    else noOtherEventsFired = false;
                };

            _contact.FirstName = "Rob";

            Assert.That(changeWasRaised);
            Assert.That(noOtherEventsFired);
        }

        [Test]
        public void changing_a_property_value()
        {
            bool changeWasRaised = false;
            bool dirtyWasRaised = false;
            bool validWasRaised = false;
            bool editingWasRaised = false;
            bool noOtherEventsFired = true;

            _contact.PropertyChanged +=
                (s, e) =>{
                    if(e.PropertyName == "FirstName") changeWasRaised = true;
                    else if(e.PropertyName == "IsDirty") dirtyWasRaised = true;
                    else if(e.PropertyName == "IsValid") validWasRaised = true;
                    else if(e.PropertyName == "IsEditing") editingWasRaised = true;
                    else noOtherEventsFired = false;
                };

            _contact.BeginEdit();
            _contact.FirstName = "Rob";

            Assert.That(changeWasRaised);
            Assert.That(dirtyWasRaised);
            Assert.That(validWasRaised);
            Assert.That(editingWasRaised);
            Assert.That(noOtherEventsFired);
        }

        [Test]
        public void changing_the_model()
        {
            _contact.FirstName = "Rob";

            IModelNode modelNode = null;
            IUndoRedoHandle handle = null;

            _contact.ModelChanged += (m, h) =>{
                modelNode = m;
                handle = h;
            };

            _contact.BeginEdit();

            _contact.FirstName = "Christopher";

            Assert.That(modelNode, Is.EqualTo(_contact[Contact.FirstNameProperty.Name]));
            Assert.That(handle, Is.Not.Null);
        }

        [Test]
        public void cancelling_a_property_change()
        {
            _contact.FirstName = "Rob";
            _contact.BeginEdit();
            _contact.FirstName = "Christopher";

            bool changeWasRaised = false;
            bool dirtyWasRaised = false;
            bool editingWasRaised = false;

            _contact.PropertyChanged +=
                (s, e) =>{
                    if(e.PropertyName == "FirstName") changeWasRaised = true;
                    else if(e.PropertyName == "IsDirty") dirtyWasRaised = true;
                    else if(e.PropertyName == "IsEditing") editingWasRaised = true;
                };

            _contact.CancelEdit();

            Assert.That(changeWasRaised);
            Assert.That(dirtyWasRaised);
            Assert.That(editingWasRaised);
        }

        [Test]
        public void committing_a_property_change()
        {
            _contact.FirstName = "Rob";
            _contact.BeginEdit();
            _contact.FirstName = "Christopher";

            bool dirtyWasRaised = false;
            bool editingWasRaised = false;

            _contact.PropertyChanged +=
                (s, e) =>{
                    if(e.PropertyName == "IsDirty") dirtyWasRaised = true;
                    else if(e.PropertyName == "IsEditing") editingWasRaised = true;
                };

            _contact.EndEdit();

            Assert.That(dirtyWasRaised);
            Assert.That(editingWasRaised);
        }

        [Test]
        public void changing_an_associated_value_that_bubble_to_the_root()
        {
            bool dirtyWasRaised = false;
            bool validWasRaised = false;
            bool editingWasRaised = false;
            bool noOtherEventsFired = true;

            _contact.PropertyChanged +=
                (s, e) =>{
                    if(e.PropertyName == "IsDirty") dirtyWasRaised = true;
                    else if(e.PropertyName == "IsValid") validWasRaised = true;
                    else if(e.PropertyName == "IsEditing") editingWasRaised = true;
                    else noOtherEventsFired = false;
                };

            _contact.BeginEdit();
            _contact.Address.Street1 = "1234 Main Street";

            Assert.That(dirtyWasRaised);
            Assert.That(validWasRaised);
            Assert.That(editingWasRaised);
            Assert.That(noOtherEventsFired);
        }

        [Test]
        public void adding_an_item_to_a_child_collection_that_bubble_to_the_root()
        {
            bool dirtyWasRaised = false;
            bool validWasRaised = false;
            bool editingWasRaised = false;
            bool noOtherEventsFired = true;

            _contact.PropertyChanged +=
                (s, e) =>{
                    if(e.PropertyName == "IsDirty") dirtyWasRaised = true;
                    else if(e.PropertyName == "IsValid") validWasRaised = true;
                    else if(e.PropertyName == "IsEditing") editingWasRaised = true;
                    else noOtherEventsFired = false;
                };

            _contact.BeginEdit();
            _contact.Numbers.Add(new PhoneNumber());

            Assert.That(dirtyWasRaised);
            Assert.That(validWasRaised);
            Assert.That(editingWasRaised);
            Assert.That(noOtherEventsFired);
        }

        [Test]
        public void changing_values_of_a_child_collection_item_that_bubble_to_the_root()
        {
            _contact.Numbers.Add(new PhoneNumber());

            bool dirtyWasRaised = false;
            bool validWasRaised = false;
            bool editingWasRaised = false;
            bool noOtherEventsFired = true;

            _contact.PropertyChanged +=
                (s, e) =>{
                    if(e.PropertyName == "IsDirty") dirtyWasRaised = true;
                    else if(e.PropertyName == "IsValid") validWasRaised = true;
                    else if(e.PropertyName == "IsEditing") editingWasRaised = true;
                    else noOtherEventsFired = false;
                };

            _contact.BeginEdit();
            _contact.Numbers[0].Type = PhoneNumberType.Fax;

            Assert.That(dirtyWasRaised);
            Assert.That(validWasRaised);
            Assert.That(editingWasRaised);
            Assert.That(noOtherEventsFired);
        }
    }
}