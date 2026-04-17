using FluentAssertions;
using InterviewPrep.Application.Features.Results;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
using InterviewPrep.Domain.Enums;
using Moq;

namespace InterviewPrep.Tests.Application;

public class GetAnswersHandlerTests
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

        var handler = new GetAnswersHandler(repositoryMock.Object);

        // Act
        var result = await handler.HandleAsync(sessionId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Mapped_Answers_When_Session_Exists()
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
            TargetLevel = InterviewLevel.Intermediate,
            Status = InterviewSessionStatus.InProgress,
            CreatedAtUtc = DateTime.UtcNow,
            Feedback = string.Empty
        };

        var answers = new List<InterviewAnswer>
        {
            new InterviewAnswer
            {
                Id = Guid.NewGuid(),
                InterviewSessionId = sessionId,
                InterviewQuestionId = Guid.NewGuid(),
                Transcript = "I used REST APIs in production.",
                Score = 75m
            }
        };

        repositoryMock
            .Setup(x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        repositoryMock
            .Setup(x => x.GetAnswersBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(answers);

        var handler = new GetAnswersHandler(repositoryMock.Object);

        // Act
        var result = await handler.HandleAsync(sessionId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result![0].Id.Should().Be(answers[0].Id);
        result[0].InterviewSessionId.Should().Be(sessionId);
        result[0].InterviewQuestionId.Should().Be(answers[0].InterviewQuestionId);
        result[0].Transcript.Should().Be("I used REST APIs in production.");
        result[0].Score.Should().Be(75m);
    }
}