using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rohit_bike_store.DTO;
using Rohit_bike_store.Services;

namespace Rohit_bike_store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenService tokenService;
        public AuthController(UserManager<IdentityUser> userManager, ITokenService tokenService)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDTO)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDTO.Username,
                Email = registerRequestDTO.Username
            };

            var identityResult = await userManager.CreateAsync(identityUser, registerRequestDTO.Password);

            if (identityResult.Succeeded)
            {
                if (registerRequestDTO.Roles != null && registerRequestDTO.Roles.Any())
                {
                    foreach (var role in registerRequestDTO.Roles)
                    {
                        identityResult = await userManager.AddToRoleAsync(identityUser, role);
                        if (!identityResult.Succeeded)
                        {
                            return BadRequest($"Failed to add role {role}. {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                        }
                    }

                    return Ok("User was registered! Please login.");
                }

                return Ok("User was registered without roles! Please login.");
            }

            return BadRequest($"User registration failed. {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDTO)
        {
            var user = await userManager.FindByEmailAsync(loginRequestDTO.Username);

            if (user != null)
            {
                var checkpass = await userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
                if (checkpass)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    if (roles != null)
                    {
                        var jwtToken = tokenService.CreateJWTToken(user, roles.ToList());
                        var response = new LoginResponseDto
                        {
                            JwtToken = jwtToken
                        };

                        return Ok(response);
                    }

                }

            }

            return BadRequest("User name or password incorrect");

        }

    }
}
