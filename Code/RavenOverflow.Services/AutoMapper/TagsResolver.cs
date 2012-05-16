using System;
using System.Collections.Generic;
using AutoMapper;
using RavenOverflow.Services.Models;

namespace RavenOverflow.Services.AutoMapper
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