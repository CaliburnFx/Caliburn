namespace Caliburn.Testability
{
    using System.Collections.Generic;
    using PresentationFramework.RoutedMessaging;

    /// <summary>
    /// Enumerates <see cref="IResult"/> instances without calling execute or waiting for completion.
    /// </summary>
    public class TestResultEnumerator
    {
        private readonly IEnumerator<IResult> _enumerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResultEnumerator"/> class.
        /// </summary>
        /// <param name="enumerator">The enumerator.</param>
        public TestResultEnumerator(IEnumerable<IResult> enumerator)
        {
            _enumerator = enumerator.GetEnumerator();
        }

        /// <summary>
        /// Returns the next <see cref="IResult"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Next<T>()
            where T : IResult
        {
            if(_enumerator.MoveNext())
                return (T)_enumerator.Current;
            return default(T);
        }
    }
}