
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using POSModels.Models;
using POSModels.Services;
using System.Threading.Tasks;

namespace PointOfSale.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILoginViewModels _loginview;
        public AccountController(ILoginViewModels loginview)
        {
            _loginview = loginview;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel lm)
        {
            AllResponseMessage resp =await _loginview.AllUserLogin(lm);
            return Ok(new {result=resp.Result,message=resp.Message, redirect=resp.redirect});
        }

        [HttpPost]
        public async Task<IActionResult> PinLogin([FromBody] LoginModel lm)
        {
            AllResponseMessage resp = await _loginview.AllUserLogin(lm);

            if ((bool)resp.Result)
            {
                return Ok(new
                {
                    result = true,
                    message = resp.Message,
                    redirect = Url.Action("Index", "Home")
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
            //HttpContext.Session.Clear();

            await HttpContext.SignOutAsync();

            return RedirectToAction("Login", "Account");
        }
    }
}
