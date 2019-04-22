using NUnit.Framework;

namespace Mocky.Test
{
    [TestFixture]
    public class TestMockerForPartialHttpRequest
    {
        private readonly IHttpRequest req;

        public TestMockerForPartialHttpRequest()
        {
            Mocker mock = new Mocker(typeof(IHttpRequest));
            mock.When("GetBody").With("Hello").Return("World");
            mock.When("GetBody").With("ISEL").Return("AVE");
            req = (IHttpRequest) mock.Create();
        }

        [Test]
        public void TestHtttpRequestSuccessfully()
        {
            Assert.AreEqual(req.GetBody("Hello"), "World");
            Assert.AreEqual(req.GetBody("ISEL"), "AVE");
        }
    }
}