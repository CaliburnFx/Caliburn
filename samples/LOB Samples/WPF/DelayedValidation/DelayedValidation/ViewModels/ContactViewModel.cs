namespace DelayedValidation.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Caliburn.Core.IoC;
    using Caliburn.PresentationFramework.Screens;
    using Caliburn.PresentationFramework.ViewModels;
    using Framework;
    using Model;

    [PerRequest(typeof(ContactViewModel))]
    public class ContactViewModel : Screen, IDataErrorInfo
    {
        readonly IValidator _validator;
        string _firstName;
        string _lastName;
        Mode _mode;
        string _phoneNumber;

        public ContactViewModel(IValidator validator)
        {
            _validator = validator;
        }

        public Mode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                NotifyOfPropertyChange(() => Mode);
            }
        }

        [Required]
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                NotifyOfPropertyChange(() => FirstName);
            }
        }

        [Required]
        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                NotifyOfPropertyChange(() => LastName);
            }
        }

        [RegularExpression(@"^\d{3}-\d{3}-\d{4}$"), Required]
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                _phoneNumber = value;
                NotifyOfPropertyChange(() => PhoneNumber);
            }
        }

        [ValidateFieldSet("Contact")]
        public void Save()
        {
            Mode = Mode.View;
        }

        public void Cancel()
        {
            if(!string.IsNullOrEmpty(Error))
                Close();

            NotifyOfPropertyChange(() => FirstName);
            NotifyOfPropertyChange(() => LastName);
            NotifyOfPropertyChange(() => PhoneNumber);
            Mode = Mode.View;
        }

        public void Edit()
        {
            Mode = Mode.Edit;
        }

        public string this[string columnName]
        {
            get { return string.Join(Environment.NewLine, _validator.Validate(this, columnName).Select(x => x.Message)); }
        }

        public string Error
        {
            get { return string.Join(Environment.NewLine, _validator.Validate(this).Select(x => x.Message)); }
        }
    }
}