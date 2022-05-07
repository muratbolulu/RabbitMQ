using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace UdemyRabbitMQWeb.ExcelCreate.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<IdentityUser> _userManaager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManaager, SignInManager<IdentityUser> signInManager)
        {
            _userManaager = userManaager;
            _signInManager = signInManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var hasUser= await _userManaager.FindByEmailAsync(Email);

            if (hasUser == null)
            {
                return View();
            }

            var signInResult = await _signInManager.PasswordSignInAsync(hasUser, Password, true, false);
            if (!signInResult.Succeeded)
            {
                return View();
            }

            return RedirectToAction(nameof(HomeController.Index),"Home");
        }
    }
}
