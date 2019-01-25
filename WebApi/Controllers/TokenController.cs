using AutoMapper;
using Data.Contracts.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApi.Models;
using WebApi.Server.Interface;

namespace WebApi.Controllers
{
    [Route("api/Token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IJwtTokenService _tokenService;
        private readonly UserManager<ApplicationUserEntity> userManager;
        private readonly IMapper mapper;

        public TokenController(
            IJwtTokenService tokenService, 
            UserManager<ApplicationUserEntity> userManager, 
            IMapper mapper
            )
        {
            _tokenService = tokenService;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        [HttpPost("[action]")]
        [Route("/api/Token/Registration")]
        public async Task<IActionResult> Registration([FromBody] TokenViewModel tokenViewModel)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            var applicationUser = new ApplicationUserEntity()
            {
                UserName = tokenViewModel.Email,
                Email = tokenViewModel.Email
            };
            var result = await userManager.CreateAsync(applicationUser, tokenViewModel.Password);
            if(!result.Succeeded)
            {
                return StatusCode(500);
            }
            return Ok();
        }
        [HttpPost]
        [Route("/api/Token/Login")]
        public async Task<IActionResult> Login([FromBody] TokenViewModel tokenViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = await userManager.FindByEmailAsync(tokenViewModel.Email);
            var correctUser = await userManager.CheckPasswordAsync(user, tokenViewModel.Password);

            if (!correctUser)
            {
                return BadRequest("Username or password is incorrect!");
            }
            return Ok(new { token = GenerateToken(tokenViewModel.Email) });
        }

        private string GenerateToken(string email)
        {
            var token = _tokenService.BuildToken(email);
            return token;
        }
    }
}
