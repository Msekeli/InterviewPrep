using InterviewPrep.Application.DTOs;
using InterviewPrep.Application.Interfaces;

namespace InterviewPrep.Application.Features.Sessions;

public class GetSessionsHandler
{
    private static readonly Guid TemporaryUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private readonly IInterviewSessionRepository _interviewSessionRepository;

    public GetSessionsHandler(IInterviewSessionRepository interviewSessionRepository)
    {
        _interviewSessionRepository = interviewSessionRepository;
    }

    public async Task<IReadOnlyList<InterviewSessionDto>> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        var sessions = await _interviewSessionRepository.GetByUserIdAsync(TemporaryUserId, cancellationToken);

        return sessions.Select(session => new InterviewSessionDto
        {
            Id = session.Id,
            CvText = session.CvText,
            JobSpecText = session.JobSpecText,
            CompanyText = session.CompanyText,
            TargetLevel = session.TargetLevel,
            Status = session.Status,
            CreatedAtUtc = session.CreatedAtUtc,
            CompletedAtUtc = session.CompletedAtUtc,
            OverallScore = session.OverallScore,
            Feedback = session.Feedback
        }).ToList();
    }
}