namespace Tests.Caliburn.Fakes
{
    using System;
    using global::Caliburn.PresentationFramework.Screens;

    public class FakeScreen : Screen
    {
        public bool CanCloseWasCalled;
        public bool CanCloseResult;

        public override void CanClose(Action<bool> callback)
        {
            CanCloseWasCalled = true;
            callback(CanCloseResult);
        }
    }
}