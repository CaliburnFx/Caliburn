namespace Caliburn.ShellFramework.Questions
{
    using System.Collections.Generic;
    using PresentationFramework.Screens;

    /// <summary>
    /// Represents a dialog capable of asking questions.
    /// </summary>
    public interface IQuestionDialog : IScreen
    {
        /// <summary>
        /// Prepares the dialog with a title and a series of questions.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="questions">The questions.</param>
        void Setup(string title, IEnumerable<Question> questions);
    }
}