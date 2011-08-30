namespace UCTemplate.Web.Mvc.Helpers.Routing
{
    #region using

    using System.Web.Routing;

    #endregion

    public class LowercaseRoute : Route
    {
        public LowercaseRoute(string url, IRouteHandler routeHandler)
            : base(url, routeHandler) { }
        public LowercaseRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
            : base(url, defaults, routeHandler) { }
        public LowercaseRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler)
            : base(url, defaults, constraints, routeHandler) { }
        public LowercaseRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler) : base(url, defaults, constraints, dataTokens, routeHandler) { }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {

            var path = base.GetVirtualPath(requestContext, values);

            if (path != null)
            {
                var pathAndQuery = path.VirtualPath.Split('?');
                path.VirtualPath = pathAndQuery[0].ToLowerInvariant() + (pathAndQuery.Length == 2 ? "?" + pathAndQuery[1] : string.Empty);
            }
                

            return path;
        }
    }
}
