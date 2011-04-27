namespace Implementation.Sms
{
    public class SmsMessage
    {
        public string PhoneNumber { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", PhoneNumber, Message);
        }

        public override bool Equals(object obj)
        {
            var message = obj as SmsMessage;
            if (message != null)
            {
                return Message == message.Message && PhoneNumber == message.PhoneNumber;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return PhoneNumber.GetHashCode() ^ Message.GetHashCode();
        }
    }
}