using System.ComponentModel.DataAnnotations;
using AutoMapper;
using FizzWare.NBuilder;
using Moq;
using Raven.Client;
using RavenOverflow.Core.Entities;
using RavenOverflow.Services;
using RavenOverflow.Services.AutoMapper;
using RavenOverflow.Services.Interfaces;
using RavenOverflow.Services.Models;
using Xunit;

namespace RavenOverflow.Tests.Services
{
    // ReSharper disable InconsistentNaming

    public class QuestionServiceFacts
    {
        public class CreateFacts
        {
            public CreateFacts()
            {
                // WebSite requires AutoMapper mappings.
                AutoMapperBootstrapper.ConfigureMappings();
                Mapper.AssertConfigurationIsValid();
            }

            [Fact]
            public void GivenAnQuestionWithInvalidData_Create_ThrowsAnException()
            {
                // Arrange.
                QuestionInputModel questionInputModel = Builder<QuestionInputModel>.CreateNew().Build();
                questionInputModel.Subject = null; // Forces it to be invalid.
                var documentSession = new Mock<IDocumentSession>();
                IQuestionService questionService = new QuestionService();

                // Act & Assert.
                Assert.Throws<ValidationException>(
                    () => questionService.Store(questionInputModel, documentSession.Object));
                documentSession.Verify(x => x.Store(questionInputModel), Times.Never());
            }

            [Fact]
            public void GivenAnQuestionWithValidData_Create_StoresAQuestion()
            {
                // Arrange.
                QuestionInputModel createInputModel = Builder<QuestionInputModel>.CreateNew().Build();
                var documentSession = new Mock<IDocumentSession>();
                IQuestionService questionService = new QuestionService();

                // Act.
                questionService.Store(createInputModel, documentSession.Object);

                // Assert.
                documentSession.Verify(x => x.Store(It.IsAny<Question>()), Times.Once());
            }
        }
    }

    // ReSharper restore InconsistentNaming
}