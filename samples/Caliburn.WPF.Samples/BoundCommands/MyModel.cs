namespace BoundCommands
{
    //Note: A basic presentation model that hosts a command instance.
    public class MyModel
    {
        private readonly MyCommand _theCommand = new MyCommand();

        public MyCommand TheCommand
        {
            get { return _theCommand; }
        }
    }
}