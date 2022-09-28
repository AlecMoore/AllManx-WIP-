using AllManx.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AllManx.StoredProcedures;
using System.Net.Mail;
using System.Net;

namespace AllManx.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public ActionResult SignIn(Login login)
        {

            int UserId = StoredProcedures.StoredProcedures.GetUserIdFromEmail(login.Email);
            string hash = StoredProcedures.StoredProcedures.GetHash(UserId);
            if (Hashing.ValidatePassword(login.Password, hash))
            {
                StoredProcedures.StoredProcedures.UpdateLastLogin(UserId);
                return Redirect("/Login/LoggedIn");
            } else
            {
                return Redirect("/Home/Index");
            }
        }

        public ActionResult LoggedIn()
        {
            return View();
        }
    }
}