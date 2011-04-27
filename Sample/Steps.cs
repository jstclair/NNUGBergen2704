using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Sample
{
    [Binding]
    internal class Steps
    {
        private readonly Calculator _calc = new Calculator();


        [Given(@"I have entered (\d+) into the calculator")]
        public void GivenIHaveEnteredANumberIntoTheCalculator(decimal number)
        {
            _calc.Push(number);
        }

        [When(@"I press add")]
        public void WhenIPressAdd()
        {
            _calc.Add();
        }

        [Then(@"the result should be (\d+) on the screen")]
        public void ThenTheResultShouldBeANumberOnTheScreen(decimal number)
        {
            Assert.That(_calc.Peek(), Is.EqualTo(number));
        }
    }

    internal class Calculator : Stack<decimal>
    {
        public void Add()
        {
            var result =  this.ToArray().Sum();
            this.Push(result);
        }
    }
}