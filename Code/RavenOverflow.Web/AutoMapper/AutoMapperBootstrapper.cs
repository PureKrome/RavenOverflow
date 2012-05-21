using AutoMapper;
using RavenOverflow.Core.Entities;
using RavenOverflow.Web.Areas.Question.Models;

namespace RavenOverflow.Web.AutoMapper
{
    public static class AutoMapperBootstrapper
    {
        public static void ConfigureMappings()
        {
            Mapper.CreateMap<QuestionInputModel, Question>()
                .ForMember(dest => dest.Tags, opt => opt.ResolveUsing<TagsResolver>())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Answers, opt => opt.Ignore())
                .ForMember(dest => dest.NumberOfViews, opt => opt.Ignore())
                .ForMember(dest => dest.Vote, opt => opt.Ignore())
                .ForMember(dest => dest.Comments, opt => opt.Ignore());

            Mapper.CreateMap<QuestionInputModel, QuestionViewModel>()
                .ForMember(dest => dest.CustomIdentity, opt => opt.Ignore());
        }
    }
}