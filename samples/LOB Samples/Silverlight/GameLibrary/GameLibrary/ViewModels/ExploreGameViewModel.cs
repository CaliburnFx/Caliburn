namespace GameLibrary.ViewModels
{
    using System.ComponentModel.Composition;
    using Caliburn.PresentationFramework.RoutedMessaging;
    using Caliburn.PresentationFramework.Screens;
    using Model;

    [Export(typeof(ExploreGameViewModel))]
    public class ExploreGameViewModel : Screen
    {
        private string _borrower;
        private GameDTO _game;

        public GameDTO Game
        {
            get { return _game; }
        }

        public string BorrowedMessage
        {
            get { return Game.Title + " is currently checked out to " + Game.Borrower + "."; }
        }

        public string Borrower
        {
            get { return _borrower; }
            set
            {
                _borrower = value;
                NotifyOfPropertyChange(() => Borrower);
                NotifyOfPropertyChange(() => CanCheckOut);
            }
        }

        public bool IsCheckedOut
        {
            get { return !string.IsNullOrEmpty(_game.Borrower); }
        }

        public bool IsCheckedIn
        {
            get { return !IsCheckedOut; }
        }

        public bool CanCheckOut
        {
            get { return !string.IsNullOrEmpty(Borrower); }
        }

        public void WithGame(GameDTO game)
        {
            _game = game;
            Borrower = game.Borrower ?? string.Empty;
        }

        public IResult CheckIn()
        {
            SetBorrower(null);

            return new CheckGameIn
            {
                Id = _game.Id
            }.AsResult();
        }

        public IResult CheckOut()
        {
            SetBorrower(Borrower);

            return new CheckGameOut
            {
                Id = _game.Id,
                Borrower = Borrower
            }.AsResult();
        }

        private void SetBorrower(string borrower)
        {
            _game.Borrower = borrower;
            Borrower = borrower;

            NotifyOfPropertyChange(() => IsCheckedOut);
            NotifyOfPropertyChange(() => IsCheckedIn);
            NotifyOfPropertyChange(() => BorrowedMessage);
        }
    }
}