namespace AsyncActions
{
    using System.Threading;
    using Caliburn.PresentationFramework.Actions;
    using Caliburn.PresentationFramework.Filters;

    public class Calculator
    {
        public bool CanDivide(double left, double right)
        {
            return right != 0;
        }

        //Note: Preview indicates something that will happen before execution of the action (on UI thread).
        //Note: If AffectsTriggers = false, then this filter will not effect the state of the UI in real time.
        [Preview("CanDivide")]
        //Note: This action will be executed asynchronously.
        //Note: If  BlockInteraction = true, the UI that fires this action will not be available until the async operation is complete.
        //Note: Callback will be called on the UI thread.  Return values are passed in the parameters.
        [AsyncAction(Callback = "DivideComplete", BlockInteraction = true)]
        public double Divide(double left, double right)
        {
            Thread.Sleep(3000);
            return left / right;
        }

        //Note: Executed on the UI thread.  See AsyncAction above.
        //Note: The return value is bound to the UI if present.
        public double DivideComplete(double result)
        {
            return result * 100;
        }
    }
}