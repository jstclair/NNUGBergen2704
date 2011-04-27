using System;
using System.Collections.Generic;
using System.Threading;
using Implementation.Bank;

namespace Implementation.Sms
{
    public class SmsGateway
    {
        private readonly ApplicationQueue<PaymentCommand> commandQueue;
        private readonly ApplicationQueue<SmsMessage> incomingMessageQueue;
        private readonly ApplicationQueue<PaymentNotification> notificationQueue;
        private readonly ApplicationQueue<SmsMessage> outgoingMessageQueue;

        public SmsGateway(
            ApplicationQueue<SmsMessage> incomingMessageQueue,
            ApplicationQueue<SmsMessage> outgoingMessageQueue,
            ApplicationQueue<PaymentCommand> commandQueue,
            ApplicationQueue<PaymentNotification> notificationQueue)
        {
            this.incomingMessageQueue = incomingMessageQueue;
            this.outgoingMessageQueue = outgoingMessageQueue;
            this.commandQueue = commandQueue;
            this.notificationQueue = notificationQueue;

            this.incomingMessageQueue.SubscribeWithHandler(ReceiveMessage);
            this.notificationQueue.SubscribeWithHandler(Notify);
        }

        public void ReceiveMessage(SmsMessage message)
        {
            var parser = new SmsParser();
            PaymentCommand command = parser.Parse(message);
            ThreadPool.QueueUserWorkItem((x) => commandQueue.Enqueue(command));
        }

        public void SendMessage(SmsMessage message)
        {
            ThreadPool.QueueUserWorkItem((x) => outgoingMessageQueue.Enqueue(message));
        }

        public void Notify(PaymentNotification notification)
        {
            Dictionary<NotificationTopic, Func<PaymentNotification, string>> formatter = GetNotificationFormatter();
            string text = formatter[notification.Topic](notification);
            var message = new SmsMessage {Message = text, PhoneNumber = notification.PhoneNumber};
            SendMessage(message);
        }

        private static Dictionary<NotificationTopic, Func<PaymentNotification, string>> GetNotificationFormatter()
        {
            return new Dictionary<NotificationTopic, Func<PaymentNotification, string>>
                       {
                           {
                               NotificationTopic.PaymentSent, x => string.Format(
                                   "You paid {0} to {1}. Your new balance is {2}. Thank you for using InMemory Bank.",
                                   x.Command.Amount, x.Payment.Collector.PhoneNumber,
                                   x.Payment.Payer.Balance)
                               },
                           {
                               NotificationTopic.PaymentReceived, x => string.Format(
                                   "You received {0} from {1}. Your new balance is {2}. Thank you for using InMemory Bank.",
                                   x.Command.Amount, x.Payment.Payer.PhoneNumber, x.Payment.Collector.Balance)
                               },
                           {
                               NotificationTopic.PayerNotRegistered, x =>
                                                                     "In order to use InMemory Bank you need to register. Command is cancelled."
                               },
                           {
                               NotificationTopic.CollectorNotRegistered, x => string.Format(
                                   "You can not send money to unregistered user ({0}). Command is cancelled.",
                                   x.Command.CollectorNumber)
                               },
                           {
                               NotificationTopic.InsufficientFunds, x => string.Format(
                                   "Not enough funds to pay {0} to {1}. Your current balance is {2}. Command is cancelled.",
                                   x.Command.Amount, x.Command.CollectorNumber, x.Payment.Payer.Balance)
                               },
                       };
        }
    }
}