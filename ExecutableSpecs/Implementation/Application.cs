using Implementation.Bank;
using Implementation.Sms;

namespace Implementation
{
    public sealed class Application
    {
        private static readonly Application instance = new Application();

        public readonly ApplicationQueue<PaymentCommand> CommandQueue = new ApplicationQueue<PaymentCommand>();

        public readonly ApplicationQueue<PaymentNotification> NotificationQueue =
            new ApplicationQueue<PaymentNotification>();

        public readonly ApplicationQueue<SmsMessage> ReceivedMessages = new ApplicationQueue<SmsMessage>();

        public readonly Repository Repository = new Repository();
        public readonly ApplicationQueue<SmsMessage> SentMessages = new ApplicationQueue<SmsMessage>();
        private CommandProcessor commandProcessor;
        private SmsGateway smsGateway;

        private Application()
        {
            smsGateway = new SmsGateway(ReceivedMessages, SentMessages, CommandQueue, NotificationQueue);
            commandProcessor = new CommandProcessor(CommandQueue, NotificationQueue, Repository);
        }

        public static Application Instance
        {
            get { return instance; }
        }
    }
}