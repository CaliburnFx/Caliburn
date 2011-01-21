namespace DelayedValidation.ViewModels {
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Caliburn.Core.InversionOfControl;
    using Caliburn.Core.Validation;
    using Caliburn.PresentationFramework.Screens;
    using Framework;

    [PerRequest(typeof(ContactViewModel))]
    public class ContactViewModel : Screen, IDataErrorInfo {
        readonly IValidator validator;
        string firstName;
        string lastName;
        Mode mode;
        string phoneNumber;

        public ContactViewModel(IValidator validator) {
            this.validator = validator;
        }

        public Mode Mode {
            get { return mode; }
            set {
                mode = value;
                NotifyOfPropertyChange(() => Mode);
            }
        }

        [Required]
        public string FirstName {
            get { return firstName; }
            set {
                firstName = value;
                NotifyOfPropertyChange(() => FirstName);
            }
        }

        [Required]
        public string LastName {
            get { return lastName; }
            set {
                lastName = value;
                NotifyOfPropertyChange(() => LastName);
            }
        }

        [RegularExpression(@"^\d{3}-\d{3}-\d{4}$"), Required]
        public string PhoneNumber {
            get { return phoneNumber; }
            set {
                phoneNumber = value;
                NotifyOfPropertyChange(() => PhoneNumber);
            }
        }

        public string this[string columnName] {
            get { return string.Join(Environment.NewLine, validator.Validate(this, columnName).Select(x => x.Message)); }
        }

        public string Error {
            get { return string.Join(Environment.NewLine, validator.Validate(this).Select(x => x.Message)); }
        }

        [ValidateFieldSet("Contact")]
        public void Save() {
            Mode = Mode.View;
        }

        public void Cancel() {
            if(!string.IsNullOrEmpty(Error))
                TryClose();

            NotifyOfPropertyChange(() => FirstName);
            NotifyOfPropertyChange(() => LastName);
            NotifyOfPropertyChange(() => PhoneNumber);
            Mode = Mode.View;
        }

        public void Edit() {
            Mode = Mode.Edit;
        }
    }
}