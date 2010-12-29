namespace Caliburn.PresentationFramework.Screens
{
    using System;
    using ViewModels;

    /// <summary>
    /// Matches screens with the same "subject" and is capable of creating screens for the encapsulated subject.
    /// </summary>
    public interface ISubjectSpecification
    {
        /// <summary>
        /// Determines if the hosts' subjects match.
        /// </summary>
        /// <param name="iHaveSubject">The item which has a subject.</param>
        /// <returns><c>true</c> if the item's subject matches; otherwise, <c>false</c>.</returns>
        bool Matches(IHaveSubject iHaveSubject);

        /// <summary>
        /// Creates an item capable of hosting the subject.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="constructionComplete">The construction completion callback.</param>
        void CreateSubjectHost(IViewModelFactory factory, Action<IHaveSubject> constructionComplete);
    }
}