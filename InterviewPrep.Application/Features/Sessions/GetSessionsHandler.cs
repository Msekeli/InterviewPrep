using InterviewPrep.Application.DTOs;
using InterviewPrep.Application.Interfaces;

namespace InterviewPrep.Application.Features.Sessions;

public class GetSessionByIdHandler
{
    private readonly IInterviewSessionRepository _interviewSessionRepository;

    public GetSessionByIdHandler(IInterviewSessionRepository interviewSessionRepository)
    {
        _interviewSessionRepository = interviewSessionRepository;
    }

    public async Task<InterviewSessionDto?> HandleAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var session = await _interviewSessionRepository.GetByIdAsync(id, cancellationToken);

        if (session is null)
        {
            return null;
        }

        return new InterviewSessionDto
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
        };
    }
}