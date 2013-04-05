using System;
using FizzWare.NBuilder;
using Moq;
using Raven.Client;
using RavenOverflow.Core.Entities;
using RavenOverflow.Core.Services;
using RavenOverflow.FakeData;
using RavenOverflow.Services;
using Xunit;

namespace RavenOverflow.Tests.Services
{
    // ReSharper disable InconsistentNaming

    public class QuestionServiceFacts
    {
        public class CreateFacts
        {
            [Fact]
            public void GivenAnQuestionWithInvalidData_Create_ThrowsAnException()
            {
                // Arrange.
                var question = Builder<Question>.CreateNew().Build();
                question.Subject = null; // Forces it to be invalid.
                var documentSession = new Mock<IDocumentSession>();
                IQuestionService questionService = new QuestionService(documentSession.Object);

                // Act & Assert.
                Assert.Throws<ArgumentNullException>(() => questionService.Store(question));
                documentSession.Verify(x => x.Store(It.IsAny<Question>()), Times.Never());
            }

            [Fact]
            public void GivenAnQuestionWithValidData_Create_StoresAQuestion()
            {
                // Arrange.
                var question = FakeQuestions.CreateAFakeQuestion("someUserId", null);
                var documentSession = new Mock<IDocumentSession>();
                IQuestionService questionService = new QuestionService(documentSession.Object);

                // Act.
                questionService.Store(question);

                // Assert.
                documentSession.Verify(x => x.Store(It.IsAny<Question>()), Times.Once());
            }
        }
    }

    // ReSharper restore InconsistentNaming
}