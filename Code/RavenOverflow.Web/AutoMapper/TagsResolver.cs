using System;
using System.Collections.Generic;
using AutoMapper;
using RavenOverflow.Web.Areas.Question.Models.ViewModels;

namespace RavenOverflow.Web.AutoMapper
{
    public class TagsResolver : ValueResolver<CreateInputModel, ICollection<string>>
    {
        protected override ICollection<string> ResolveCore(CreateInputModel createInputModel)
        {
            if (createInputModel == null || createInputModel.Tags == null)
            {
                return null;
            }

            return createInputModel.Tags.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}