using InterviewPrep.Application.DTOs;
using InterviewPrep.Application.Features.Questions;
using InterviewPrep.Application.Features.Results;
using InterviewPrep.Application.Features.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPrep.Api.Controllers;

[ApiController]
[Route("api/sessions")]
public class InterviewSessionsController : ControllerBase
{
    private readonly CreateSessionHandler _createSessionHandler;
    private readonly GetSessionsHandler _getSessionsHandler;
    private readonly GetSessionByIdHandler _getSessionByIdHandler;
    private readonly GenerateQuestionsHandler _generateQuestionsHandler;
    private readonly GetQuestionsHandler _getQuestionsHandler;
    private readonly SubmitAnswerHandler _submitAnswerHandler;
    private readonly GetAnswersHandler _getAnswersHandler;
    private readonly CompleteSessionHandler _completeSessionHandler;

    public InterviewSessionsController(
        CreateSessionHandler createSessionHandler,
        GetSessionsHandler getSessionsHandler,
        GetSessionByIdHandler getSessionByIdHandler,
        GenerateQuestionsHandler generateQuestionsHandler,
        GetQuestionsHandler getQuestionsHandler,
        SubmitAnswerHandler submitAnswerHandler,
        GetAnswersHandler getAnswersHandler,
        CompleteSessionHandler completeSessionHandler)
    {
        _createSessionHandler = createSessionHandler;
        _getSessionsHandler = getSessionsHandler;
        _getSessionByIdHandler = getSessionByIdHandler;
        _generateQuestionsHandler = generateQuestionsHandler;
        _getQuestionsHandler = getQuestionsHandler;
        _submitAnswerHandler = submitAnswerHandler;
        _getAnswersHandler = getAnswersHandler;
        _completeSessionHandler = completeSessionHandler;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSession(
        [FromBody] CreateSessionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createSessionHandler.HandleAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetSessions(CancellationToken cancellationToken)
    {
        var result = await _getSessionsHandler.HandleAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSessionById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _getSessionByIdHandler.HandleAsync(id, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost("{id:guid}/questions")]
    public async Task<IActionResult> GenerateQuestions(Guid id, CancellationToken cancellationToken)
    {
        var result = await _generateQuestionsHandler.HandleAsync(id, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet("{id:guid}/questions")]
    public async Task<IActionResult> GetQuestions(Guid id, CancellationToken cancellationToken)
    {
        var result = await _getQuestionsHandler.HandleAsync(id, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost("{id:guid}/answers")]
    public async Task<IActionResult> SubmitAnswer(
        Guid id,
        [FromBody] SubmitAnswerRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _submitAnswerHandler.HandleAsync(id, request, cancellationToken);

        if (result.SessionNotFound)
        {
            return NotFound(result.ErrorMessage);
        }

        if (result.InvalidQuestion)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Answer);
    }

    [HttpGet("{id:guid}/answers")]
    public async Task<IActionResult> GetAnswers(Guid id, CancellationToken cancellationToken)
    {
        var result = await _getAnswersHandler.HandleAsync(id, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> CompleteSession(Guid id, CancellationToken cancellationToken)
    {
        var result = await _completeSessionHandler.HandleAsync(id, cancellationToken);

        if (result.SessionNotFound)
        {
            return NotFound(result.ErrorMessage);
        }

        if (result.NoAnswersSubmitted)
        {
            return BadRequest(result.ErrorMessage);
        }

        if (result.SessionAlreadyCompleted)
        {
            return Ok(result.Result);
        }

        return Ok(result.Result);
    }
}