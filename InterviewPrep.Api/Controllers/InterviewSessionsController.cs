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
    private static readonly Guid TemporaryUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private readonly IInterviewSessionRepository _interviewSessionRepository;
    private readonly IQuestionService _questionService;

    public InterviewSessionsController(
        IInterviewSessionRepository interviewSessionRepository,
        IQuestionService questionService)
    {
        _interviewSessionRepository = interviewSessionRepository;
        _questionService = questionService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSession([FromBody] CreateSessionRequest request, CancellationToken cancellationToken)
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
        var sessions = await _interviewSessionRepository.GetByUserIdAsync(TemporaryUserId, cancellationToken);

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

        return Ok(new InterviewSessionDto
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
    }

    [HttpPost("{id:guid}/questions")]
    public async Task<IActionResult> GenerateQuestions(Guid id, CancellationToken cancellationToken)
    {
        var session = await _interviewSessionRepository.GetByIdAsync(id, cancellationToken);

        if (session is null)
        {
            return NotFound();
        }

        var existingQuestions = await _interviewSessionRepository.GetQuestionsBySessionIdAsync(id, cancellationToken);

        if (existingQuestions.Count > 0)
        {
            return Ok(existingQuestions.Select(question => new
            {
                question.Id,
                question.Category,
                question.Text,
                question.Order
            }));
        }

        var questions = await _questionService.GenerateQuestionsAsync(session, cancellationToken);

        await _interviewSessionRepository.AddQuestionsAsync(questions, cancellationToken);

        return Ok(questions.Select(question => new
        {
            question.Id,
            question.Category,
            question.Text,
            question.Order
        }));
    }

    [HttpGet("{id:guid}/questions")]
    public async Task<IActionResult> GetQuestions(Guid id, CancellationToken cancellationToken)
    {
        var session = await _interviewSessionRepository.GetByIdAsync(id, cancellationToken);

        if (session is null)
        {
            return NotFound();
        }

        var questions = await _interviewSessionRepository.GetQuestionsBySessionIdAsync(id, cancellationToken);

        return Ok(questions.Select(question => new
        {
            question.Id,
            question.Category,
            question.Text,
            question.Order
        }));
    }
}