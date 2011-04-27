using System.Collections.Generic;
using System.Linq;
using Implementation.Bank;

namespace Implementation
{
    public class Repository
    {
        public Repository()
        {
            Users = new List<User>();
            Fees = new List<PaymentFee>();
            Payments = new List<Payment>();
        }

        public List<User> Users { get; private set; }
        public List<PaymentFee> Fees { get; private set; }
        public List<Payment> Payments { get; private set; }

        public User FindUser(string phoneNumber)
        {
            return Users.Where(x => x.PhoneNumber == phoneNumber).SingleOrDefault();
        }

        public decimal GetPayerFee(PaymentType paymentType)
        {
            return Fees.Where(x => x.PaymentType == paymentType).Select(x => x.PayerFee).SingleOrDefault();
        }

        public decimal GetCollectorFee(PaymentType paymentType)
        {
            return Fees.Where(x => x.PaymentType == paymentType).Select(x => x.CollectorFee).SingleOrDefault();
        }
    }
}