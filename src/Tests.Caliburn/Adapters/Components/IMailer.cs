namespace Tests.Caliburn.Adapters.Components
{
    public interface IMailer
    {
        void SendMessage(string address, string message);
    }
}