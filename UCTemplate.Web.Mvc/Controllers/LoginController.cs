namespace UCTemplate.Web.Mvc.Controllers
{
    #region using

    using System;
    using System.DirectoryServices.AccountManagement;
    using System.DirectoryServices.Protocols;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Security;

    using Helpers.Attributes;
    using ViewModels;

    #endregion

    public class LoginController : AbstractController
    {
        [Ssl]
        public ActionResult Login()
        {
            return View();
        }

        [Ssl, HttpPost, OutputCache(Duration = 0, NoStore = true)]
        public ActionResult Login(LoginInput input, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View("Login", input);

            var principalContext = MvcApplication.DomainConnections.FirstOrDefault(); // grab a dc connection

            if (principalContext == null) // not connected to any dcs
            {
                ModelState.AddModelError(string.Empty, "The login server is unavailable, please contact us if the problem persists.");
                return View("Login", input);
            }

            var user = UserPrincipal.FindByIdentity(
                principalContext, IdentityType.SamAccountName, input.Username);

            if (user == null) // not in active directory
            {
                ModelState.AddModelError(string.Empty, "Please enter a valid username and password.");
                return View("Login", input);
            }

            if (!user.Enabled.HasValue || !user.Enabled.Value) // account disabled
            {
                ModelState.AddModelError(string.Empty, "Your account is disabled.");
            }
            else
            {
                try
                {
                    if (principalContext.ValidateCredentials(input.Username, input.Password)) // logged in successfully
                    {
                        Log.Info(string.Format("Successful login for '{0}' by '{1}'.", input.Username, Request.UserHostAddress));
                        FormsAuthentication.SetAuthCookie(input.Username, true);

                        return RedirectFromLoginPage(returnUrl);
                    }
                    ModelState.AddModelError(string.Empty, "Please enter a valid username and password."); // failed
                }
                catch (LdapException ex)
                {
                    Log.Error(string.Format("Problem w/{0}, disposing connection '{0}'", principalContext.ConnectedServer), ex);
                    MvcApplication.DisposeConnection(principalContext);
                }
                catch (Exception ex)
                {
                    Log.Error("Unable to validate credentials", ex);
                    ModelState.AddModelError(string.Empty, "Unable to validate your credentials, please contact us if the problem persists.");
                }
            }
            Log.Warn(string.Format("Unsucessful login attempt to '{0}' by '{1}'.", input.Username, Request.UserHostAddress)); // always log if the account is valid

            return View("Login", input );
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
