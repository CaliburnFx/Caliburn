using Caliburn.PresentationFramework.RoutedMessaging;
using Caliburn.ShellFramework.Results;

namespace GameLibrary.ViewModels
{
    using System.ComponentModel.Composition;
    using Framework;

    [Export(typeof(NoResultsViewModel))]
    public class NoResultsViewModel
    {
        private string _searchText;

        public IResult AddGame()
        {
            return Show.Child<AddGameViewModel>()
                .In<IShell>()
                .Configured(x => x.Title = _searchText);
        }

        public NoResultsViewModel WithTitle(string searchText)
        {
            _searchText = searchText;
            return this;
        }
    }
}