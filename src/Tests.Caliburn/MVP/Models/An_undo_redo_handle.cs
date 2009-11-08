namespace Tests.Caliburn.MVP.Models
{
    using System.Collections.Generic;
    using Fakes.ViewModel;
    using global::Caliburn.Core;
    using global::Caliburn.ModelFramework;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class An_undo_redo_handle : TestBase
    {
        private Contact _contact;

        protected override void given_the_context_of()
        {
            _contact = new Contact();
        }

        [Test]
        public void can_undo_redo_a_simple_property_change()
        {
            const string name = "Rob";

            IUndoRedoHandle handle = null;
            _contact.ModelChanged += (m, h) => { handle = h; };

            Assert.That(_contact.FirstName, Is.Null);

            _contact.BeginEdit();
            _contact.FirstName = name;

            Assert.That(_contact.FirstName, Is.EqualTo(name));

            handle.Undo();

            Assert.That(_contact.FirstName, Is.Null);

            handle.Redo();

            Assert.That(_contact.FirstName, Is.EqualTo(name));
        }

        [Test]
        public void can_undo_redo_an_associated_property_change()
        {
            const string city = "Tallahassee";

            IUndoRedoHandle handle = null;
            _contact.ModelChanged += (m, h) => { handle = h; };

            Assert.That(_contact.Address.City, Is.Null);

            _contact.BeginEdit();
            _contact.Address.City = city;

            Assert.That(_contact.Address.City, Is.EqualTo(city));

            handle.Undo();

            Assert.That(_contact.Address.City, Is.Null);

            handle.Redo();

            Assert.That(_contact.Address.City, Is.EqualTo(city));
        }

        [Test]
        public void can_undo_redo_a_collection_add()
        {
            var number = new PhoneNumber();

            IUndoRedoHandle handle = null;
            _contact.ModelChanged += (m, h) => { handle = h; };

            _contact.Numbers.Add(new PhoneNumber());
            Assert.That(_contact.Numbers, Has.Count(1));

            _contact.BeginEdit();
            _contact.Numbers.Add(number);

            Assert.That(_contact.Numbers[1], Is.EqualTo(number));

            handle.Undo();

            Assert.That(_contact.Numbers, Has.Count(1));

            handle.Redo();

            Assert.That(_contact.Numbers[1], Is.EqualTo(number));
        }

        [Test]
        public void can_undo_redo_a_collection_insert()
        {
            IUndoRedoHandle handle = null;
            _contact.ModelChanged += (m, h) => { handle = h; };

            for(int i = 0; i < 3; i++) _contact.Numbers.Add(new PhoneNumber());

            Assert.That(_contact.Numbers, Has.Count(3));

            _contact.BeginEdit();

            var itemToInsert = new PhoneNumber();

            _contact.Numbers.Insert(1, itemToInsert);

            Assert.That(_contact.Numbers, Has.Count(4));
            Assert.That(_contact.Numbers[1], Is.EqualTo(itemToInsert));

            handle.Undo();

            Assert.That(_contact.Numbers, Has.Count(3));
            Assert.That(_contact.Numbers[1], Is.Not.EqualTo(itemToInsert));

            handle.Redo();

            Assert.That(_contact.Numbers, Has.Count(4));
            Assert.That(_contact.Numbers[1], Is.EqualTo(itemToInsert));
        }

        [Test]
        public void can_undo_redo_a_collection_remove()
        {
            IUndoRedoHandle handle = null;
            _contact.ModelChanged += (m, h) => { handle = h; };

            for(int i = 0; i < 3; i++) _contact.Numbers.Add(new PhoneNumber());

            Assert.That(_contact.Numbers, Has.Count(3));

            _contact.BeginEdit();

            var itemToRemove = _contact.Numbers[1];

            _contact.Numbers.Remove(itemToRemove);

            Assert.That(_contact.Numbers, Has.Count(2));
            Assert.That(_contact.Numbers[0], Is.Not.EqualTo(itemToRemove));
            Assert.That(_contact.Numbers[1], Is.Not.EqualTo(itemToRemove));

            handle.Undo();

            Assert.That(_contact.Numbers, Has.Count(3));
            Assert.That(_contact.Numbers[1], Is.EqualTo(itemToRemove));

            handle.Redo();

            Assert.That(_contact.Numbers, Has.Count(2));
            Assert.That(_contact.Numbers[0], Is.Not.EqualTo(itemToRemove));
            Assert.That(_contact.Numbers[1], Is.Not.EqualTo(itemToRemove));
        }

        [Test]
        public void can_undo_redo_a_collection_remove_at()
        {
            IUndoRedoHandle handle = null;
            _contact.ModelChanged += (m, h) => { handle = h; };

            for(int i = 0; i < 3; i++) _contact.Numbers.Add(new PhoneNumber());

            Assert.That(_contact.Numbers, Has.Count(3));

            _contact.BeginEdit();

            var itemToRemove = _contact.Numbers[1];

            _contact.Numbers.RemoveAt(1);

            Assert.That(_contact.Numbers, Has.Count(2));
            Assert.That(_contact.Numbers[0], Is.Not.EqualTo(itemToRemove));
            Assert.That(_contact.Numbers[1], Is.Not.EqualTo(itemToRemove));

            handle.Undo();

            Assert.That(_contact.Numbers, Has.Count(3));
            Assert.That(_contact.Numbers[1], Is.EqualTo(itemToRemove));

            handle.Redo();

            Assert.That(_contact.Numbers, Has.Count(2));
            Assert.That(_contact.Numbers[0], Is.Not.EqualTo(itemToRemove));
            Assert.That(_contact.Numbers[1], Is.Not.EqualTo(itemToRemove));
        }

        [Test]
        public void can_undo_redo_a_collection_clear()
        {
            IUndoRedoHandle handle = null;
            _contact.ModelChanged += (m, h) => { handle = h; };

            var list = new List<PhoneNumber>
            {
                new PhoneNumber(),
                new PhoneNumber(),
                new PhoneNumber()
            };

            list.Apply(x => _contact.Numbers.Add(x));

            Assert.That(_contact.Numbers, Has.Count(3));

            _contact.BeginEdit();

            _contact.Numbers.Clear();

            Assert.That(_contact.Numbers, Has.Count(0));

            handle.Undo();

            Assert.That(_contact.Numbers, Has.Count(3));
            Assert.That(_contact.Numbers[0], Is.EqualTo(list[0]));
            Assert.That(_contact.Numbers[1], Is.EqualTo(list[1]));
            Assert.That(_contact.Numbers[2], Is.EqualTo(list[2]));

            handle.Redo();

            Assert.That(_contact.Numbers, Has.Count(0));
        }

        [Test]
        public void can_undo_redo_a_collection_replace()
        {
            IUndoRedoHandle handle = null;
            _contact.ModelChanged += (m, h) => { handle = h; };

            for(int i = 0; i < 3; i++) _contact.Numbers.Add(new PhoneNumber());

            Assert.That(_contact.Numbers, Has.Count(3));

            _contact.BeginEdit();

            var itemToReplace = _contact.Numbers[1];
            var replacement = new PhoneNumber();

            _contact.Numbers[1] = replacement;

            Assert.That(_contact.Numbers, Has.Count(3));
            Assert.That(_contact.Numbers[1], Is.EqualTo(replacement));

            handle.Undo();

            Assert.That(_contact.Numbers, Has.Count(3));
            Assert.That(_contact.Numbers[1], Is.EqualTo(itemToReplace));

            handle.Redo();

            Assert.That(_contact.Numbers, Has.Count(3));
            Assert.That(_contact.Numbers[1], Is.EqualTo(replacement));
        }

        [Test]
        public void can_undo_redo_a_collection_child_change_change()
        {
            const PhoneNumberType numberType = PhoneNumberType.Fax;

            IUndoRedoHandle handle = null;
            _contact.ModelChanged += (m, h) => { handle = h; };

            _contact.Numbers.Add(new PhoneNumber());

            _contact.BeginEdit();
            _contact.Numbers[0].Type = numberType;

            Assert.That(_contact.Numbers[0].Type, Is.EqualTo(numberType));

            handle.Undo();

            Assert.That(_contact.Numbers[0].Type, Is.Not.EqualTo(numberType));

            handle.Redo();

            Assert.That(_contact.Numbers[0].Type, Is.EqualTo(numberType));
        }
    }
}