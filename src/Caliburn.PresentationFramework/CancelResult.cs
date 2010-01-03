namespace Caliburn.PresentationFramework
{
    using Core;

    /// <summary>
    /// Used by a child to cancel the enumeration of <see cref="IResult"/> instances.  
    /// This will not bubble up to the caller as an exception, but will simply halt enumeration.
    /// </summary>
    public class CancelResult : CaliburnException {}
}