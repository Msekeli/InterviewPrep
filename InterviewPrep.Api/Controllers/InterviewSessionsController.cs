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
}