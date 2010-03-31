namespace GameLibrary.ViewModels
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.ComponentModel.DataAnnotations;
    using System.Windows;
    using Caliburn.PresentationFramework.RoutedMessaging;
    using Caliburn.PresentationFramework.Screens;
    using Caliburn.PresentationFramework.ViewModels;
    using Caliburn.ShellFramework.Results;
    using Framework;
    using Model;
    using MessageBoxResult = System.Windows.MessageBoxResult;

    [Export(typeof(AddGameViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AddGameViewModel : Screen, IDataErrorInfo
    {
        private readonly IValidator _validator; //NOTE: You could also achieve validation without implementing the IDataErrorInfo interface by using Caliburn's AOP support.
        private string _notes;
        private double _rating;
        private string _title;
        private bool _wasSaved;

        [ImportingConstructor]
        public AddGameViewModel(IValidator validator)
        {
            _validator = validator;
        }

        [Required]
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyOfPropertyChange(() => Title);
                NotifyOfPropertyChange(() => CanAddGame);
            }
        }

        public string Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                NotifyOfPropertyChange(() => Notes);
            }
        }

        public double Rating
        {
            get { return _rating; }
            set
            {
                _rating = value;
                NotifyOfPropertyChange(() => Rating);
            }
        }

        public bool CanAddGame
        {
            get { return string.IsNullOrEmpty(Error); }
        }

        public IEnumerable<IResult> AddGame()
        {
            var add = new AddGameToLibrary
            {
                Title = Title,
                Notes = Notes,
                Rating = Rating
            }.AsResult();

            _wasSaved = true;

            yield return add;
            yield return Show.Child<SearchViewModel>().In<IShell>();
        }

        public override bool CanShutdown()
        {
            return _wasSaved || MessageBox.Show(
                "Are you sure you want to cancel?  Changes will be lost.",
                "Unsaved Changes",
                MessageBoxButton.OKCancel
                ) == MessageBoxResult.OK;
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