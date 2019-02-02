using AutoMapper;
using Data.Contracts.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Implementation.Services;
using WebApi.Models;
using WebApi.Server.Interface;
using System.Collections.Generic;
using Data.Implementation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/Token/[action]")]
    public class TokenController : ControllerBase
    {
        private readonly IJwtTokenService _tokenService;
        private readonly UserManager<ApplicationUserEntity> userManager;
        private readonly IMapper mapper;
        private readonly ServiceOfAccount serviceOfAccount;

        public TokenController(
            ApplicationDbContext context,
            IJwtTokenService tokenService, 
            UserManager<ApplicationUserEntity> userManager,
            RoleManager<IdentityRole> roleManager,
            IHostingEnvironment hostingEnvironment,
            IMapper mapper
            )
        {
            _tokenService = tokenService;
            this.userManager = userManager;
            this.mapper = mapper;

            serviceOfAccount = new ServiceOfAccount(context, userManager, roleManager, hostingEnvironment, mapper);
        }

        [HttpPost]
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
            await serviceOfAccount.TryToRegistration(tokenViewModel.Email);
            return Ok();
        }
        [HttpPost]
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

            IList<string> roles = await userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, (roles.Contains("admin") ? "admin" : "user")),
                new Claim("UserId" , user.Id)
            };

            return Ok(new { token = _tokenService.BuildToken(tokenViewModel.Email, claims) });
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "user")]
        [HttpGet]
        public IActionResult TokenVerification()
        {
            return Ok();
        }
    }
}
