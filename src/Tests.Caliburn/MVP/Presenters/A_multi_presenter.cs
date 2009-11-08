namespace Tests.Caliburn.MVP.Presenters
{
    using global::Caliburn.PresentationFramework.ApplicationModel;
    using NUnit.Framework;

    [TestFixture]
    public class A_multi_presenter : A_presenter
    {
        protected MultiPresenter _multiPresenter;

        protected override PresenterBase CreatePresenter()
        {
            return new MultiPresenter();
        }

        protected override void given_the_context_of()
        {
            base.given_the_context_of();

            _multiPresenter = (MultiPresenter)_presenter;
        }
    }
}