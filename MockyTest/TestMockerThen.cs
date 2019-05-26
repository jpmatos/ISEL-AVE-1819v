using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Mocky.Test
{
    [TestFixture]
    public class TestMockerThen
    {
        private Mocker mockCalc;
        private Mocker mockReq;

        public TestMockerThen()
        {
            mockCalc = new Mocker(typeof(ICalculator));
            mockReq = new Mocker(typeof(IHttpRequest));
        }

        [Test]
        public void TestAddDelegate()
        {
            mockCalc.When("Add").Then<int, int, int>((a, b) => a + b);
            ICalculator calc = (ICalculator) mockCalc.Create();
            int res = calc.Add(5, 7);
            Assert.AreEqual(12, res);
        }

        [Test]
        public void TestGetBodyDelegate()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("url", "body");
            
            mockReq.When("GetBody").Then<string, string>(url =>
            {
                dic.TryGetValue(url, out string value);
                return value;
            });
            IHttpRequest req = (IHttpRequest) mockReq.Create();
            string body = req.GetBody("url");
            Assert.AreEqual("body", body);
        }
    }
}