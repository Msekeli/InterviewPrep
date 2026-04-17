using FluentAssertions;
using InterviewPrep.Application.DTOs;
using InterviewPrep.Application.Features.Results;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
using InterviewPrep.Domain.Enums;
using Moq;

namespace InterviewPrep.Tests.Application;

public class SubmitAnswerHandlerTests
{
    [Fact]
    public async Task HandleAsync_Should_Return_SessionNotFound_When_Session_Does_Not_Exist()
    {
        // Arrange
        var repositoryMock = new Mock<IInterviewSessionRepository>();
        var sessionId = Guid.NewGuid();

        repositoryMock
            .Setup(x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InterviewSession?)null);

        var handler = new SubmitAnswerHandler(repositoryMock.Object);

        var request = new SubmitAnswerRequest
        {
            InterviewQuestionId = Guid.NewGuid(),
            Transcript = "My answer"
        };

        // Act
        var result = await handler.HandleAsync(sessionId, request, CancellationToken.None);

        // Assert
        result.SessionNotFound.Should().BeTrue();
        result.InvalidQuestion.Should().BeFalse();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Session not found.");
    }

    [Fact]
    public async Task HandleAsync_Should_Return_InvalidQuestion_When_Question_Does_Not_Belong_To_Session()
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
            Status = InterviewSessionStatus.Draft,
            CreatedAtUtc = DateTime.UtcNow,
            Feedback = string.Empty
        };

        repositoryMock
            .Setup(x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        repositoryMock
            .Setup(x => x.GetQuestionsBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InterviewQuestion>
            {
                new InterviewQuestion
                {
                    Id = Guid.NewGuid(),
                    InterviewSessionId = sessionId,
                    Category = QuestionCategory.Technical,
                    Text = "What is DI?",
                    Order = 1
                }
            });

        var handler = new SubmitAnswerHandler(repositoryMock.Object);

        var request = new SubmitAnswerRequest
        {
            InterviewQuestionId = Guid.NewGuid(),
            Transcript = "My answer"
        };

        // Act
        var result = await handler.HandleAsync(sessionId, request, CancellationToken.None);

        // Assert
        result.SessionNotFound.Should().BeFalse();
        result.InvalidQuestion.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Question does not belong to this session.");
    }

    [Fact]
    public async Task HandleAsync_Should_Save_Answer_And_Move_Draft_To_InProgress()
    {
        // Arrange
        var repositoryMock = new Mock<IInterviewSessionRepository>();
        var sessionId = Guid.NewGuid();
        var questionId = Guid.NewGuid();

        var session = new InterviewSession
        {
            Id = sessionId,
            UserId = Guid.NewGuid(),
            CvText = "CV",
            JobSpecText = "Job",
            CompanyText = "Company",
            TargetLevel = InterviewLevel.Intermediate,
            Status = InterviewSessionStatus.Draft,
            CreatedAtUtc = DateTime.UtcNow,
            Feedback = string.Empty
        };

        repositoryMock
            .Setup(x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        repositoryMock
            .Setup(x => x.GetQuestionsBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InterviewQuestion>
            {
                new InterviewQuestion
                {
                    Id = questionId,
                    InterviewSessionId = sessionId,
                    Category = QuestionCategory.Technical,
                    Text = "What is DI?",
                    Order = 1
                }
            });

        repositoryMock
            .Setup(x => x.AddAnswerAsync(It.IsAny<InterviewAnswer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<InterviewSession>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new SubmitAnswerHandler(repositoryMock.Object);

        var request = new SubmitAnswerRequest
        {
            InterviewQuestionId = questionId,
            Transcript = "Dependency injection helps decouple services."
        };

        // Act
        var result = await handler.HandleAsync(sessionId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.SessionNotFound.Should().BeFalse();
        result.InvalidQuestion.Should().BeFalse();
        result.Answer.Should().NotBeNull();
        result.Answer!.InterviewSessionId.Should().Be(sessionId);
        result.Answer.InterviewQuestionId.Should().Be(questionId);
        result.Answer.Transcript.Should().Be("Dependency injection helps decouple services.");
        session.Status.Should().Be(InterviewSessionStatus.InProgress);

        repositoryMock.Verify(
            x => x.AddAnswerAsync(It.IsAny<InterviewAnswer>(), It.IsAny<CancellationToken>()),
            Times.Once);

        repositoryMock.Verify(
            x => x.UpdateAsync(It.Is<InterviewSession>(s => s.Status == InterviewSessionStatus.InProgress), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Should_Save_Answer_Without_Updating_Status_When_Already_InProgress()
    {
        // Arrange
        var repositoryMock = new Mock<IInterviewSessionRepository>();
        var sessionId = Guid.NewGuid();
        var questionId = Guid.NewGuid();

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

        repositoryMock
            .Setup(x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        repositoryMock
            .Setup(x => x.GetQuestionsBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InterviewQuestion>
            {
                new InterviewQuestion
                {
                    Id = questionId,
                    InterviewSessionId = sessionId,
                    Category = QuestionCategory.Technical,
                    Text = "What is EF Core?",
                    Order = 1
                }
            });

        repositoryMock
            .Setup(x => x.AddAnswerAsync(It.IsAny<InterviewAnswer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new SubmitAnswerHandler(repositoryMock.Object);

        var request = new SubmitAnswerRequest
        {
            InterviewQuestionId = questionId,
            Transcript = "EF Core is an ORM."
        };

        // Act
        var result = await handler.HandleAsync(sessionId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        session.Status.Should().Be(InterviewSessionStatus.InProgress);

        repositoryMock.Verify(
            x => x.AddAnswerAsync(It.IsAny<InterviewAnswer>(), It.IsAny<CancellationToken>()),
            Times.Once);

        repositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<InterviewSession>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}