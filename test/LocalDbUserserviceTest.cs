using NUnit.Framework;
using user.Services;

namespace test
{
    [TestFixture]
    public class LocalDbUserserviceTest
    {
        private AuthService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new AuthService(new LocalInMemoryDbContextFactory().GetUserContext());
        }

        [Test]
        public void GetAllUsers()
        {
            //
            var result = _service.GetAllUsers().GetAwaiter().GetResult();

            foreach (var item in result)
            {
                System.Console.WriteLine(item.Name);

            }

            Assert.That(result, Is.Not.Null);

        }

    }
}
