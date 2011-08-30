namespace UCTemplate.Web.Mvc.Helpers.Attributes
{
    #region using

    using System.Web.Mvc;

    #endregion

    /// <summary>
    /// Properly redirects unauthorized users.
    /// </summary>
    public class AuthWithRedirectAttribute : AuthorizeAttribute
    {
        public string UnauthorizedRedirect { get; set; }

        public AuthWithRedirectAttribute(string unauthorizedRedirect)
        {
            UnauthorizedRedirect = unauthorizedRedirect;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if(filterContext.Result is HttpUnauthorizedResult)
                filterContext.Result = new RedirectResult(UnauthorizedRedirect);
        }
    }
}