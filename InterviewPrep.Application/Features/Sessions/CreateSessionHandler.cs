using InterviewPrep.Application.DTOs;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
using InterviewPrep.Domain.Enums;

namespace InterviewPrep.Application.Features.Sessions;

public class CreateSessionHandler
{
    private static readonly Guid TemporaryUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private readonly IInterviewSessionRepository _interviewSessionRepository;

    public CreateSessionHandler(IInterviewSessionRepository interviewSessionRepository)
    {
        _interviewSessionRepository = interviewSessionRepository;
    }

    public async Task<InterviewSessionDto> HandleAsync(
        CreateSessionRequest request,
        CancellationToken cancellationToken = default)
    {
        var session = new InterviewSession
        {
            Id = Guid.NewGuid(),
            UserId = TemporaryUserId,
            CvText = request.CvText,
            JobSpecText = request.JobSpecText,
            CompanyText = request.CompanyText,
            TargetLevel = request.TargetLevel,
            Status = InterviewSessionStatus.Draft,
            CreatedAtUtc = DateTime.UtcNow,
            Feedback = string.Empty
        };

        await _interviewSessionRepository.AddAsync(session, cancellationToken);

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