using AutoMapper;
using RavenOverflow.Core.Entities;
using RavenOverflow.Web.Areas.Question.Models.ViewModels;

namespace RavenOverflow.Web.Models.AutoMapping
{
    public static class AutoMapperBootstrapper
    {
        public static void ConfigureMappings()
        {
            Mapper.CreateMap<CreateInputModel, CreateViewModel>()
                .ForMember(dest => dest.CustomIdentity, opt => opt.Ignore())
                .ForMember(dest => dest.Header, opt => opt.Ignore());

            Mapper.CreateMap<CreateInputModel, Question>()
                .ForMember(dest => dest.Tags, opt => opt.ResolveUsing<TagsResolver>())
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Answers, opt => opt.Ignore())
                .ForMember(dest => dest.NumberOfViews, opt => opt.Ignore())
                .ForMember(dest => dest.Vote, opt => opt.Ignore())
                .ForMember(dest => dest.Comments, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}