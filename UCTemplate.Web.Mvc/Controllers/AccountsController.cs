namespace UCTemplate.Web.Mvc.Controllers
{
    #region using

    using System;
    using System.DirectoryServices.AccountManagement;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Security;

    using Helpers.Attributes;
    using ViewModels;

    #endregion

    public class AccountsController : AbstractController
    {
        [Ssl]
        public ActionResult Login()
        {
            return View();
        }

        [Ssl, HttpPost, OutputCache(Duration = 0, NoStore = true)]
        public ActionResult Login(LoginInput input, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Membership.ValidateUser(input.Username, input.Password))
                    {
                        FormsAuthentication.SetAuthCookie(input.Username, false);

                        return RedirectFromLoginPage(returnUrl);
                    }

                    ModelState.AddModelError(string.Empty, "Please provide a valid username and password.");
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "An error occured while validating your credentials, please contact us if the problem persists.");
                }
                

            }
            return View(input);
        }

        [HttpGet]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            TempData["Logout"] = true;
            return RedirectToAction("Index", "Home");
        }

        [ChildActionOnly]
        public ActionResult CurrentUser()
        {
            if (!Request.IsAuthenticated)
                return PartialView(new CurrentUserViewModel());

            var principalContext = MvcApplication.DomainConnections.FirstOrDefault(); // grab a dc connection

            if (principalContext == null) // not connected to any dcs
            {
                return PartialView(new CurrentUserViewModel { DisplayName = HttpContext.User.Identity.Name });
            }

            var user = UserPrincipal.FindByIdentity(
                principalContext, IdentityType.SamAccountName, HttpContext.User.Identity.Name);

            return PartialView(new CurrentUserViewModel { DisplayName = user != null ? user.DisplayName : HttpContext.User.Identity.Name });
        }

        private ActionResult RedirectFromLoginPage(string retrunUrl = null)
        {
            if (string.IsNullOrEmpty(retrunUrl))
                return RedirectToAction("Index", "Home");
            return Redirect(retrunUrl);
        }
    }
}
