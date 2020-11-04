using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using user.Dtos;
using user.Models;
using user.Services;

namespace user.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpGet("test")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _service.GetAllUsers());
        }

        [HttpPost("register")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto toRegisterUser)
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id){
            await _service.Delete(id);
            return NoContent();

        }

        [HttpPost("login")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] UserLoginDto toLoginUser)
        {
            var response = await _service.Login(toLoginUser.Email, toLoginUser.Password);

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Data);
        }

        [HttpGet("IsEmailExisted/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> IsEmailExisted(string email)
        {
            var response = await _service.IsEmailExisted(email);

            if (response)
            {
                return Ok($"Yes, {email} already exists!");
            }
            return NoContent();
        }



    }
}
