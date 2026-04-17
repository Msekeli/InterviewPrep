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
    public async Task HandleAsync_Should_Return_Null_When_Session_Does_Not_Exist()
    {
        // Arrange
        var repositoryMock = new Mock<IInterviewSessionRepository>();
        var questionServiceMock = new Mock<IQuestionService>();
        var sessionId = Guid.NewGuid();

        repositoryMock
            .Setup(x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InterviewSession?)null);

        var handler = new GenerateQuestionsHandler(
            repositoryMock.Object,
            questionServiceMock.Object);

        // Act
        var result = await handler.HandleAsync(sessionId, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        repositoryMock.Verify(
            x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()),
            Times.Once);

        questionServiceMock.Verify(
            x => x.GenerateQuestionsAsync(It.IsAny<InterviewSession>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Existing_Questions_When_They_Already_Exist()
    {
        // Arrange
        var repositoryMock = new Mock<IInterviewSessionRepository>();
        var questionServiceMock = new Mock<IQuestionService>();
        var sessionId = Guid.NewGuid();

        var session = new InterviewSession
        {
            Id = sessionId,
            UserId = Guid.NewGuid(),
            CvText = "C# .NET",
            JobSpecText = "Backend role",
            CompanyText = "Tech company",
            TargetLevel = InterviewLevel.Intermediate,
            Status = InterviewSessionStatus.Draft,
            CreatedAtUtc = DateTime.UtcNow,
            Feedback = string.Empty
        };

        var existingQuestions = new List<InterviewQuestion>
        {
            new InterviewQuestion
            {
                Id = Guid.NewGuid(),
                InterviewSessionId = sessionId,
                Category = QuestionCategory.Technical,
                Text = "What is DI?",
                Order = 1
            },
            new InterviewQuestion
            {
                Id = Guid.NewGuid(),
                InterviewSessionId = sessionId,
                Category = QuestionCategory.Behavioural,
                Text = "Tell me about teamwork.",
                Order = 2
            }
        };

        repositoryMock
            .Setup(x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        repositoryMock
            .Setup(x => x.GetQuestionsBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingQuestions);

        var handler = new GenerateQuestionsHandler(
            repositoryMock.Object,
            questionServiceMock.Object);

        // Act
        var result = await handler.HandleAsync(sessionId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result![0].Text.Should().Be("What is DI?");
        result[1].Text.Should().Be("Tell me about teamwork.");

        questionServiceMock.Verify(
            x => x.GenerateQuestionsAsync(It.IsAny<InterviewSession>(), It.IsAny<CancellationToken>()),
            Times.Never);

        repositoryMock.Verify(
            x => x.AddQuestionsAsync(It.IsAny<IReadOnlyList<InterviewQuestion>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_Should_Generate_And_Save_Questions_When_None_Exist()
    {
        // Arrange
        var repositoryMock = new Mock<IInterviewSessionRepository>();
        var questionServiceMock = new Mock<IQuestionService>();
        var sessionId = Guid.NewGuid();

        var session = new InterviewSession
        {
            Id = sessionId,
            UserId = Guid.NewGuid(),
            CvText = "Blazor and .NET",
            JobSpecText = "Full-stack developer",
            CompanyText = "Product company",
            TargetLevel = InterviewLevel.Intermediate,
            Status = InterviewSessionStatus.Draft,
            CreatedAtUtc = DateTime.UtcNow,
            Feedback = string.Empty
        };

        var generatedQuestions = new List<InterviewQuestion>
        {
            new InterviewQuestion
            {
                Id = Guid.NewGuid(),
                InterviewSessionId = sessionId,
                Category = QuestionCategory.Technical,
                Text = "Explain Blazor component lifecycle.",
                Order = 1
            }
        };

        repositoryMock
            .Setup(x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        repositoryMock
            .Setup(x => x.GetQuestionsBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InterviewQuestion>());

        questionServiceMock
            .Setup(x => x.GenerateQuestionsAsync(session, It.IsAny<CancellationToken>()))
            .ReturnsAsync(generatedQuestions);

        repositoryMock
            .Setup(x => x.AddQuestionsAsync(It.IsAny<IReadOnlyList<InterviewQuestion>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new GenerateQuestionsHandler(
            repositoryMock.Object,
            questionServiceMock.Object);

        // Act
        var result = await handler.HandleAsync(sessionId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result![0].Text.Should().Be("Explain Blazor component lifecycle.");

        questionServiceMock.Verify(
            x => x.GenerateQuestionsAsync(session, It.IsAny<CancellationToken>()),
            Times.Once);

        repositoryMock.Verify(
            x => x.AddQuestionsAsync(It.IsAny<IReadOnlyList<InterviewQuestion>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}