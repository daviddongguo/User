using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using user.Dtos;
using user.Models;
using user.Services;

namespace user.Controllers
{
    [ApiController]
    [Route("[controller")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto toRegisterUser)
        {
            var response = await _service.Register(
                new User { Email = toRegisterUser.Email }, toRegisterUser.Password
            );

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Created("", response.Data);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto toLoginUser)
        {
            var response = await _service.Login(toLoginUser.Email, toLoginUser.Password);

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Data);
        }

        [HttpPost("IsEmailExisted")]
        public async Task<IActionResult> IsEmailExisted(string email)
        {
            var response = await _service.IsEmailExisted(email);

            if (response)
            {
                return BadRequest($"{email} already exists!");
            }
            return Ok();
        }



    }
}
