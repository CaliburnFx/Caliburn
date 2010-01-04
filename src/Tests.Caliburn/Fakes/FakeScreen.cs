namespace Tests.Caliburn.Fakes
{
    using System;
    using global::Caliburn.PresentationFramework.ApplicationModel;
    using global::Caliburn.PresentationFramework.Screens;

    public class FakeScreen : Screen, ISupportCustomShutdown
    {
        public bool CanShutdownWasCalled;
        public bool CustomCanShutdownResult;
        public bool CanShutdownResult;
        private readonly FakeShutdownModel ShutdownModel;

        public FakeScreen()
        {
            ShutdownModel = new FakeShutdownModel {Master = this};
        }

        public override bool CanShutdown()
        {
            return CanShutdownResult;
        }

        public ISubordinate CreateShutdownModel()
        {
            return ShutdownModel;
        }

        public bool CanShutdown(ISubordinate shutdownModel)
        {
            if(shutdownModel != ShutdownModel)
                throw new Exception();

            CanShutdownWasCalled = true;

            return CustomCanShutdownResult;
        }
    }
}