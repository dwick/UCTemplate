namespace UCTemplate.Web.Mvc.Mapping.Profiles
{
    #region using

    using AutoMapper;

    using Models;
    using ViewModels;

    #endregion

    public class LogViewModelMapper : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Log, LogViewModel>()
                .ForMember(x => x.Date, o => o.MapFrom(s => s.Date.ToString("MM/dd/yy HH:mm:ss")));
        }
    }
}
