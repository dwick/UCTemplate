namespace UCTemplate.Web.Mvc.Controllers
{
    #region using

    using System;
    using System.Web.Mvc;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using log4net;

    using Helpers.Results;

    #endregion

    /// <summary>
    /// Base controller.
    /// </summary>
    public abstract class AbstractController : Controller
    {
        protected readonly ILog Log;

        protected AbstractController()
        {
            Log = LogManager.GetLogger(GetType());
        }
        
        protected new JsonNetResult Json(object data)
        {
            var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            return new JsonNetResult(data, settings);
        }

        protected string GetDomainUrl()
        {
            if (Request.Url == null)
                throw new NullReferenceException();

            return Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host +
                   (Request.Url.Port != 80 ? ":" + Request.Url.Port : "");
        }
    }
}
