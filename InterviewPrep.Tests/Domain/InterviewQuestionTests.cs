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
        var sessionId = Guid.NewGuid();

        // Act
        var question = new InterviewQuestion(
            sessionId,
            QuestionCategory.Technical,
            "Explain dependency injection in ASP.NET Core.",
            1);

        // Assert
        question.Id.Should().NotBeEmpty();
        question.InterviewSessionId.Should().Be(sessionId);
        question.Category.Should().Be(QuestionCategory.Technical);
        question.Text.Should().Be("Explain dependency injection in ASP.NET Core.");
        question.Order.Should().Be(1);
    }

    [Fact]
    public void Should_Allow_Different_Question_Categories()
    {
        // Arrange
        var sessionId = Guid.NewGuid();

        // Act
        var behaviouralQuestion = new InterviewQuestion(
            sessionId,
            QuestionCategory.Behavioural,
            "Tell me about a time you handled conflict.",
            2);

        // Assert
        behaviouralQuestion.Category.Should().Be(QuestionCategory.Behavioural);
        behaviouralQuestion.Text.Should().Contain("conflict");
        behaviouralQuestion.Order.Should().Be(2);
    }

    [Fact]
    public void Should_Throw_When_Text_Is_Empty()
    {
        // Arrange
        var sessionId = Guid.NewGuid();

        // Act
        var action = () => new InterviewQuestion(
            sessionId,
            QuestionCategory.Technical,
            "",
            1);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("Question text cannot be empty.");
    }

    [Fact]
    public void Should_Update_Order_When_SetOrder_Is_Valid()
    {
        // Arrange
        var question = new InterviewQuestion(
            Guid.NewGuid(),
            QuestionCategory.Technical,
            "What is Clean Architecture?",
            1);

        // Act
        question.SetOrder(3);

        // Assert
        question.Order.Should().Be(3);
    }

    [Fact]
    public void Should_Throw_When_SetOrder_Is_Less_Than_Or_Equal_To_Zero()
    {
        // Arrange
        var question = new InterviewQuestion(
            Guid.NewGuid(),
            QuestionCategory.Technical,
            "What is Clean Architecture?",
            1);

        // Act
        var action = () => question.SetOrder(0);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("Order must be greater than zero.");
    }
}