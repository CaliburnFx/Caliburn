namespace ContactManager.Presenters
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Core.IoC;
    using Caliburn.PresentationFramework.Screens;
    using Caliburn.WPF.ApplicationFramework;
    using Interfaces;

    [PerRequest(typeof(IQuestionPresenter))]
    public class QuestionPresenter : Screen, IQuestionPresenter
    {
        private Action _completed;
        private IEnumerable<Question> _questions;

        public IEnumerable<Question> Questions
        {
            get { return _questions; }
        }

        public void Setup(IEnumerable<Question> questions, Action completed)
        {
            _completed = completed;
            _questions = questions;
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();

            _completed();
        }
    }
}