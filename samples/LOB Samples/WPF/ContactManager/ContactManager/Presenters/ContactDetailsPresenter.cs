namespace ContactManager.Presenters
{
    using System;
    using System.ComponentModel;
    using Caliburn.ModelFramework;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Filters;
    using Caliburn.PresentationFramework.RoutedMessaging;
    using Caliburn.PresentationFramework.Screens;
    using Caliburn.WPF.ApplicationFramework;
    using Interfaces;
    using Model;
    using Web;

    public class ContactDetailsPresenter : Screen<Contact>, IContactDetailsPresenter
    {
        private void OnPropertyChangedEvent(object s, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "IsDirty" || e.PropertyName == "IsValid")
                NotifyOfPropertyChange(() => CanSave);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Subject.PropertyChanged += OnPropertyChangedEvent;

            Subject.BeginEdit();
            Subject.Validate();
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            Subject.PropertyChanged -= OnPropertyChangedEvent;
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            Subject.CancelEdit();
        }

        public bool CanSave
        {
            get { return Subject.IsDirty && Subject.IsValid; }
        }

        [Preview("CanSave")]
        public IResult Apply()
        {
            return SaveContact(x => Subject.BeginEdit());
        }

        [Preview("CanSave")]
        public IResult Ok()
        {
            return SaveContact(x => Close());
        }

        public void Cancel()
        {
            Close();
        }

        private IResult SaveContact(Action<AsyncCompletedEventArgs> callback)
        {
            Subject.EndEdit();
            var dto = Map.ToDto(Subject);

            if (dto.ID == Guid.Empty)
                dto.ID = Guid.NewGuid();

            return new WebServiceResult<ContactServiceClient, AsyncCompletedEventArgs>(
                x => x.UpdateContactAsync(dto),
                callback
                );
        }

        public void AddNumber()
        {
            Subject.AddPhoneNumber(new PhoneNumber());
        }

        public void RemoveNumber(PhoneNumber numberToRemove)
        {
            Subject.RemovePhoneNumber(numberToRemove);
        }

        public IResult ValidateContact()
        {
            return new ErrorResult(Subject.Validate());
        }

        public override bool CanShutdown()
        {
            return !Subject.IsDirty;
        }

        public ISubordinate CreateShutdownModel()
        {
            if (Subject.IsValid)
            {
                return new Question(
                    this,
                    string.Format(
                        "Contact '{0}' has not been saved.  Do you want to save before closing?",
                        (Subject.LastName ?? string.Empty) + ", " + (Subject.FirstName ?? string.Empty)
                        )
                    ) {Answer = Answer.Yes};
            }

            return new Question(
                this,
                string.Format(
                    "Contact '{0}' is invalid.  Changes will be lost.  Do you still want to close?",
                    (Subject.LastName ?? string.Empty) + ", " + (Subject.FirstName ?? string.Empty)
                    ),
                Answer.Yes, Answer.No
                );
        }

        public bool CanShutdown(ISubordinate shutdownModel)
        {
            var question = (Question)shutdownModel;

            if (Subject.IsValid)
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
    }
}