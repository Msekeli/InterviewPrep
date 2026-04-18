using FluentAssertions;
using InterviewPrep.Application.Features.Questions;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
using InterviewPrep.Domain.Enums;
using Moq;

namespace InterviewPrep.Tests.Application;

public class GenerateQuestionsHandlerTests
{
    [Fact]
    public async Task HandleAsync_Should_Return_Null_When_Session_Not_Found()
    {
        var repoMock = new Mock<IInterviewSessionRepository>();
        var serviceMock = new Mock<IQuestionService>();
        var sessionId = Guid.NewGuid();

        repoMock
            .Setup(x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InterviewSession?)null);

        var handler = new GenerateQuestionsHandler(repoMock.Object, serviceMock.Object);

        var result = await handler.HandleAsync(sessionId, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Existing_Questions_When_Already_Exist()
    {
        var repoMock = new Mock<IInterviewSessionRepository>();
        var serviceMock = new Mock<IQuestionService>();
        var sessionId = Guid.NewGuid();

        var session = new InterviewSession
        {
            Id = sessionId
        };

        var existingQuestions = new List<InterviewQuestion>
        {
            new InterviewQuestion(
                sessionId,
                QuestionCategory.Technical,
                "Existing question",
                1)
        };

        repoMock.Setup(x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        repoMock.Setup(x => x.GetQuestionsBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingQuestions);

        var handler = new GenerateQuestionsHandler(repoMock.Object, serviceMock.Object);

        var result = await handler.HandleAsync(sessionId, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Count.Should().Be(1);
        result[0].Text.Should().Be("Existing question");

        serviceMock.Verify(x => x.GenerateQuestionsAsync(It.IsAny<InterviewSession>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_Should_Generate_And_Save_Questions_When_None_Exist()
    {
        var repoMock = new Mock<IInterviewSessionRepository>();
        var serviceMock = new Mock<IQuestionService>();
        var sessionId = Guid.NewGuid();

        var session = new InterviewSession
        {
            Id = sessionId
        };

        var generatedQuestions = new List<InterviewQuestion>
        {
            new InterviewQuestion(
                sessionId,
                QuestionCategory.Technical,
                "Generated question",
                1)
        };

        repoMock.Setup(x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        repoMock.Setup(x => x.GetQuestionsBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InterviewQuestion>());

        serviceMock.Setup(x => x.GenerateQuestionsAsync(session, It.IsAny<CancellationToken>()))
            .ReturnsAsync(generatedQuestions);

        repoMock.Setup(x => x.AddQuestionsAsync(generatedQuestions, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new GenerateQuestionsHandler(repoMock.Object, serviceMock.Object);

        var result = await handler.HandleAsync(sessionId, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Count.Should().Be(1);
        result[0].Text.Should().Be("Generated question");

        repoMock.Verify(x => x.AddQuestionsAsync(generatedQuestions, It.IsAny<CancellationToken>()), Times.Once);
    }
}