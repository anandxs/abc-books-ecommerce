namespace AbcBooks.Utilities.Interfaces;

public interface ISmsSender
{
    void SendSmsAsync(string number, string message);
}
