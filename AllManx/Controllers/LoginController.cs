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

        [ValidateAntiForgeryToken]
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
                ModelState.AddModelError("", "Login details are incorrect");
                return View("Index");
            }
        }

        public ActionResult LoggedIn()
        {
            return View();
        }


        public ActionResult PasswordRecovery()
        {
            return View();

        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult PasswordRecovery(string Email)
        {
            SendPasswordRecoveryEmail(Email);

            return View()

        }
        private void SendPasswordRecoveryEmail(string Email)
        {

            int UserId = StoredProcedures.StoredProcedures.GetUserIdFromEmail(Email);
            Guid activationCode = Guid.NewGuid();
            StoredProcedures.StoredProcedures.InsertActivationCode(UserId, activationCode);
            string baseUrl = string.Format("{0}://{1}", HttpContext.Request.Scheme, HttpContext.Request.Host);
            using (MailMessage mm = new MailMessage("AllManxApp@gmail.com", Email))
            {
                mm.Subject = "Password Reset";
                string body = "<br /><br />Please click the following link to reset your password";
                body += "<br /><a href = '" + $"{baseUrl}/Login/PasswordReset?ActivationCode={activationCode}" + "'>Click here to reset your password</a>";
                body += "<br /><br />Thanks";
                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential("AllManxApp@gmail.com", "smmsphpvvchrlmmj");
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
            }
        }
    }
}