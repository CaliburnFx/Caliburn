namespace Caliburn.ShellFramework.Questions
{
    using System.Collections.Generic;
    using PresentationFramework.Screens;

    public interface IQuestionDialog : IScreen
    {
        void Setup(string caption, IEnumerable<Question> questions);
    }
}