using AbcBooks.Utilities.Interfaces;

namespace AbcBooks.Utilities
{
	public class SmsSender : ISmsSender
    {
        public void SendSmsAsync(string number, string message)
        {
            //var accountSid = "ACca482f9adc5472a030f52f51d4ab4222";
            //var authToken = "076d21b95e3cca87afe92fcfa9c33d92";
            //TwilioClient.Init(accountSid, authToken);

            //var messageOptions = new CreateMessageOptions(
            //  new PhoneNumber("+919645766626"));
            //messageOptions.From = new PhoneNumber("+14705163845");
            //messageOptions.Body = message;

            //MessageResource.Create(messageOptions);
            //Console.WriteLine(message);
        }
    }
}
