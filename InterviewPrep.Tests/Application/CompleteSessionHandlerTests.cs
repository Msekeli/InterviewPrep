using FluentAssertions;
using InterviewPrep.Application.DTOs;
using InterviewPrep.Application.Features.Results;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
using InterviewPrep.Domain.Enums;
using Moq;

namespace InterviewPrep.Tests.Application;

public class CompleteSessionHandlerTests
{
    [Fact]
    public async Task HandleAsync_Should_Return_SessionNotFound_When_Session_Does_Not_Exist()
    {
        // Arrange
        var repositoryMock = new Mock<IInterviewSessionRepository>();
        var evaluatorMock = new Mock<IInterviewEvaluatorService>();
        var sessionId = Guid.NewGuid();

        repositoryMock
            .Setup(x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InterviewSession?)null);

        var handler = new CompleteSessionHandler(
            repositoryMock.Object,
            evaluatorMock.Object);

        // Act
        var result = await handler.HandleAsync(sessionId, CancellationToken.None);

        // Assert
        result.SessionNotFound.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Session not found.");
    }

    [Fact]
    public async Task HandleAsync_Should_Return_NoAnswersSubmitted_When_Answers_Are_Empty()
    {
        // Arrange
        var repositoryMock = new Mock<IInterviewSessionRepository>();
        var evaluatorMock = new Mock<IInterviewEvaluatorService>();
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

        repositoryMock
            .Setup(x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        repositoryMock
            .Setup(x => x.GetQuestionsBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InterviewQuestion>
            {
                new InterviewQuestion(
                    sessionId,
                    QuestionCategory.Technical,
                    "What is DI?",
                    1)
            });

        repositoryMock
            .Setup(x => x.GetAnswersBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InterviewAnswer>());

        var handler = new CompleteSessionHandler(
            repositoryMock.Object,
            evaluatorMock.Object);

        // Act
        var result = await handler.HandleAsync(sessionId, CancellationToken.None);

        // Assert
        result.NoAnswersSubmitted.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("You cannot complete a session without answers.");
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Existing_Result_When_Session_Is_Already_Completed()
    {
        // Arrange
        var repositoryMock = new Mock<IInterviewSessionRepository>();
        var evaluatorMock = new Mock<IInterviewEvaluatorService>();
        var sessionId = Guid.NewGuid();
        var completedAt = DateTime.UtcNow.AddMinutes(-10);

        var session = new InterviewSession
        {
            Id = sessionId,
            UserId = Guid.NewGuid(),
            CvText = "CV",
            JobSpecText = "Job",
            CompanyText = "Company",
            TargetLevel = InterviewLevel.Senior,
            Status = InterviewSessionStatus.Completed,
            CreatedAtUtc = DateTime.UtcNow.AddHours(-1),
            CompletedAtUtc = completedAt,
            OverallScore = 88m,
            Feedback = "Strong overall performance."
        };

        repositoryMock
            .Setup(x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var handler = new CompleteSessionHandler(
            repositoryMock.Object,
            evaluatorMock.Object);

        // Act
        var result = await handler.HandleAsync(sessionId, CancellationToken.None);

        // Assert
        result.SessionAlreadyCompleted.Should().BeTrue();
        result.Result.Should().NotBeNull();
        result.Result!.SessionId.Should().Be(sessionId);
        result.Result.OverallScore.Should().Be(88m);
        result.Result.Feedback.Should().Be("Strong overall performance.");
        result.Result.CompletedAtUtc.Should().Be(completedAt);

        evaluatorMock.Verify(
            x => x.EvaluateAsync(
                It.IsAny<InterviewSession>(),
                It.IsAny<IReadOnlyList<InterviewQuestion>>(),
                It.IsAny<IReadOnlyList<InterviewAnswer>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_Should_Evaluate_Update_And_Return_Result_When_Session_Is_Completed()
    {
        // Arrange
        var repositoryMock = new Mock<IInterviewSessionRepository>();
        var evaluatorMock = new Mock<IInterviewEvaluatorService>();
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
            CreatedAtUtc = DateTime.UtcNow.AddHours(-1),
            Feedback = string.Empty
        };

        var questions = new List<InterviewQuestion>
        {
            new InterviewQuestion(
                sessionId,
                QuestionCategory.Technical,
                "What is Clean Architecture?",
                1)
        };

        var answers = new List<InterviewAnswer>
        {
            new InterviewAnswer
            {
                Id = Guid.NewGuid(),
                InterviewSessionId = sessionId,
                InterviewQuestionId = questions[0].Id,
                Transcript = "It separates concerns into layers.",
                Score = null
            }
        };

        evaluatorMock
            .Setup(x => x.EvaluateAsync(session, questions, answers, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EvaluationResultDto
            {
                OverallScore = 82m,
                Feedback = "Good structure and relevant examples."
            });

        repositoryMock
            .Setup(x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        repositoryMock
            .Setup(x => x.GetQuestionsBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(questions);

        repositoryMock
            .Setup(x => x.GetAnswersBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(answers);

        repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<InterviewSession>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CompleteSessionHandler(
            repositoryMock.Object,
            evaluatorMock.Object);

        // Act
        var result = await handler.HandleAsync(sessionId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().NotBeNull();
        result.Result!.SessionId.Should().Be(sessionId);
        result.Result.OverallScore.Should().Be(82m);
        result.Result.Feedback.Should().Be("Good structure and relevant examples.");

        session.Status.Should().Be(InterviewSessionStatus.Completed);
        session.OverallScore.Should().Be(82m);
        session.Feedback.Should().Be("Good structure and relevant examples.");
        session.CompletedAtUtc.Should().NotBeNull();

        evaluatorMock.Verify(
            x => x.EvaluateAsync(session, questions, answers, It.IsAny<CancellationToken>()),
            Times.Once);

        repositoryMock.Verify(
            x => x.UpdateAsync(
                It.Is<InterviewSession>(s =>
                    s.Status == InterviewSessionStatus.Completed &&
                    s.OverallScore == 82m &&
                    s.Feedback == "Good structure and relevant examples."),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}