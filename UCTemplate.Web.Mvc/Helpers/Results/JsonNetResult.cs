namespace UCTemplate.Web.Mvc.Helpers.Results
{
    #region using

    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Web.Mvc;

    using Newtonsoft.Json;

    #endregion

    /// <summary>
    /// A better JsonResult, using Newtonsoft.NET for serialization.
    /// </summary>
    public class JsonNetResult : ActionResult
    {
        public JsonNetResult()
        {
        }

        public JsonNetResult(object responseBody)
        {
            ResponseBody = responseBody;
        }

        public JsonNetResult(object responseBody, JsonSerializerSettings settings)
            : this(responseBody)
        {
            Settings = settings;
        }

        /// <summary>Gets or sets the serialiser settings</summary> 
        public JsonSerializerSettings Settings { get; set; }

        /// <summary>Gets or sets the encoding of the response</summary> 
        public Encoding ContentEncoding { get; set; }

        /// <summary>Gets or sets the content type for the response</summary> 
        public string ContentType { get; set; }

        /// <summary>Gets or sets the body of the response</summary> 
        public object ResponseBody { get; set; }

        /// <summary>Gets the formatting types depending on whether we are in debug mode</summary> 
        private static Formatting Formatting
        {
            get
            {
                return Debugger.IsAttached ? Formatting.Indented : Formatting.None;
            }
        }

        /// <summary> 
        /// Serialises the response and writes it out to the response object 
        /// </summary> 
        /// <param name="context">The execution context</param> 
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var response = context.HttpContext.Response;

            // set content type 
            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

            // set content encoding 
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            if (ResponseBody != null)
            {
                response.Write(JsonConvert.SerializeObject(ResponseBody, Formatting, Settings));
            }
        }
    }
}
