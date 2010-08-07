namespace BoundCommands
{
	using System.Windows;
	using Caliburn.PresentationFramework.Filters;
	using System.Windows.Input;
	using System.Timers;


	public class MyWpfCommand : ICommand
	{
		Timer _timer;
		public MyWpfCommand()
		{
			_timer = new Timer(2000);
			_timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
			_timer.Start();
		}

		void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			_executionEnabled = !_executionEnabled;
			CanExecuteChanged(this, System.EventArgs.Empty);
		}

		public event System.EventHandler CanExecuteChanged = delegate { };
	
		bool _executionEnabled = true;
		public bool CanExecute(object parameter)
		{
			return parameter != null && !string.IsNullOrEmpty(parameter.ToString())
				&& _executionEnabled;
		}

		public void Execute(object parameter)
		{
			//Note: This is for demo purposes only.
			//Note: It is not a good practice to call MessageBox.Show from a non-View class.
			//Note: Consider implementing a MessageBoxService.
			MessageBox.Show((parameter ?? string.Empty).ToString());
		}

		
	}
}