using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using user.Controllers;
using user.Dtos;
using user.Models;
using user.Services;

namespace test
{
    [TestFixture]
    public class AuthControllerTests
    {
        private AuthController _controller;
        private Mock<IAuthService> _mockService;
        private IQueryable<User> _usersList;

        [SetUp]
        public void SetUp()
        {
            Utility.CreatePasswordHash("123", out byte[] passwordHash, out byte[] passwordSalt);

            _usersList = (new List<User>{
                new User { Id = "sis", Email="sis@user.com", PasswordHash = passwordHash, PasswordSalt = passwordSalt },
                new User { Id = "admin", Name = "admin", Email="admin@user.com", PasswordHash = passwordHash, PasswordSalt = passwordSalt }

                }).AsQueryable();

            _mockService = new Mock<IAuthService>();

            _controller = new AuthController(_mockService.Object);
        }

        [TestCase(200)]
        public void IsUserExisted_WhenEmailExists(int expectedStatusCode)
        {
            // Arrange
            _mockService.Setup(s => s.IsEmailExisted(It.IsAny<string>())).Returns(Task.FromResult(true));

            // Act
            var response = _controller.IsEmailExisted("").GetAwaiter().GetResult();

            // Assert
            var result = response as ObjectResult;
            Assert.That(result.StatusCode == expectedStatusCode);
            PrintOut(response);

        }

        [TestCase(204)]
        public void IsUserExisted_WhenEmailDoesNotExist(int expectedStatusCode)
        {
            // Arrange
            _mockService.Setup(s => s.IsEmailExisted(It.IsAny<string>())).Returns(Task.FromResult(false));

            // Act
            var response = _controller.IsEmailExisted("").GetAwaiter().GetResult();

            // Assert
            var result = response as NoContentResult;
            Assert.That(result.StatusCode == expectedStatusCode);
            PrintOut(response);
        }

        [TestCase(true, 201)]
        public void Register_WhenSuccess(bool serviceResponse, int expectedStatusCode)
        {
            // Arrange
            _mockService
                   .Setup(s => s.Register(It.IsAny<User>(), It.IsAny<string>()))
                   .Returns(Task.FromResult(new ServiceResponse<string>
                   {
                       Success = serviceResponse,
                       Data = It.IsAny<string>()
                   }));

            // Act
            var response = _controller.Register(new UserRegisterDto()).GetAwaiter().GetResult();

            // Assert
            var result = response as CreatedResult;
            Assert.That(result.StatusCode == expectedStatusCode);
            PrintOut(response);
        }

        [TestCase(false, 400)]
        public void Register_WhenFailed(bool serviceResponse, int expectedStatusCode)
        {
            // Arrange
            _mockService
                   .Setup(s => s.Register(It.IsAny<User>(), It.IsAny<string>()))
                   .Returns(Task.FromResult(new ServiceResponse<string>
                   {
                       Success = serviceResponse,
                       Message = It.IsAny<string>()
                   }));

            // Act
            var response = _controller.Register(new UserRegisterDto()).GetAwaiter().GetResult();

            // Assert
            var result = response as ObjectResult;
            Assert.That(result.StatusCode == expectedStatusCode);
            PrintOut(response);
        }


        [TestCase(false, 400)]
        public void Login_WhenFailed(bool serviceResponse, int expectedStatusCode)
        {
            // Arrange
            _mockService
                   .Setup(s => s.Login(It.IsAny<string>(), It.IsAny<string>()))
                   .Returns(Task.FromResult(new ServiceResponse<string>
                   {
                       Success = serviceResponse,
                       Message = It.IsAny<string>()
                   }));

            // Act
            var response = _controller.Login(new UserLoginDto()).GetAwaiter().GetResult();

            // Assert
            var result = response as ObjectResult;
            Assert.That(result.StatusCode == expectedStatusCode);
            PrintOut(response);
        }

        [TestCase(true, 200)]
        public void Login_WhenSuccess(bool serviceResponse, int expectedStatusCode)
        {
            // Arrange
            _mockService
                   .Setup(s => s.Login(It.IsAny<string>(), It.IsAny<string>()))
                   .Returns(Task.FromResult(new ServiceResponse<string>
                   {
                       Success = serviceResponse,
                       Message = It.IsAny<string>()
                   }));

            // Act
            var response = _controller.Login(new UserLoginDto()).GetAwaiter().GetResult();

            // Assert

            var result = response as OkObjectResult;
            Assert.That(result.StatusCode == expectedStatusCode);
            PrintOut(response);
        }



        private void PrintOut(Object obj, int len = 600)
        {
            var str = PrettyJson(JsonSerializer.Serialize(obj));
            str = str.Length > len ? str.Substring(0, len) : str;
            System.Console.WriteLine(str);
        }


        private string PrettyJson(string unPrettyJson)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            var jsonElement = JsonSerializer.Deserialize<JsonElement>(unPrettyJson);

            return JsonSerializer.Serialize(jsonElement, options);
        }




    }
}
