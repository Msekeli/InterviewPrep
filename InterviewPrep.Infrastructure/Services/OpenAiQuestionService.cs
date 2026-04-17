using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
using InterviewPrep.Domain.Enums;

namespace InterviewPrep.Infrastructure.Services;

public class OpenAiQuestionService : IQuestionService
{
    public Task<IReadOnlyList<InterviewQuestion>> GenerateQuestionsAsync(
        InterviewSession session,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<InterviewQuestion> questions =
        [
            new InterviewQuestion
            {
                Id = Guid.NewGuid(),
                InterviewSessionId = session.Id,
                Category = QuestionCategory.Cv,
                Text = "Can you walk me through your background and tell me about yourself?",
                Order = 1
            },
            new InterviewQuestion
            {
                Id = Guid.NewGuid(),
                InterviewSessionId = session.Id,
                Category = QuestionCategory.Technical,
                Text = "Tell me about a project where you used .NET and what your role was.",
                Order = 2
            },
            new InterviewQuestion
            {
                Id = Guid.NewGuid(),
                InterviewSessionId = session.Id,
                Category = QuestionCategory.Behavioural,
                Text = "Describe a challenge you faced in a team and how you handled it.",
                Order = 3
            }
        ];

        return Task.FromResult(questions);
    }
}