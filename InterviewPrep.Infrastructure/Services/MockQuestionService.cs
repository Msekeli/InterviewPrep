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
                "Tell us about a time you modernized a legacy system. What challenges did you face during the .NET 4 → .NET 9 migration, and how did you ensure the new Blazor components integrated smoothly?",
                1),

            new InterviewQuestion(
                session.Id,
                QuestionCategory.Technical,
                "Walk us through your approach to translating Figma designs into pixel-perfect React components. How do you ensure consistency across devices and browsers?",
                2),

            new InterviewQuestion(
                session.Id,
                QuestionCategory.Behavioural,
                "Describe a situation where you collaborated with designers, backend engineers, or product teams. How did you handle conflicting requirements or feedback?",
                3),

            new InterviewQuestion(
                session.Id,
                QuestionCategory.Technical,
                "Can you explain how you integrated a frontend feature with Azure Functions? What tools did you use to test and validate the endpoints?",
                4),

            new InterviewQuestion(
                session.Id,
                QuestionCategory.Technical,
                "How do you typically work with stored procedures in SQL Server from .NET? Describe a scenario where you retrieved and displayed complex production data.",
                5),

            new InterviewQuestion(
                session.Id,
                QuestionCategory.Behavioural,
                "Tell us about a project you built independently or took major ownership of. What architectural and UI decisions did you make?",
                6)
        };

        return Task.FromResult<IReadOnlyList<InterviewQuestion>>(questions);
    }
}