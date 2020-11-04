using NUnit.Framework;
using RestSharp;
using user.Dtos;

namespace apiTests
{
    [TestFixture]
    public class AzureApiTests
    {
        private RestClient _client;
        //private readonly string baseUrl = "https://david-user-login.azurewebsites.net/";
        private readonly string baseUrl = "http://localhost:5000";

        [SetUp]
        public void SetUp()
        {
            _client = new RestClient(baseUrl);
            _client.AddHandler("application/json", () => new RestSharp.Serialization.Json.JsonSerializer());
            _client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
        }

        [TestCase(200), Timeout(8000)]
        public void TestUsers(int expectedstatusCode)
        {
            // Arrange
            var request = new RestRequest("auth/test", Method.GET);

            // Act
            var response = _client.ExecuteGetAsync(request).GetAwaiter().GetResult();

            // Assert
            Assert.That((int)response.StatusCode == expectedstatusCode);
            System.Console.WriteLine(response.ResponseUri);
            System.Console.WriteLine(response.StatusCode);
            System.Console.WriteLine(response.Content);
        }

        [TestCase("aaabbddddddd", 204)]
        [TestCase("sis@user.com", 200)]
        [TestCase("Sis@user.com", 200)]
        [TestCase("Sis@xxer.com", 204)]
        public void IsUserExisted(string email, int expectedStatusCode)
        {
            // Arrange
            var request = new RestRequest("auth/IsEmailExisted/" + email, Method.GET);

            // Act
            var response = _client.ExecuteAsync(request).GetAwaiter().GetResult();

            // Assert
            Assert.That((int)response.StatusCode == expectedStatusCode);
            System.Console.WriteLine(response.ResponseUri);
            System.Console.WriteLine(response.StatusCode);
            System.Console.WriteLine(response.Content);
        }

        [TestCase("sis@user.com", "123", 200)]
        [TestCase("sis@user.com", "133", 400)]
        [TestCase("sis@usxx.com", "123", 400)]
        public void Login(string email, string password, int expectedStatusCode)
        {
            // Arrange
            var request = new RestRequest("auth/login" , Method.POST);
            request.AddJsonBody(new UserLoginDto
            {
                Email = email,
                Password = password
            });

            // Act
            var response = _client.ExecuteAsync(request).GetAwaiter().GetResult();

            // Assert
            Assert.That((int)response.StatusCode == expectedStatusCode);
            System.Console.WriteLine(response.ResponseUri);
            System.Console.WriteLine(response.StatusCode);
            System.Console.WriteLine(response.Content);
        }

        [TestCase("sis@user.com", "323", 400)]
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
            Assert.That((int)response.StatusCode == expectedStatusCode);
            System.Console.WriteLine(response.ResponseUri);
            System.Console.WriteLine(response.StatusCode);
            System.Console.WriteLine(response.Content);
        }

        [TestCase("guest@test.com", "323", 201)]
        public void Register_WhenSuccess(string email, string password, int expectedStatusCode)
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
            Assert.That((int)response.StatusCode == expectedStatusCode);
            System.Console.WriteLine(response.ResponseUri);
            System.Console.WriteLine(response.StatusCode);
            System.Console.WriteLine(response.Content);

            // DeleteRT
            var id = response.Content;
            request = new RestRequest("auth/" + id.Substring(1,12), Method.DELETE);
            response = _client.ExecuteAsync(request).GetAwaiter().GetResult();
            System.Console.WriteLine(response.ResponseUri);
            System.Console.WriteLine(response.StatusCode);
            System.Console.WriteLine(response.Content);


        }



    }
}
