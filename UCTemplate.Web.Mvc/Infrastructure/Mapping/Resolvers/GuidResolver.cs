namespace UCTemplate.Web.Mvc.Infrastructure.Mapping.Resolvers
{
    #region using

    using System;

    using Common;

    #endregion

    public static class GuidResolver
    {
        public static Guid Resolve(string id)
        {
            Check.IsNotNullOrEmpty(id, "id");

            Guid guidValue;

            Guid.TryParse(id, out guidValue);

            return guidValue != Guid.Empty ? guidValue : default(Guid);
        }
    }
}
