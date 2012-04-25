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
            var documentStore = new Mock<IDocumentStore>();
            IQuestionService questionService = new QuestionService(documentStore.Object);

            // Act & Assert.
            Assert.Throws<ValidationException>(() => questionService.Create(question));
        }

        [Fact]
        public void GivenAnQuestionWithValidData_Create_StoresAQuestion()
        {
            // Arrange.
            Question question = FakeQuestions.CreateAFakeQuestion("users/1", null);
            var documentSession = new Mock<IDocumentSession>();
            var documentStore = new Mock<IDocumentStore>();
            documentStore.Setup(x => x.OpenSession()).Returns(documentSession.Object);
            IQuestionService questionService = new QuestionService(documentStore.Object);

            // Act.
            questionService.Create(question);

            // Assert.
            documentSession.Verify(x => x.Store(question), Times.Once());
            documentSession.Verify(x => x.SaveChanges(), Times.Once());
        }

        // ReSharper restore InconsistentNaming
    }
}