namespace UCTemplate.Web.Mvc.Helpers.Attributes
{
    #region using

    using System.Web.Mvc;

    using Common;

    #endregion

    /// <summary>
    /// Requires SSL, redirects if request isn't local.
    /// </summary>
    public class SslAttribute : RequireHttpsAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            Check.IsNotNull(filterContext, "filterContext");

            if(filterContext.HttpContext != null && filterContext.HttpContext.Request.IsLocal)
            {
                return;
            }
            
            base.OnAuthorization(filterContext);
        }
    }
}