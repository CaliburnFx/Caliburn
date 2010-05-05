namespace DependentActions
{
    using System.ComponentModel;
    using Caliburn.PresentationFramework.Filters;

    //Note: You must implement INotifyPropertyChanged to use dependent actions.
    //Note: Consider using Caliburn's PropertyChangedBase instead of implementing the interface yourself.
    public class Calculator : INotifyPropertyChanged
    {
        private double _left;
        private double _right;
        private double _result;

        public double Left
        {
            get { return _left; }
            set
            {
                _left = value;
                RaisePropertyChanged("Left");
            }
        }

        public double Right
        {
            get { return _right; }
            set
            {
                _right = value;
                RaisePropertyChanged("Right");
            }
        }

        public double Result
        {
            get { return _result; }
            set
            {
                _result = value;
                RaisePropertyChanged("Result");
            }
        }

        [Preview("CanDivide")]
        //Note: Actions can declare properties that they are dependent on.
        //Note: Changes in these properties will cause execution of pre-execute filters that affect triggers.
        [Dependencies("Left", "Right")]
        public void Divide()
        {
            Result = Left / Right;
        }

        public bool CanDivide()
        {
            return Right != 0 && Left != 0;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}