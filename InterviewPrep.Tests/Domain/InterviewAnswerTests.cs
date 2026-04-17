using FluentAssertions;
using InterviewPrep.Domain.Entities;

namespace InterviewPrep.Tests.Domain;

public class InterviewAnswerTests
{
    [Fact]
    public void Should_Create_InterviewAnswer_With_Expected_Properties()
    {
        // Arrange
        var answerId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var questionId = Guid.NewGuid();

        // Act
        var answer = new InterviewAnswer
        {
            Id = answerId,
            InterviewSessionId = sessionId,
            InterviewQuestionId = questionId,
            Transcript = "I used dependency injection to decouple services.",
            Score = null
        };

        // Assert
        answer.Id.Should().Be(answerId);
        answer.InterviewSessionId.Should().Be(sessionId);
        answer.InterviewQuestionId.Should().Be(questionId);
        answer.Transcript.Should().Be("I used dependency injection to decouple services.");
        answer.Score.Should().BeNull();
    }

    [Fact]
    public void Should_Allow_Score_To_Be_Set_Later()
    {
        // Arrange
        var answer = new InterviewAnswer
        {
            Id = Guid.NewGuid(),
            InterviewSessionId = Guid.NewGuid(),
            InterviewQuestionId = Guid.NewGuid(),
            Transcript = "My answer text",
            Score = null
        };

        // Act
        answer.Score = 78.5m;

        // Assert
        answer.Score.Should().Be(78.5m);
    }
}