using System;
using NUnit.Framework;

namespace Mocky.Test
{
    [TestFixture]
    public class TestMockerForPartialCalculator
    {
        readonly ICalculator calc;

        public TestMockerForPartialCalculator()
        {
            Mocker mock = new Mocker(typeof(ICalculator));
            mock.When("Add").With(5, 7).Return(12);
            mock.When("Add").With(3, 4).Return(7);
            mock.When("Mul").With(3, 3).Return(9);
            calc = (ICalculator)mock.Create();
//            MockMethod[] arr = {
//                new MockMethod(typeof(ICalculator), "Add"),
//                new MockMethod(typeof(ICalculator), "Mul")
//            };
//            arr[0].With(5, 7).Return(12);
//            arr[0].With(3, 4).Return(7);
//            arr[1].With(3, 3).Return(9);
//            calc = new Calculator(arr);
        }
        
        [Test]
        public void TestCalculatorSuccessfully()
        {
            Assert.AreEqual(calc.Add(5, 7), 12);
            Assert.AreEqual(calc.Add(3, 4), 7);
            Assert.AreEqual(calc.Add(4, 1), 0); // Returns 0 rather than 5 because that behavior was not defined for Add
            Assert.AreEqual(calc.Mul(3, 3), 9);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestCalculatorFailing()
        {
            Assert.AreEqual(calc.Sub(2, 1), 1); // NotImplementedException
        }
    }
}
