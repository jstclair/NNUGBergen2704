using System;
using System.Threading;

namespace Implementation.Bank
{
    public class CommandProcessor
    {
        private readonly ApplicationQueue<PaymentCommand> commandQueue;
        private readonly ApplicationQueue<PaymentNotification> notificationQueue;
        private readonly Repository repository;

        public CommandProcessor(
            ApplicationQueue<PaymentCommand> commandQueue,
            ApplicationQueue<PaymentNotification> notificationQueue,
            Repository repository)
        {
            this.commandQueue = commandQueue;
            this.notificationQueue = notificationQueue;
            this.repository = repository;

            this.commandQueue.SubscribeWithHandler(ProcessCommand);
        }

        public void ProcessCommand(PaymentCommand command)
        {
            Payment payment = null;
            try
            {
                User payer = repository.FindUser(command.PayerNumber);
                if (payer == null)
                    throw new PayerNotRegisteredException();

                User collector = repository.FindUser(command.CollectorNumber);
                if (collector == null)
                    throw new CollectorNotRegisteredException();

                payment = new Payment
                              {
                                  TransferTime = DateTime.Now,
                                  PaymentType = command.PaymentType,
                                  Amount = command.Amount,
                                  Payer = payer,
                                  Collector = collector,
                                  PayerFee = repository.GetPayerFee(command.PaymentType),
                                  CollectorFee = repository.GetCollectorFee(command.PaymentType)
                              };

                lock (repository.Payments)
                {
                    RegisterPayment(payment);
                }

                NotifyUser(payer.PhoneNumber, command, payment, NotificationTopic.PaymentSent);
                NotifyUser(collector.PhoneNumber, command, payment, NotificationTopic.PaymentReceived);
            }
            catch (PayerNotRegisteredException)
            {
                NotifyUser(command.PayerNumber, command, null, NotificationTopic.PayerNotRegistered);
            }
            catch (CollectorNotRegisteredException)
            {
                NotifyUser(command.PayerNumber, command, null, NotificationTopic.CollectorNotRegistered);
            }
            catch (InsufficientFundsException)
            {
                NotifyUser(command.PayerNumber, command, payment, NotificationTopic.InsufficientFunds);
            }
        }

        private void RegisterPayment(Payment payment)
        {
            decimal newPayerBalance = payment.Payer.Balance - payment.Amount - payment.PayerFee;
            if (newPayerBalance < 0)
                throw new InsufficientFundsException();

            payment.Payer.Balance = newPayerBalance;
            payment.Collector.Balance += payment.Amount - payment.CollectorFee;
            repository.Payments.Add(payment);
        }

        private void NotifyUser(string phoneNumber, PaymentCommand command, Payment payment, NotificationTopic topic)
        {
            ThreadPool.QueueUserWorkItem((x) => notificationQueue.Enqueue(new PaymentNotification
                                                                              {
                                                                                  PhoneNumber = phoneNumber,
                                                                                  Topic = topic,
                                                                                  Command = command,
                                                                                  Payment = payment,
                                                                              }));
        }
    }
}