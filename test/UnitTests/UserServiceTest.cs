using NUnit.Framework;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using user.Models;
using user.Services;

namespace test
{
    [TestFixture]
    public class UserServiceTest
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
            Assert.That(result != null, Is.EqualTo(expectedValue));

            System.Console.WriteLine($"{email} : {result?.Email}");
        }

        [TestCase("sis@user.com", true)]
        [TestCase("Sis@user.com", true)]
        [TestCase("", true)]
        [TestCase("Sis@xxer.com", false)]
        public void IsUserExisted(string email, bool expectedValue)
        {
            var result = _service.IsEmailExisted(email).GetAwaiter().GetResult();
            Assert.That(result == expectedValue);

            System.Console.WriteLine($"{email} : {result}");
        }

        [TestCase("sis@user.com")]
        public void CreateToken(string email)
        {
            var result = _service.CreateToken(_service.GetUserByEmail(email).GetAwaiter().GetResult());
            Assert.That(result, Is.Not.Null);
            System.Console.WriteLine(result);

            var tokenS = DecodeToken(result);
            ((List<Claim>)tokenS.Claims).ForEach(a => System.Console.WriteLine(a.Type.ToString() + " " + a.Value));
            var resultEmail = ((List<Claim>)tokenS.Claims).FirstOrDefault(a => a.Type.ToString().Equals("email"));
            Assert.That(resultEmail.Value, Is.EqualTo(email));
        }

        [TestCase("sis@user.com", "123", true)]
        [TestCase("sis@user.com", "l32", false)]
        [TestCase("sis@user.com", "", false)]
        public void VerifyPasswordHash(string email, string password, bool expectedValue)
        {
            // Arrange
            var user = _service.GetUserByEmail(email).GetAwaiter().GetResult();

            // Act
            var result = _service.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt);
            Assert.That(result, Is.EqualTo(expectedValue));
        }

        [TestCase("sis@user.com", "l32", false)]
        [TestCase("sis@user.com", "", false)]
        [TestCase("six@user.com", "", false)]
        public void Login_WhenFailed(string email, string password, bool expectedValue)
        {
            // Act
            var result = _service.Login(email, password).GetAwaiter().GetResult();
            Assert.That(result.Success, Is.EqualTo(expectedValue));
            Assert.That(result.Message, Is.Not.Null);
            System.Console.WriteLine(result.Message);
        }

        [TestCase("sis@user.com", "123", true)]
        public void Login_WhenSuccess(string email, string password, bool expectedValue)
        {
            // Act
            var result = _service.Login(email, password).GetAwaiter().GetResult();
            Assert.That(result.Success, Is.EqualTo(expectedValue));
            Assert.That(result?.Data, Is.Not.Null);
            System.Console.WriteLine(result?.Data);
        }

        [TestCase("david@wu.com", "123", true)]
        [TestCase("sis@user.com", "123", false)]
        [TestCase("", "123", false)]
        public void Register(string email, string password, bool expectedValue)
        {
            // Arrange
            var user = new User { Email = email };

            // Act
            var result = _service.Register(user, password).GetAwaiter().GetResult();
            Assert.That(result?.Data != null, Is.EqualTo(expectedValue));
            System.Console.WriteLine(result?.Message);
        }




        private JwtSecurityToken DecodeToken(string encodedJwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadToken(encodedJwt) as JwtSecurityToken;
            return tokenS;
        }



    }
}
