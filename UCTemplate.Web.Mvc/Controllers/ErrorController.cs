namespace UCTemplate.Web.Mvc.Controllers
{
    #region using

    using System;
    using System.Web.Mvc;

    #endregion

    [OutputCache(Duration = Int32.MaxValue)]
    public class ErrorController : Controller
    {
        public ActionResult Authorization()
        {
            return View();
        }

    }
}
