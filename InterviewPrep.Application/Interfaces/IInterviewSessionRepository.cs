using InterviewPrep.Domain.Entities;

namespace InterviewPrep.Application.Interfaces;

public interface IInterviewSessionRepository
{
    Task AddAsync(InterviewSession session, CancellationToken cancellationToken = default);
    Task<InterviewSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InterviewSession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task UpdateAsync(InterviewSession session, CancellationToken cancellationToken = default);
    Task AddQuestionsAsync(IReadOnlyList<InterviewQuestion> questions, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InterviewQuestion>> GetQuestionsBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task AddAnswerAsync(InterviewAnswer answer, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InterviewAnswer>> GetAnswersBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
}