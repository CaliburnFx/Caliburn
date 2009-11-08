namespace CustomFilters
{
    using Caliburn.PresentationFramework.Filters;

    public class Calculator
    {
        //Note: Applied the custom pre-execute filter.  
        //Note: Highest priority executes first.
        [UserInRole("Admin", Priority = 1)]
        [Preview("CanDivide")]
        public int Divide(int left, int right)
        {
            return left / right;
        }

        public bool CanDivide(int left, int right)
        {
            return right != 0;
        }
    }
}