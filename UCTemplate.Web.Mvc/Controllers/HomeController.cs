namespace UCTemplate.Web.Mvc.Controllers
{
    #region using

    using System;
    using System.Web.Mvc;

    #endregion

    public class HomeController : AbstractController
    {
        public ActionResult Index()
        {
            return Request.IsAuthenticated ? View() : View("Login");
        }

        [OutputCache(Duration = Int32.MaxValue)]
        public ActionResult Contact()
        {
            return View();
        }

        [OutputCache(Duration = Int32.MaxValue)]
        public ActionResult Guide()
        {
            return View();
        }
    }
}
