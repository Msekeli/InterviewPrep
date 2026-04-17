using InterviewPrep.Application.DTOs;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
using InterviewPrep.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPrep.Api.Controllers;

[ApiController]
[Route("api/sessions")]
public class InterviewSessionsController : ControllerBase
{
    private readonly IInterviewSessionRepository _interviewSessionRepository;

    public InterviewSessionsController(IInterviewSessionRepository interviewSessionRepository)
    {
        _interviewSessionRepository = interviewSessionRepository;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSession([FromBody] CreateSessionRequest request, CancellationToken cancellationToken)
    {
        var session = new InterviewSession
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(), // temporary until auth is added
            CvText = request.CvText,
            JobSpecText = request.JobSpecText,
            CompanyText = request.CompanyText,
            TargetLevel = request.TargetLevel,
            Status = InterviewSessionStatus.Draft,
            CreatedAtUtc = DateTime.UtcNow,
            Feedback = string.Empty
        };

        await _interviewSessionRepository.AddAsync(session, cancellationToken);

        return Ok(new
        {
            session.Id,
            session.Status,
            session.CreatedAtUtc
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetSessions(CancellationToken cancellationToken)
    {
        var temporaryUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var sessions = await _interviewSessionRepository.GetByUserIdAsync(temporaryUserId, cancellationToken);

        var response = sessions.Select(session => new InterviewSessionDto
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
        });

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSessionById(Guid id, CancellationToken cancellationToken)
    {
        var session = await _interviewSessionRepository.GetByIdAsync(id, cancellationToken);

        if (session is null)
        {
            return NotFound();
        }

        var response = new InterviewSessionDto
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

        return Ok(response);
    }
}