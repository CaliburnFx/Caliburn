namespace ContactManager.Presenters.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.Silverlight.ApplicationFramework;

    public interface IQuestionPresenter : IPresenter, ILifecycleNotifier
    {
        void Setup(IEnumerable<Question> questions, Action completed);
    }
}