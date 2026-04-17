using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterviewPrep.Infrastructure.Persistence;

public class InterviewSessionRepository : IInterviewSessionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public InterviewSessionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(InterviewSession session, CancellationToken cancellationToken = default)
    {
        await _dbContext.InterviewSessions.AddAsync(session, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<InterviewSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.InterviewSessions
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<InterviewSession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.InterviewSessions
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(InterviewSession session, CancellationToken cancellationToken = default)
    {
        _dbContext.InterviewSessions.Update(session);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddQuestionsAsync(IReadOnlyList<InterviewQuestion> questions, CancellationToken cancellationToken = default)
    {
        await _dbContext.InterviewQuestions.AddRangeAsync(questions, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InterviewQuestion>> GetQuestionsBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.InterviewQuestions
            .Where(x => x.InterviewSessionId == sessionId)
            .OrderBy(x => x.Order)
            .ToListAsync(cancellationToken);
    }
    public async Task AddAnswerAsync(InterviewAnswer answer, CancellationToken cancellationToken = default)
{
    await _dbContext.InterviewAnswers.AddAsync(answer, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
}

public async Task<IReadOnlyList<InterviewAnswer>> GetAnswersBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
{
    return await _dbContext.InterviewAnswers
        .Where(x => x.InterviewSessionId == sessionId)
        .OrderBy(x => x.Id)
        .ToListAsync(cancellationToken);
}
}