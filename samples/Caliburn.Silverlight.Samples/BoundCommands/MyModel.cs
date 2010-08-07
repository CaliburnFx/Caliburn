namespace BoundCommands
{
    //Note: A basic presentation model that hosts a command instance.
    public class MyModel
    {
		private readonly MyCommand _theCommand = new MyCommand();
		private readonly MyWpfCommand _theWpfCommand = new MyWpfCommand();

		public MyCommand TheCommand
		{
			get { return _theCommand; }
		}

		public MyWpfCommand TheWpfCommand
		{
			get { return _theWpfCommand; }
		}
    }
}