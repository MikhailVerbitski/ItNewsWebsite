using System;
using System.Collections;
using System.Threading.Tasks;
using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Domain.Contracts.Models.ViewModels.Account;
using Domain.Implementation.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace SocialNetwork.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUserEntity> userManager;
        private readonly SignInManager<ApplicationUserEntity> signInManager;

        private readonly ServiceOfUser serviceOfUser;
        private readonly ServiceOfImage serviceOfImage;

        public AccountController(
            ApplicationDbContext context, 
            UserManager<ApplicationUserEntity> userManager, 
            SignInManager<ApplicationUserEntity> signInManager,
            IHostingEnvironment hostingEnvironment,
            IMapper mapper)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.signInManager = signInManager;

            serviceOfImage = new ServiceOfImage(context, hostingEnvironment);
            serviceOfUser = new ServiceOfUser(context, mapper, hostingEnvironment);
        }

        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            return View(new RegisterViewModel() { ReturnUrl = returnUrl });
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUserEntity userEntity = mapper.Map<RegisterViewModel, ApplicationUserEntity>(model);

                var result = await userManager.CreateAsync(userEntity, model.Password);

                if (result.Succeeded)
                {
                    userEntity = serviceOfUser.GetApplicationUser(model);
                    return RedirectToAction("Login");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel() { ReturnUrl = returnUrl });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Login, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {

                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "News");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}