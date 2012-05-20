using System;
using System.Collections.Generic;
using AutoMapper;
using RavenOverflow.Web.Areas.Question.Models;

namespace RavenOverflow.Web.AutoMapper
{
    public class TagsResolver : ValueResolver<QuestionInputModel, ICollection<string>>
    {
        protected override ICollection<string> ResolveCore(QuestionInputModel createInputModel)
        {
            if (createInputModel == null || createInputModel.Tags == null)
            {
                return null;
            }

            return createInputModel.Tags.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}