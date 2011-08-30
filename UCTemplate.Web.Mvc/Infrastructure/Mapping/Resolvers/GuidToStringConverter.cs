namespace UCTemplate.Web.Mvc.Infrastructure.Mapping.Resolvers
{
    #region using

    using System;

    using AutoMapper;

    #endregion

    public class GuidToStringConverter : TypeConverter<Guid, string>
    {
        protected override string ConvertCore(Guid source)
        {
            return source.ToString("N");
        }
    }
}
