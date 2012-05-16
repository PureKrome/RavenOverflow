using AutoMapper;
using RavenOverflow.Services.Models;
using RavenOverflow.Web.Areas.Question.Models.ViewModels;

namespace RavenOverflow.Web.AutoMapper
{
    public static class AutoMapperBootstrapper
    {
        public static void ConfigureMappings()
        {
            Mapper.CreateMap<QuestionInputModel, CreateViewModel>()
                .ForMember(dest => dest.CustomIdentity, opt => opt.Ignore())
                .ForMember(dest => dest.Header, opt => opt.Ignore());
        }
    }
}