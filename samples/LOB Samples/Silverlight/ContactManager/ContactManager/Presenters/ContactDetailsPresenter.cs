namespace ContactManager.Presenters
{
    using System;
    using System.ComponentModel;
    using Caliburn.Core.IoC;
    using Caliburn.ModelFramework;
    using Caliburn.PresentationFramework;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Filters;
    using Caliburn.Silverlight.ApplicationFramework;
    using Interfaces;
    using Model;
    using Web;

    [PerRequest(typeof(IContactDetailsPresenter))]
    public class ContactDetailsPresenter : Presenter, IContactDetailsPresenter
    {
        private Contact _contact;
        private IPresenterManager _owner;

        public Contact Contact
        {
            get { return _contact; }
            private set
            {
                _contact = value;
                NotifyOfPropertyChange("Contact");
            }
        }

        public void Setup(IPresenterManager owner, Contact contact)
        {
            _owner = owner;
            Contact = contact;
        }

        private void OnPropertyChangedEvent(object s, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "IsDirty" || e.PropertyName == "IsValid")
                NotifyOfPropertyChange("CanSave");
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Contact.BeginEdit();
            Contact.Validate();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Contact.PropertyChanged += OnPropertyChangedEvent;
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            Contact.PropertyChanged -= OnPropertyChangedEvent;
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            Contact.CancelEdit();
        }

        [Dependencies("CanSave")]
        [Preview("CanSave")]
        public IResult Apply()
        {
            return SaveContact(x => Contact.BeginEdit());
        }

        [Dependencies("CanSave")]
        [Preview("CanSave")]
        public IResult Ok()
        {
            return SaveContact(x => _owner.Shutdown(this));
        }

        public bool CanSave()
        {
            return Contact.IsDirty && Contact.IsValid;
        }

        public void Cancel()
        {
            _owner.Shutdown(this);
        }

        private IResult SaveContact(Action<AsyncCompletedEventArgs> callback)
        {
            Contact.EndEdit();
            var dto = Map.ToDto(Contact);

            if (dto.ID == Guid.Empty)
                dto.ID = Guid.NewGuid();

            return new WebServiceResult<ContactServiceClient, AsyncCompletedEventArgs>(
                x => x.UpdateContactAsync(dto),
                callback
                );
        }

        public void AddNumber()
        {
            Contact.AddPhoneNumber(new PhoneNumber());
        }

        public void RemoveNumber(PhoneNumber numberToRemove)
        {
            Contact.RemovePhoneNumber(numberToRemove);
        }

        public IResult ValidateContact()
        {
            return new ErrorResult(Contact.Validate());
        }

        public override bool CanShutdown()
        {
            return !Contact.IsDirty;
        }

        public ISubordinate CreateShutdownModel()
        {
            if(Contact.IsValid)
            {
                return new Question(
                    this,
                    string.Format(
                        "Contact '{0}' has not been saved.  Do you want to save before closing?",
                        (Contact.LastName ?? string.Empty) + ", " + (Contact.FirstName ?? string.Empty)
                        )
                    ) {Answer = Answer.Yes};
            }

            return new Question(
                this,
                string.Format(
                    "Contact '{0}' is invalid.  Changes will be lost.  Do you still want to close?",
                    (Contact.LastName ?? string.Empty) + ", " + (Contact.FirstName ?? string.Empty)
                    ),
                Answer.Yes, Answer.No
                );
        }

        public bool CanShutdown(ISubordinate shutdownModel)
        {
            var question = (Question)shutdownModel;

            if(Contact.IsValid)
            {
                if(question.Answer == Answer.Cancel)
                    return false;

                if (question.Answer == Answer.Yes)
                    Apply().Execute();

                return true;
            }

            if(question.Answer == Answer.Yes)
                return true;

            return false;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ContactDetailsPresenter;
            return other != null && other.Contact == Contact;
        }
    }
}