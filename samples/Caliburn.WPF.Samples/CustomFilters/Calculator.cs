namespace CustomFilters
{
    using Caliburn.PresentationFramework.Filters;
	using System.Windows.Threading;
using System;

    public class Calculator
    {
		public Calculator() {
			
			SolarCell = new SolarCell();
		}

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




		//emulates a fake energy source
		public SolarCell SolarCell { get; set; }

		//precondition for "DoEnergyConsumingCalculation" execution
		public bool CanDoEnergyConsumingCalculation()
		{
			return SolarCell.CurrentCharge > 0.5;
		}

		//the filter hooks the event indicated by the "event path" and forces
		//an availability update of the triggers (re-evaluation of preconditions)
		//when the event is received
		[ChangedEvent("SolarCell.CurrentChargeChanged")]
		public void DoEnergyConsumingCalculation() {
			//consume the energy amount
			SolarCell.Consume(0.5);
		}
    }


	public class SolarCell {
	 
		DispatcherTimer _timer;
		bool _charging;
		public SolarCell()
		{
			CurrentCharge = 0;
			_charging = true;
			_timer = new DispatcherTimer();
			_timer.Interval = TimeSpan.FromSeconds(1);
			_timer.Tick += (o, e) => {
				CurrentCharge += _charging ? 0.2 : -0.1;
				if (CurrentCharge >= 1) _charging = false;
				if (CurrentCharge < 0.3) _charging = true;

				this.CurrentChargeChanged.Invoke(this, EventArgs.Empty); 
			};
			_timer.IsEnabled = true;
		}


		public double CurrentCharge { get; set; }

		public void Consume(double amount) {
			if (amount > CurrentCharge) throw new ArgumentException("Amount unavailable", "amount");
			CurrentCharge -= amount;
			CurrentChargeChanged.Invoke(this, EventArgs.Empty);
		}

		public event EventHandler CurrentChargeChanged = delegate { };
	}
}