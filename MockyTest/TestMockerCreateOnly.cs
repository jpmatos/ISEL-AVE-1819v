using System;
using NUnit.Framework;

namespace Mocky.Test
{
    [TestFixture]
    public class TestMockerCreateOnly
    {
        readonly ICalculator calc;
        readonly IHttpRequest req;

        public TestMockerCreateOnly()
        {
            Mocker mockCalc = new Mocker(typeof(ICalculator));
            calc = (ICalculator)mockCalc.Create();
            Mocker mockReq = new Mocker(typeof(IHttpRequest));
            req = (IHttpRequest) mockReq.Create();
        }

        [Test]
        public void TestCalculatorTypeName()
        {
            string klassName = calc.ToString(); // OK ToString() inherited from Object
            Assert.AreEqual("MockICalculator", klassName);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestCalculatorAddNotImplemented()
        {
            calc.Add(2, 3);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestCalculatorSubNotImplemented()
        {
            calc.Sub(2, 3);
        }
        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestCalculatorMulNotImplemented()
        {
            calc.Mul(2, 3);
        }
        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestCalculatorDivNotImplemented()
        {
            calc.Div(2, 3);
        }

        [Test]
        public void TestRequestTypeName()
        {
            string klassName = req.ToString(); // OK ToString() inherited from Object
            Assert.AreEqual("MockIHttpRequest", klassName);
        }

        //This test will pass despite GetBody being implemented. This behaviour is intended.
        //It throws NotImplementedException because it does not find the value for "".
        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestRequestGetBodyNotImplemented()
        {
            req.GetBody("");
        }

        //This test will now fail as Dispose is now implemented and does a simple return.
        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestRequestDisposeNotImplemented()
        {
            req.Dispose();
        }
    }
}
