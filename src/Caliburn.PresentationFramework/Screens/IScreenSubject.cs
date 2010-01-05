namespace Caliburn.PresentationFramework.Screens
{
    using System;
    using ViewModels;

    /// <summary>
    /// Matches screens with the same "subject" and is capable of creating screens for the encapsulated subject.
    /// </summary>
    public interface IScreenSubject
    {
        /// <summary>
        /// Determines if the specified screen matches this subject.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <returns><c>true</c> if the screen matches the subject; otherwise, <c>false</c>.</returns>
        bool Matches(IScreen screen);

        /// <summary>
        /// Creates the screen.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="constructionComplete">The construction completion callback.</param>
        void CreateScreen(IViewModelFactory factory, Action<IScreen> constructionComplete);
    }
}