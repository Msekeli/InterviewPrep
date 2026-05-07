using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
using InterviewPrep.Domain.Enums;

namespace InterviewPrep.Infrastructure.Services;

public class MockQuestionService : IQuestionService
{
    public Task<IReadOnlyList<InterviewQuestion>> GenerateQuestionsAsync(
        InterviewSession session,
        CancellationToken cancellationToken = default)
    {
        var questions = new List<InterviewQuestion>
        {
            new InterviewQuestion(
                session.Id,
                QuestionCategory.Behavioural,
                "I’ve reviewed your background and noticed experience across both customer-facing platforms and enterprise systems. Could you briefly tell me about your journey as a software engineer?",
                1),

            new InterviewQuestion(
                session.Id,
                QuestionCategory.Technical,
                "You’ve worked with both React and Blazor. How do you decide which one to use for a project?",
                2),

            new InterviewQuestion(
                session.Id,
                QuestionCategory.Technical,
                "You’ve also worked with Azure. What’s your understanding of the shared responsibility model in cloud computing?",
                3),

            new InterviewQuestion(
                session.Id,
                QuestionCategory.Technical,
                "As systems grow, how do you make sure your code stays maintainable and aligned with SOLID principles?",
                4),

            new InterviewQuestion(
                session.Id,
                QuestionCategory.Behavioural,
                "What’s a project you’re particularly proud of, and what made it meaningful to you?",
                5)
        };

        return Task.FromResult<IReadOnlyList<InterviewQuestion>>(questions);
    }
}