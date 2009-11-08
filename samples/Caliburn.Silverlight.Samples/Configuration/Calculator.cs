namespace Configuration
{
    using Caliburn.PresentationFramework.Filters;

    public class Calculator
    {
        //Note: Preview indicates something that will happen before execution of the action.
        //Note: If AffectsTriggers = false, then this filter will not effect the state of the UI in real time.
        [Preview("CanDivide")]
        //Note: The return value is bound to the UI if present.
        public int Divide(int left, int right)
        {
            return left / right;
        }

        //Note: See Preview filter.
        public bool CanDivide(int left, int right)
        {
            return right != 0;
        }
    }
}