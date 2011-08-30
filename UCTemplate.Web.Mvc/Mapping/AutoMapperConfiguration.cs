namespace UCTemplate.Web.Mvc.Mapping
{
    #region using

    using System;

    using AutoMapper;

    using Profiles;
    using Infrastructure.Mapping.Resolvers;

    #endregion

    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.CreateMap<Guid, string>().ConvertUsing<GuidToStringConverter>();

            Mapper.AddProfile(new LogViewModelMapper());
        }
    }
}
