using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using user.Dtos;
using user.Models;

namespace apiTests
{
    [TestFixture]
    public class AzureApiTests
    {
        private RestClient _client;
        private readonly string _toRegisterEmail = "guest@email.ca";
        private readonly string baseUrl = "https://david-user-login.azurewebsites.net/";
        // private readonly string baseUrl = "http://localhost:5000";

        [OneTimeSetUp]
        public void Init()
        {
            _client = new RestClient(baseUrl);
            _client.AddHandler("application/json", () => new RestSharp.Serialization.Json.JsonSerializer());
            _client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            DeleteUserByEmail(_toRegisterEmail);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
        }

        private void DeleteUserByEmail(string email)
        {
            // Find User
            var request = new RestRequest("auth/email/" + email, Method.GET);
            var response = _client.ExecuteAsync<User>(request).GetAwaiter().GetResult();
            // var id = response.Data.Id;
            var userContent = response?.Content;

            string id = null;
            if (!string.IsNullOrWhiteSpace(userContent))
            {
                var obj = JsonConvert.DeserializeObject<User>(userContent);
                id = obj.Id;
            }


            // Delete User
            if (id != null)
            {
                request = new RestRequest("auth/" + id, Method.DELETE);
                var res = _client.ExecuteAsync(request).GetAwaiter().GetResult();
            }

        }

        [TestCase(StatusCodes.Status200OK), Timeout(8000)]
        public void TestUsers(int expectedStatusCode)
        {
            // Arrange
            var request = new RestRequest("auth/test", Method.GET);

            // Act
            var response = _client.ExecuteGetAsync(request).GetAwaiter().GetResult();

            // Assert
            Assert.That((int)response.StatusCode, Is.EqualTo(expectedStatusCode));
            System.Console.WriteLine(response.ResponseUri);
            System.Console.WriteLine(response.StatusCode);
            System.Console.WriteLine(response.Content);
        }

        [TestCase("aaabbddddddd", StatusCodes.Status204NoContent)]
        [TestCase("sis@user.com", StatusCodes.Status200OK)]
        [TestCase("Sis@user.com", StatusCodes.Status200OK)]
        [TestCase("Sis@xxer.com", StatusCodes.Status204NoContent)]
        public void IsUserExisted(string email, int expectedStatusCode)
        {
            // Arrange
            var request = new RestRequest("auth/IsEmailExisted/" + email, Method.GET);

            // Act
            var response = _client.ExecuteAsync(request).GetAwaiter().GetResult();

            // Assert
            Assert.That((int)response.StatusCode, Is.EqualTo(expectedStatusCode));
            System.Console.WriteLine(response.ResponseUri);
            System.Console.WriteLine(response.StatusCode);
            System.Console.WriteLine(response.Content);
        }

        [TestCase("sis@user.com", "123", StatusCodes.Status200OK)]
        [TestCase("sis@user.com", "133", StatusCodes.Status400BadRequest)]
        [TestCase("sis@usxx.com", "123", StatusCodes.Status400BadRequest)]
        public void Login(string email, string password, int expectedStatusCode)
        {
            // Arrange
            var request = new RestRequest("auth/login", Method.POST);
            request.AddJsonBody(new UserLoginDto
            {
                Email = email,
                Password = password
            });

            // Act
            var response = _client.ExecuteAsync(request).GetAwaiter().GetResult();

            // Assert
            Assert.That((int)response.StatusCode, Is.EqualTo(expectedStatusCode));
            System.Console.WriteLine(response.ResponseUri);
            System.Console.WriteLine(response.StatusCode);
            System.Console.WriteLine(response.Content);
        }

        [TestCase("sis@user.com", "323", StatusCodes.Status400BadRequest)]
        public void Register_WhenFailed(string email, string password, int expectedStatusCode)
        {
            // Arrange
            var request = new RestRequest("auth/Register", Method.POST);
            request.AddJsonBody(new UserRegisterDto
            {
                Email = email,
                Password = password
            });

            // Act
            var response = _client.ExecuteAsync(request).GetAwaiter().GetResult();

            // Assert
            Assert.That((int)response.StatusCode, Is.EqualTo(expectedStatusCode));
            System.Console.WriteLine(response.ResponseUri);
            System.Console.WriteLine(response.StatusCode);
            System.Console.WriteLine(response.Content);
        }

        [TestCase("323", StatusCodes.Status201Created)]
        public void Register_WhenSuccess(string password, int expectedStatusCode)
        {
            // Arrange
            var request = new RestRequest("auth/Register", Method.POST);
            request.AddJsonBody(new UserRegisterDto
            {
                Email = _toRegisterEmail,
                Password = password
            });

            // Act
            var response = _client.ExecuteAsync(request).GetAwaiter().GetResult();

            // Assert
            Assert.That((int)response.StatusCode, Is.EqualTo(expectedStatusCode));
            System.Console.WriteLine(response.ResponseUri);
            System.Console.WriteLine(response.StatusCode);
            System.Console.WriteLine(response.Content);
        }
    }
}
