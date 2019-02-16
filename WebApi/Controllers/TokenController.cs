using AutoMapper;
using Data.Contracts.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Implementation.Services;
using WebApi.Server.Interface;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Domain.Contracts.Models.ViewModels.Account;
using Domain.Contracts.Validators.ViewModels.Account;
using System.Linq;
using Domain.Contracts.Models.ViewModels;
using System.IO;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/Token/[action]")]
    public class TokenController : Controller
    {
        private readonly IJwtTokenService _tokenService;
        private readonly UserManager<ApplicationUserEntity> userManager;
        private readonly IMapper mapper;
        private readonly ServiceOfAccount serviceOfAccount;

        public TokenController(
            IJwtTokenService tokenService, 
            UserManager<ApplicationUserEntity> userManager,
            IHostingEnvironment hostingEnvironment,
            IMapper mapper,
            ServiceOfAccount serviceOfAccount
            )
        {
            _tokenService = tokenService;
            this.userManager = userManager;
            this.mapper = mapper;
            this.serviceOfAccount = serviceOfAccount;
        }

        [HttpPost]
        public async Task<JsonResult> Registration([FromBody] RegisterViewModel registerViewModel)
        {
            var validator = new RegisterValidator();
            var validatorResult = validator.Validate(registerViewModel);
            if (validatorResult.IsValid)
            {
                var applicationUser = mapper.Map<RegisterViewModel, ApplicationUserEntity>(registerViewModel);
                var result = await userManager.CreateAsync(applicationUser, registerViewModel.Password);
                if (!result.Succeeded)
                {
                    return Json(result.Errors.Select(a => a.Description).ToList());
                }
                var code = await userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
                var callbackUrl = Url.Action(
                    nameof(ConfirmEmail),
                    this.ControllerContext.ActionDescriptor.ControllerName,
                    new
                    {
                        userId = applicationUser.Id,
                        code = code
                    },
                    this.HttpContext.Request.Scheme,
                    this.HttpContext.Request.Host.ToString());
                serviceOfAccount.SendEmailAsync(registerViewModel.Email, "Confirm your account", $"Confirm your registration by clicking on the link: <a href='{callbackUrl}'>link</a>");
                await serviceOfAccount.TryToRegistration(registerViewModel.Login);
                return Json(Ok());
            }
            else
            {
                return Json(validatorResult.Errors.Select(a => a.ErrorMessage).ToList());
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return StatusCode(400);
            }
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return StatusCode(400);
            }
            var result = await userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return Redirect($"{this.HttpContext.Request.Scheme}://{this.HttpContext.Request.Host.ToString()}/Login");
            }
            return StatusCode(400);
        }
        [HttpPost]
        public async Task<JsonResult> Login([FromBody] LoginViewModel loginViewModel)
        {
            var response = new TokenViewModel();

            var validator = new LoginValidator();
            var validatorResult = validator.Validate(loginViewModel);
            if (validatorResult.IsValid)
            {
                var user = serviceOfAccount.Get(a => a.UserName == loginViewModel.Login);
                var correctUser = await userManager.CheckPasswordAsync(user, loginViewModel.Password);

                if (!correctUser)
                {
                    response.Errors = new List<string>();
                    (response.Errors as List<string>).Add("Username or password is incorrect!");
                    return Json(response);
                }

                IList<string> roles = await userManager.GetRolesAsync(user);
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, roles.FirstOrDefault()),
                    new Claim("UserId" , user.Id)
                };

                response.Token = _tokenService.BuildToken(user.Email, claims);
            }
            else
            {
                response.Errors = validatorResult.Errors.Select(a => a.ErrorMessage).ToList();
            }
            return Json(response);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "user, writer, admin")]
        [HttpGet]
        public IActionResult TokenVerification()
        {
            return Ok();
        }
    }
}
