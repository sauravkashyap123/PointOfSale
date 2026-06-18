using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POSModels.Models;
using POSModels.Services;

namespace PointOfSale.Controllers
{
   
    public class POSController : Controller
    {
        private readonly ILoginViewModels _loginview;
        public POSController(ILoginViewModels loginview)
        {
            _loginview= loginview;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel lm)
        {
            AllResponseMessage resp = await _loginview.AllStaffLogin(lm);
            return Ok(new { result = resp.Result, message = resp.Message, redirect = "/Billing/Index" });
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();

            await HttpContext.SignOutAsync();

            return RedirectToAction("Login", "POS");
        }
    }
}
