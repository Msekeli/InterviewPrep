using FluentAssertions;
using InterviewPrep.Application.Features.Questions;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
using InterviewPrep.Domain.Enums;
using Moq;

namespace InterviewPrep.Tests.Application;

public class GetQuestionsHandlerTests
{
    [Fact]
    public async Task HandleAsync_Should_Return_Null_When_Session_Does_Not_Exist()
    {
        // Arrange
        var repositoryMock = new Mock<IInterviewSessionRepository>();
        var sessionId = Guid.NewGuid();

        repositoryMock
            .Setup(x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InterviewSession?)null);

        var handler = new GetQuestionsHandler(repositoryMock.Object);

        // Act
        var result = await handler.HandleAsync(sessionId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Mapped_Questions_When_Session_Exists()
    {
        // Arrange
        var repositoryMock = new Mock<IInterviewSessionRepository>();
        var sessionId = Guid.NewGuid();

        var session = new InterviewSession
        {
            Id = sessionId,
            UserId = Guid.NewGuid(),
            CvText = "CV",
            JobSpecText = "Job",
            CompanyText = "Company",
            TargetLevel = InterviewLevel.Junior,
            Status = InterviewSessionStatus.Draft,
            CreatedAtUtc = DateTime.UtcNow,
            Feedback = string.Empty
        };

        var questions = new List<InterviewQuestion>
        {
            new InterviewQuestion(
                sessionId,
                QuestionCategory.Technical,
                "What is C#?",
                1)
        };

        repositoryMock
            .Setup(x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        repositoryMock
            .Setup(x => x.GetQuestionsBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(questions);

        var handler = new GetQuestionsHandler(repositoryMock.Object);

        // Act
        var result = await handler.HandleAsync(sessionId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result![0].Id.Should().Be(questions[0].Id);
        result[0].Category.Should().Be(QuestionCategory.Technical);
        result[0].Text.Should().Be("What is C#?");
        result[0].Order.Should().Be(1);
    }
}