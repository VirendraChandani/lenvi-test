using Microsoft.AspNetCore.Mvc;
using TransactionManager.Models;
using TransactionManager.Services.Authentication;

namespace TransactionManager.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger, IAuthService authService)
        {
            _configuration = configuration;
            _logger = logger;
            _authService = authService;
        }

        [HttpPost, Route("login")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK, Type =typeof(LoginResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Login(LoginRequest model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
            {
                _logger.LogError("Invalid Login Request or Invalid Username or Password.");
                var response = new LoginResponse
                {
                    IsSuccess = false,
                    Message = "Invalid Login Request or Invalid Username or Password."
                };
                return BadRequest(new { Response = response });
            }

            var isAuthenticated = _authService.Authenticate(model.UserName, model.Password);

            if (isAuthenticated)
            {
                var token = _authService.GenerateJwtToken(model.UserName);
                _logger.LogInformation($"Token generated successfully for user {model.UserName}.");
                return Ok(new LoginResponse
                {
                    IsSuccess = true,
                    Message = "Login successful.",
                    Token = token,
                    ExpirationDate = DateTime.Now.AddSeconds(int.Parse(_configuration["Jwt:TokenExpirationSeconds"]))
                });
            }
            else
            {
                _logger.LogError("Unauthorized - Invalid username or password.");
                return Unauthorized(new LoginResponse
                {
                    IsSuccess = false,
                    Message = "Invalid Username or Password."
                });
            }
        }
    }
}
