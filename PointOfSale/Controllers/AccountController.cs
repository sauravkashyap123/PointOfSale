using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using POSModels.Models;
using POSModels.Services;
using System.Threading.Tasks;

namespace PointOfSale.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel lm)
        {
            AllResponseMessage resp = await _authService.AllUserLoginAsync(lm);
            if (resp.Extra != null && resp.Extra.TryGetValue(4, out var extraItem))
            {
                ViewBag.Role = extraItem;
            }
            else
            {
                ViewBag.Role = "DefaultRole";
            }
            return Ok(new { result = resp.Result, message = resp.Message, redirect = resp.redirect });
        }

        [HttpPost]
        public async Task<IActionResult> PinLogin([FromBody] LoginModel lm)
        {
            AllResponseMessage resp = await _authService.AllUserLoginAsync(lm);

            if ((bool)resp.Result)
            {
                return Ok(new
                {
                    result = true,
                    message = resp.Message,
                    redirect = resp.redirect ?? Url.Action("Index", "Home")
                });
            }

            return Ok(new
            {
                result = false,
                message = resp.Message
            });
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
