using AutoMapper;
using RavenOverflow.Core.Entities;
using RavenOverflow.Web.Areas.Question.Models.ViewModels;

namespace RavenOverflow.Web.Models.AutoMapping
{
    public static class AutoMapperBootstrapper
    {
        public static void ConfigureMappings()
        {
            Mapper.CreateMap<CreateInputModel, CreateViewModel>();
            Mapper.CreateMap<CreateInputModel, Question>()
                .ForMember(dest => dest.Tags, opt => opt.ResolveUsing<TagsResolver>());
        }
    }
}