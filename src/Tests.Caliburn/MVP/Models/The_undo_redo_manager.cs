namespace Tests.Caliburn.MVP.Models
{
    using Fakes.ViewModel;
    using global::Caliburn.ModelFramework;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class The_undo_redo_manager : TestBase
    {
        private Contact _contact1;
        private Contact _contact2;
        private UndoRedoManager _manager;

        private const string lastName = "Eisenberg";
        private const string firstName = "Rob";
        private const string state = "Florida";

        protected override void given_the_context_of()
        {
            _contact1 = new Contact();
            _contact2 = new Contact();
            _manager = new UndoRedoManager();
        }

        [Test]
        public void can_undo_changes_on_a_single_instance()
        {
            _manager.Register(_contact1);
            _contact1.BeginEdit();

            _contact1.FirstName = firstName;
            _contact1.LastName = lastName;
            _contact1.Address.State = state;

            _manager.Undo();

            Assert.That(_contact1.Address.State, Is.Null);
            Assert.That(_contact1.LastName, Is.EqualTo(lastName));
            Assert.That(_contact1.FirstName, Is.EqualTo(firstName));

            _manager.Undo();

            Assert.That(_contact1.Address.State, Is.Null);
            Assert.That(_contact1.LastName, Is.Null);
            Assert.That(_contact1.FirstName, Is.EqualTo(firstName));

            _manager.Undo();

            Assert.That(_contact1.Address.State, Is.Null);
            Assert.That(_contact1.LastName, Is.Null);
            Assert.That(_contact1.FirstName, Is.Null);
        }

        [Test]
        public void can_redo_changes_on_a_single_instance()
        {
            _manager.Register(_contact1);
            _contact1.BeginEdit();

            _contact1.FirstName = firstName;
            _contact1.LastName = lastName;
            _contact1.Address.State = state;

            _manager.Undo();
            _manager.Undo();
            _manager.Undo();

            _manager.Redo();

            Assert.That(_contact1.Address.State, Is.Null);
            Assert.That(_contact1.LastName, Is.Null);
            Assert.That(_contact1.FirstName, Is.EqualTo(firstName));

            _manager.Redo();

            Assert.That(_contact1.Address.State, Is.Null);
            Assert.That(_contact1.LastName, Is.EqualTo(lastName));
            Assert.That(_contact1.FirstName, Is.EqualTo(firstName));

            _manager.Redo();

            Assert.That(_contact1.Address.State, Is.EqualTo(state));
            Assert.That(_contact1.LastName, Is.EqualTo(lastName));
            Assert.That(_contact1.FirstName, Is.EqualTo(firstName));
        }

        [Test]
        public void can_undo_changes_on_multiple_instances()
        {
            _manager.Register(_contact1);
            _manager.Register(_contact2);
            _contact1.BeginEdit();
            _contact2.BeginEdit();

            _contact1.FirstName = firstName;
            _contact2.LastName = lastName;
            _contact1.Address.State = state;
            _contact2.Numbers.Add(new PhoneNumber());

            _manager.Undo();

            Assert.That(_contact1.FirstName, Is.EqualTo(firstName));
            Assert.That(_contact2.LastName, Is.EqualTo(lastName));
            Assert.That(_contact1.Address.State, Is.EqualTo(state));
            Assert.That(_contact2.Numbers, Has.Count(0));

            _manager.Undo();

            Assert.That(_contact1.FirstName, Is.EqualTo(firstName));
            Assert.That(_contact2.LastName, Is.EqualTo(lastName));
            Assert.That(_contact1.Address.State, Is.Null);
            Assert.That(_contact2.Numbers, Has.Count(0));

            _manager.Undo();

            Assert.That(_contact1.FirstName, Is.EqualTo(firstName));
            Assert.That(_contact2.LastName, Is.Null);
            Assert.That(_contact1.Address.State, Is.Null);
            Assert.That(_contact2.Numbers, Has.Count(0));

            _manager.Undo();

            Assert.That(_contact1.FirstName, Is.Null);
            Assert.That(_contact2.LastName, Is.Null);
            Assert.That(_contact1.Address.State, Is.Null);
            Assert.That(_contact2.Numbers, Has.Count(0));
        }

        [Test]
        public void can_redo_changes_on_multiple_instances()
        {
            _manager.Register(_contact1);
            _manager.Register(_contact2);
            _contact1.BeginEdit();
            _contact2.BeginEdit();

            _contact1.FirstName = firstName;
            _contact2.LastName = lastName;
            _contact1.Address.State = state;
            _contact2.Numbers.Add(new PhoneNumber());

            _manager.Undo();
            _manager.Undo();
            _manager.Undo();
            _manager.Undo();

            _manager.Redo();

            Assert.That(_contact1.FirstName, Is.EqualTo(firstName));
            Assert.That(_contact2.LastName, Is.Null);
            Assert.That(_contact1.Address.State, Is.Null);
            Assert.That(_contact2.Numbers, Has.Count(0));

            _manager.Redo();

            Assert.That(_contact1.FirstName, Is.EqualTo(firstName));
            Assert.That(_contact2.LastName, Is.EqualTo(lastName));
            Assert.That(_contact1.Address.State, Is.Null);
            Assert.That(_contact2.Numbers, Has.Count(0));

            _manager.Redo();

            Assert.That(_contact1.FirstName, Is.EqualTo(firstName));
            Assert.That(_contact2.LastName, Is.EqualTo(lastName));
            Assert.That(_contact1.Address.State, Is.EqualTo(state));
            Assert.That(_contact2.Numbers, Has.Count(0));

            _manager.Redo();

            Assert.That(_contact1.FirstName, Is.EqualTo(firstName));
            Assert.That(_contact2.LastName, Is.EqualTo(lastName));
            Assert.That(_contact1.Address.State, Is.EqualTo(state));
            Assert.That(_contact2.Numbers, Has.Count(1));
        }

        [Test]
        public void indicates_when_undo_is_available()
        {
            _manager.Register(_contact1);
            _contact1.BeginEdit();

            bool canUndoWasRaised = false;

            _manager.PropertyChanged += (s, e) => { if(e.PropertyName == "CanUndo") canUndoWasRaised = true; };

            Assert.That(_manager.CanUndo, Is.False);

            _contact1.FirstName = firstName;

            Assert.That(_manager.CanUndo);
            Assert.That(canUndoWasRaised);
        }

        [Test]
        public void indicates_when_redo_is_available()
        {
            _manager.Register(_contact1);
            _contact1.BeginEdit();

            _contact1.FirstName = firstName;

            bool canRedoWasRaised = false;
            _manager.PropertyChanged += (s, e) => { if(e.PropertyName == "CanRedo") canRedoWasRaised = true; };

            Assert.That(_manager.CanRedo, Is.False);

            _manager.Undo();

            Assert.That(_manager.CanRedo);
            Assert.That(canRedoWasRaised);
        }

        [Test]
        public void can_clear_history()
        {
            _manager.Register(_contact1);
            _contact1.BeginEdit();

            _contact1.FirstName = firstName;
            _contact1.LastName = lastName;
            _contact1.Address.State = state;

            _manager.Clear();

            Assert.That(_manager.CanUndo, Is.False);
            Assert.That(_manager.CanRedo, Is.False);
        }
    }
}