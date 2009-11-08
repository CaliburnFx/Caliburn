namespace Tests.Caliburn.Fakes
{
    using System;
    using global::Caliburn.PresentationFramework.ApplicationModel;

    public class FakePresenter : Presenter, ISupportCustomShutdown
    {
        public bool CanShutdownWasCalled;
        public bool CustomCanShutdownResult;
        public bool CanShutdownResult;
        private readonly FakeShutdownModel ShutdownModel;

        public FakePresenter()
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