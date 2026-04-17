using FluentAssertions;
using InterviewPrep.Domain.Entities;
using InterviewPrep.Domain.Enums;

namespace InterviewPrep.Tests.Domain;

public class InterviewQuestionTests
{
    [Fact]
    public void Should_Create_InterviewQuestion_With_Expected_Properties()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();

        // Act
        var question = new InterviewQuestion
        {
            Id = questionId,
            InterviewSessionId = sessionId,
            Category = QuestionCategory.Technical,
            Text = "Explain dependency injection in ASP.NET Core.",
            Order = 1
        };

        // Assert
        question.Id.Should().Be(questionId);
        question.InterviewSessionId.Should().Be(sessionId);
        question.Category.Should().Be(QuestionCategory.Technical);
        question.Text.Should().Be("Explain dependency injection in ASP.NET Core.");
        question.Order.Should().Be(1);
    }

    [Fact]
    public void Should_Allow_Different_Question_Categories()
    {
        // Act
        var behaviouralQuestion = new InterviewQuestion
        {
            Id = Guid.NewGuid(),
            InterviewSessionId = Guid.NewGuid(),
            Category = QuestionCategory.Behavioural,
            Text = "Tell me about a time you handled conflict.",
            Order = 2
        };

        // Assert
        behaviouralQuestion.Category.Should().Be(QuestionCategory.Behavioural);
        behaviouralQuestion.Text.Should().Contain("conflict");
        behaviouralQuestion.Order.Should().Be(2);
    }
}