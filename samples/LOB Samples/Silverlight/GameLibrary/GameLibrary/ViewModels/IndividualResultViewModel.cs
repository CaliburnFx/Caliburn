namespace GameLibrary.ViewModels
{
    using System.Collections.Generic;
    using Caliburn.PresentationFramework.RoutedMessaging;
    using Caliburn.PresentationFramework.Screens;
    using Caliburn.ShellFramework.Results;
    using Framework;
    using Model;

    public class IndividualResultViewModel : Screen
    {
        private readonly int _number;
        private readonly SearchResult _result;

        public IndividualResultViewModel(SearchResult result, int number)
        {
            _result = result;
            _number = number;
        }

        public int Number
        {
            get { return _number; }
        }

        public string Title
        {
            get { return _result.Title; }
        }

        public IEnumerable<IResult> Open()
        {
            var getGame = new GetGame
            {
                Id = _result.Id
            }.AsResult();

            yield return Show.Busy();
            yield return getGame;
            yield return Show.Child<ExploreGameViewModel>().In<IShell>()
                .Configured(x => x.WithGame(getGame.Response));
            yield return Show.NotBusy();
        }
    }
}