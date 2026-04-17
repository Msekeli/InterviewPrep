using FluentAssertions;
using InterviewPrep.Application.Features.Sessions;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
using InterviewPrep.Domain.Enums;
using Moq;

namespace InterviewPrep.Tests.Application;

public class GetSessionByIdHandlerTests
{
    [Fact]
    public async Task HandleAsync_Should_Return_Dto_When_Session_Exists()
    {
        // Arrange
        var repositoryMock = new Mock<IInterviewSessionRepository>();
        var sessionId = Guid.NewGuid();

        var session = new InterviewSession
        {
            Id = sessionId,
            UserId = Guid.NewGuid(),
            CvText = "C# and .NET experience",
            JobSpecText = "Build scalable APIs",
            CompanyText = "Fintech company",
            TargetLevel = InterviewLevel.Intermediate,
            Status = InterviewSessionStatus.InProgress,
            CreatedAtUtc = DateTime.UtcNow.AddHours(-1),
            CompletedAtUtc = null,
            OverallScore = null,
            Feedback = string.Empty
        };

        repositoryMock
            .Setup(x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var handler = new GetSessionByIdHandler(repositoryMock.Object);

        // Act
        var result = await handler.HandleAsync(sessionId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(session.Id);
        result.CvText.Should().Be(session.CvText);
        result.JobSpecText.Should().Be(session.JobSpecText);
        result.CompanyText.Should().Be(session.CompanyText);
        result.TargetLevel.Should().Be(session.TargetLevel);
        result.Status.Should().Be(session.Status);
        result.CreatedAtUtc.Should().Be(session.CreatedAtUtc);
        result.CompletedAtUtc.Should().Be(session.CompletedAtUtc);
        result.OverallScore.Should().Be(session.OverallScore);
        result.Feedback.Should().Be(session.Feedback);

        repositoryMock.Verify(
            x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Null_When_Session_Does_Not_Exist()
    {
        // Arrange
        var repositoryMock = new Mock<IInterviewSessionRepository>();
        var sessionId = Guid.NewGuid();

        repositoryMock
            .Setup(x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InterviewSession?)null);

        var handler = new GetSessionByIdHandler(repositoryMock.Object);

        // Act
        var result = await handler.HandleAsync(sessionId, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        repositoryMock.Verify(
            x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}