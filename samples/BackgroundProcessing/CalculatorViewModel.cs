namespace BackgroundProcessing
{
	using System.Threading;
	using Caliburn.PresentationFramework.Actions;
	using Caliburn.PresentationFramework.Filters;
	using Framework;

	public class CalculatorViewModel
	{
        public bool CanDivide(double left, double right) {
            return right != 0;
        }

        [Preview("CanDivide")]
        [AsyncAction(Callback = "DivideComplete", BlockInteraction = true)]
        //Note: The action is labeled with a title and registered into a collection of running actions during 
        //		its entire execution.
        //		By default it is marked with IsIndeterminate flag, meaning that the action has an indeterminate 
        //		duration and doesn't support progress notification; this way the RunningTaskView could display 
        //		an appropriate indeterminate progress indicator.
        [ActionInfo("Executing division...")]
        public double Divide(double left, double right) {
            Thread.Sleep(10000); //Don't ever call Thread.Sleep....it's just for demo purposes.
            return left / right;
        }

        public double DivideComplete(double result) {
            return result * 100;
        }

        public bool CanCalculateFactorial(int number) {
            return number > 0 && number <= 16;
        }

        [Preview("CanCalculateFactorial")]
        [AsyncAction(BlockInteraction = false)]
        //Note: The action is labeled with a title and registered into a collection of running actions during 
        //		its entire execution. 
        //		In addiction it is marked as supporting progress notification (IsIndeterminate = false), meaning
        //		that the action can provide information about its work progress; this allows the RunningTaskView 
        //		to display the correct progress indicator based on the notifications.
        //		Setting IsCancellable=true allows RunningActionsView to enable an appropriate control to
        //		cancel operation
        [ActionInfo("Calculating factorial...", IsIndeterminate = false, IsCancellable = true)]
        public int Factorial(int number) {
            double stepPercentage = 1.0 / number;
            int result = 1;

            for(int i = 1; i <= number; ++i) {
                if(RunningAction.Current.CancellationPending) {
                    RunningAction.Current.Title = "Cancelling...";
                    return 0;
                }

                result = result * i;

                //notifies the completed percentage of the work
                RunningAction.Current.CurrentPercentage = stepPercentage * i;

                //changing title...
                if(number > 6 && i >= number - 2)
                    RunningAction.Current.Title = "Almost done...";

                Thread.Sleep(1000);
            }

            return result;
        }

        [AsyncAction(BlockInteraction = true)]
        [ActionInfo("Executing very long action", IsIndeterminate = false, IsCancellable = true)]
        public void VeryLongAction() {
            for(int i = 1; i <= 1000; ++i) {
                if(RunningAction.Current.CancellationPending)
                    return;

                RunningAction.Current.CurrentPercentage = i / 1000.0;
                Thread.Sleep(10);
            }
        }
	}
}