using LoyalCode.Dmitriev.Ivan.Application.User.LoginUser;
using LoyalCode.Dmitriev.Ivan.Application.User.LogoutUser;
using LoyalCode.Dmitriev.Ivan.Application.User.RegisterUser;
using LoyalCode.Dmitriev.Ivan.UserService.Dto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LoyalCode.Dmitriev.Ivan.UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserApiController> _logger;

        public UserApiController(IMediator mediator, ILogger<UserApiController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<RegisterUserResponseDto>> Register(UserDto registerDto)
        {
            try
            {
                var command = new RegisterUserCommand
                {
                    Name = registerDto.Name,
                    Password = registerDto.Password
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user {UserName}", registerDto.Name);
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login(UserDto loginDto)
        {
            try
            {
                var command = new LoginUserCommand
                {
                    Name = loginDto.Name,
                    Password = loginDto.Password
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed login attempt for user {UserName}", loginDto.Name);
                return Unauthorized(new { error = "Неверное имя пользователя или пароль" });
            }
        }

        [HttpGet("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (!string.IsNullOrEmpty(token))
                {
                    await _mediator.Send(new LogoutUserCommand { Token = token });
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logout ");
                return BadRequest(new { error = ex.Message });
            }
        }
    }


}
