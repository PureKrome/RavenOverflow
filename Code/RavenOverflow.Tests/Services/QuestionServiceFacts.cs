using System.ComponentModel.DataAnnotations;
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
        [Fact]
        public void GivenAnQuestionWithInvalidData_Create_ThrowsAnException()
        {
            // Arrange.
            Question question = FakeQuestions.CreateAFakeQuestion(null, null); // No user created this question.
            var documentSession = new Mock<IDocumentSession>();
            IQuestionService questionService = new QuestionService();

            // Act & Assert.
            Assert.Throws<ValidationException>(() => questionService.Create(question, documentSession.Object));
            documentSession.Verify(x => x.Store(question), Times.Never());
        }

        [Fact]
        public void GivenAnQuestionWithValidData_Create_StoresAQuestion()
        {
            // Arrange.
            Question question = FakeQuestions.CreateAFakeQuestion("users/1", null);
            var documentSession = new Mock<IDocumentSession>();
            IQuestionService questionService = new QuestionService();

            // Act.
            questionService.Create(question, documentSession.Object);

            // Assert.
            documentSession.Verify(x => x.Store(question), Times.Once());
        }

        // ReSharper restore InconsistentNaming
    }
}