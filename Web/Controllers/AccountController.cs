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
        private readonly SignInManager<ApplicationUserEntity> signInManager;

        private readonly ServiceOfUser serviceOfUser;
        private readonly ServiceOfAccount serviceOfAccount;

        public AccountController(
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context, 
            UserManager<ApplicationUserEntity> userManager, 
            SignInManager<ApplicationUserEntity> signInManager,
            IHostingEnvironment hostingEnvironment,
            IMapper mapper)
        {
            this.signInManager = signInManager;

            serviceOfUser = new ServiceOfUser(context, roleManager, userManager, mapper, hostingEnvironment);
            serviceOfAccount = new ServiceOfAccount(context, userManager, roleManager, hostingEnvironment, mapper);
        }

        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            var registerViewModel = new RegisterViewModel()
            {
                ReturnUrl = returnUrl,
            };
            return View(registerViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await serviceOfAccount.TryToRegistration(model);
                if (result.Succeeded)
                {
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
                        return RedirectToAction("Index", "ForTests");
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