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
    public async Task HandleAsync_Should_Return_False_When_Session_Not_Found()
    {
        var repoMock = new Mock<IInterviewSessionRepository>();
        var sessionId = Guid.NewGuid();
        var request = new SubmitAnswerRequest
        {
            InterviewQuestionId = Guid.NewGuid(),
            Transcript = "answer"
        };

        repoMock
            .Setup(x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InterviewSession?)null);

        var handler = new SubmitAnswerHandler(repoMock.Object);

        var result = await handler.HandleAsync(sessionId, request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.SessionNotFound.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_Should_Return_False_When_Question_Not_Found()
    {
        var repoMock = new Mock<IInterviewSessionRepository>();
        var sessionId = Guid.NewGuid();
        var request = new SubmitAnswerRequest
        {
            InterviewQuestionId = Guid.NewGuid(),
            Transcript = "answer"
        };

        var session = new InterviewSession { Id = sessionId };

        repoMock
            .Setup(x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        repoMock
            .Setup(x => x.GetQuestionsBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InterviewQuestion>());

        var handler = new SubmitAnswerHandler(repoMock.Object);

        var result = await handler.HandleAsync(sessionId, request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.InvalidQuestion.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_Should_Save_Answer_When_Valid()
    {
        var repoMock = new Mock<IInterviewSessionRepository>();
        var sessionId = Guid.NewGuid();

        var session = new InterviewSession { Id = sessionId };

        var question = new InterviewQuestion(
            sessionId,
            QuestionCategory.Technical,
            "What is .NET?",
            1);

        var request = new SubmitAnswerRequest
        {
            InterviewQuestionId = question.Id,
            Transcript = "My answer"
        };

        var questions = new List<InterviewQuestion> { question };

        repoMock
            .Setup(x => x.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        repoMock
            .Setup(x => x.GetQuestionsBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(questions);

        repoMock
            .Setup(x => x.AddAnswerAsync(It.IsAny<InterviewAnswer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new SubmitAnswerHandler(repoMock.Object);

        var result = await handler.HandleAsync(sessionId, request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Answer.Should().NotBeNull();
        result.Answer!.InterviewSessionId.Should().Be(sessionId);
        result.Answer.InterviewQuestionId.Should().Be(question.Id);
        result.Answer.Transcript.Should().Be("My answer");

        repoMock.Verify(
            x => x.AddAnswerAsync(
                It.Is<InterviewAnswer>(a =>
                    a.InterviewSessionId == sessionId &&
                    a.InterviewQuestionId == question.Id &&
                    a.Transcript == "My answer"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}