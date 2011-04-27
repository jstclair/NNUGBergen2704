using System;
using System.Linq;
using System.Threading;
using Implementation;
using Implementation.Sms;
using NUnit.Framework;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Specifications.Steps
{
    [Binding]
    public class Steps
    {
        [Given(@"user with phone number (\w+) is not registered")]
        public void GivenUserWithPhoneNumberIsNotRegistered(string phoneNumber)
        {
            RepositoryHelper.EnsureUserDoesNotExist(phoneNumber);
        }

        [When(@"user sends SMS")]
        public void WhenUserSendsSMS(Table table)
        {
            table.CreateSet<SmsMessage>().ToList().ForEach(m => Application.Instance.ReceivedMessages.Enqueue(m));
            Thread.Sleep(100); // ZZzzzZZZ
        }

        [Then(@"following SMS should be sent")]
        public void ThenFollowingSMSShouldBeSent(Table table)
        {
            table.CreateSet<SmsMessage>().ToList()
                .ForEach(message => Assert.Contains(message, Application.Instance.SentMessages));
        }

        [Then(@"no SMS should be sent to (\w+)")]
        public void ThenNoSMSShouldBeSentToNumber(string phoneNumber)
        {
            Assert.False(Application.Instance.SentMessages.Any(m => m.PhoneNumber == phoneNumber));
        }


    }

    public static class RepositoryHelper
    {
        public static void EnsureUserDoesNotExist(string phoneNumber)
        {
            Application.Instance.Repository.Users
                .Where(u => u.PhoneNumber == phoneNumber).ToList()
                .ForEach(u => u.PhoneNumber = Guid.NewGuid().ToString());
        }
    }
}