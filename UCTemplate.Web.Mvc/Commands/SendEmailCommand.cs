namespace UCTemplate.Web.Mvc.Commands
{
    #region using

    using System;
    using System.Collections;
    using System.Configuration;
    using System.IO;
    using System.Net.Mail;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Infrastructure.Commands;

    #endregion

    /// <summary>
    /// Retrieves an email template and sends it given the provided information.
    /// </summary>
    public class SendEmailCommand : CommandBase
    {
        private readonly string _sendFrom;
        private readonly string _sendTo;
        private readonly string _subject;
        private readonly string _view;
        private readonly object _model;

        /// <summary>
        /// Initializes the command
        /// </summary>
        /// <param name="sendFrom">Address to send from.</param>
        /// <param name="sendTo">Address to send to.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="view">The email template.</param>
        /// <param name="model">The view model for the template.</param>
        public SendEmailCommand(
            string sendTo,
            string subject,
            string view,
            object model,
            string sendFrom = null)
        {
            _sendFrom = sendFrom ?? ConfigurationManager.AppSettings["NoReplyEmail"];
            _sendTo = sendTo;
            _subject = subject;
            _view = view;
            _model = model;
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        public override void Execute()
        {
            var routeData = new RouteData();
            routeData.Values.Add("controller", "MailTemplates");
            var controllerContext = new ControllerContext(new MailHttpContext(), routeData, new MailController());
            var viewEngineResult = ViewEngines.Engines.FindView(controllerContext, _view, "_Layout");
            var stringWriter = new StringWriter();
            viewEngineResult.View.Render(
                new ViewContext(controllerContext, viewEngineResult.View, new ViewDataDictionary(_model), new TempDataDictionary(),
                                stringWriter), stringWriter);


            var mailMessage = new MailMessage
            {
                From = new MailAddress(_sendFrom),
                IsBodyHtml = true,
                Body = stringWriter.GetStringBuilder().ToString(),
                Subject = _subject,
            };
            if (string.IsNullOrEmpty(_sendTo) == false)
            {
                try
                {
                    Log.Debug(string.Format("Attempting to send email to '{0}'.", _sendTo));
                    mailMessage.To.Add(new MailAddress(_sendTo));
                }
                catch(Exception ex)
                {
                    Log.Error(string.Format("Unable to send email to '{0}', invalid email address.", _sendTo), ex);
                }
            }

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Send(mailMessage);
            }

        }

        public class MailController : Controller
        {
        }

        public class MailHttpContext : HttpContextBase
        {
            private readonly IDictionary _items = new Hashtable();

            public override IDictionary Items
            {
                get { return _items; }
            }

            public override System.Web.Caching.Cache Cache
            {
                get { return HttpRuntime.Cache; }
            }

            public override HttpResponseBase Response
            {
                get
                {
                    return new MailHttpResponse();
                }
            }

            public override HttpRequestBase Request
            {
                get
                {
#if DEBUG
                    return new HttpRequestWrapper(new HttpRequest("", ConfigurationManager.AppSettings["DevUrl"], ""));
#else
                    return new HttpRequestWrapper(new HttpRequest("", ConfigurationManager.AppSettings["ProductionUrl"], ""));
#endif
                }
            }
        }

        public class MailHttpResponse : HttpResponseBase
        {
            public override string ApplyAppPathModifier(string virtualPath)
            {
                return virtualPath;
            }
        }
    }

}
