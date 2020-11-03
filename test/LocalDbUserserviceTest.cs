using System.Linq;
using NUnit.Framework;
using user.Models;
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
            _service = new AuthService(new LocalInMemoryDbContextFactory().GetUserContext(), ConfigurationManager.AppSetting);
        }

        [TestCase(5, 1)]
        [TestCase(10, 1)]
        [TestCase(8, 2)]
        public void GetAllUsers(int pageSize, int page)
        {
            //
            var result = _service.GetAllUsers(pageSize, page).GetAwaiter().GetResult();
            Assert.That(result.Count() == pageSize);

            int i = 1;
            foreach (var item in result)
            {
                System.Console.WriteLine($"{i++}  :  Name: {item.Name}, Id: {item.Id}, IsActive: {item.IsActive} ");
            }
        }

        [TestCase()]
        public void GetAllUsers_Default()
        {
            var result = _service.GetAllUsers().GetAwaiter().GetResult();
            Assert.That(result.Count() == 5);

            int i = 1;
            foreach (var item in result)
            {
                System.Console.WriteLine($"{i++}  :  Name: {item.Name}, Id: {item.Id}, IsActive: {item.IsActive} ");
            }
        }

        [TestCase("sis@user.com", true)]
        [TestCase("Sis@user.com", true)]
        [TestCase("Sis@xxer.com", false)]
        public void GetUserByEmail(string email, bool expectedValue)
        {
            var result = _service.GetUserByEmail(email).GetAwaiter().GetResult();
            Assert.That(result !=null, Is.EqualTo(expectedValue));

            System.Console.WriteLine($"{email} : {result?.Email}");
        }

        [TestCase("sis@user.com", true)]
        [TestCase("Sis@user.com", true)]
        [TestCase("Sis@xxer.com", false)]
        public void IsUserExisted(string email, bool expectedValue)
        {
            var result = _service.IsUserExisted(email).GetAwaiter().GetResult();
            Assert.That(result == expectedValue);

            System.Console.WriteLine($"{email} : {result}");
        }

        [TestCase("sis@user.com")]
        public void CreateToken(string email)
        {
            var result = _service.CreateToken(_service.GetUserByEmail(email).GetAwaiter().GetResult());
            Assert.That(result, Is.Not.Null);

            System.Console.WriteLine(result);
        }



    }
}
