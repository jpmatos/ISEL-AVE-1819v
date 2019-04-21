using NUnit.Framework;

namespace Mocky.Test
{
    [TestFixture]
    public class TestMockMethod
    {
        private MockMethod[] ms = null;

        [Test]
        public void TestMockMethodForAdd()
        {
            MockMethod add = new MockMethod(typeof(ICalculator), "Add");
            add.With(5, 3).Return(8);
            add.With(2, 7).Return(9);
            Assert.AreEqual(add.Call(5, 3), 8);
            Assert.AreEqual(add.Call(2, 7), 9);
            Assert.AreEqual(add.Call(4, 8), 0);
        }

        public int SimulateAddMockImplementatioToCheckIL(int a, int b) {
            return (int) ms[3].Call(a, b);
        }

        public string SimulateGetBodyMockImplementatioToCheckIL(string url)
        {
            return (string) ms[7].Call(url);
        }
    }
}
