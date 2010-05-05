namespace Actions
{
    using System;
    using System.Windows;
    using Caliburn.PresentationFramework.Filters;

    //Note: This rescue will catch exceptions thrown by any action on this class.
    [Rescue("GeneralRescue")]
    public class Calculator
    {
        //Note: This rescue catches exceptions thrown by this method.
        [Rescue("ActionSpecificRescue")]
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

        //Note: See class level rescue filter.
        public void GeneralRescue(Exception ex)
        {
            //Note: This is for demo purposes only.
            //Note: It is not a good practice to call MessageBox.Show from a non-View class.
            //Note: Consider implementing a MessageBoxService.
            MessageBox.Show(ex.Message);
        }

        //Note: See rescue filter on Divide method.
        public void ActionSpecificRescue(Exception ex)
        {
            //Note: This is for demo purposes only.
            //Note: It is not a good practice to call MessageBox.Show from a non-View class.
            //Note: Consider implementing a MessageBoxService.
            MessageBox.Show("Divide Action: " + ex.Message);
        }
    }
}