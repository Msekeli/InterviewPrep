using InterviewPrep.Domain.Entities;

namespace InterviewPrep.Application.Interfaces;

public interface IQuestionService
{
    Task<IReadOnlyList<InterviewQuestion>> GenerateQuestionsAsync(
        InterviewSession session,
        CancellationToken cancellationToken = default);
}