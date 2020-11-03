using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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


    }
}
