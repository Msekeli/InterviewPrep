using InterviewPrep.Application.DTOs;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Enums;

namespace InterviewPrep.Application.Features.Results;

public class CompleteSessionHandler
{
    private readonly IInterviewSessionRepository _interviewSessionRepository;
    private readonly IInterviewEvaluatorService _interviewEvaluatorService;

    public CompleteSessionHandler(
        IInterviewSessionRepository interviewSessionRepository,
        IInterviewEvaluatorService interviewEvaluatorService)
    {
        _interviewSessionRepository = interviewSessionRepository;
        _interviewEvaluatorService = interviewEvaluatorService;
    }

    public async Task<CompleteSessionResult> HandleAsync(
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        var session = await _interviewSessionRepository.GetByIdAsync(sessionId, cancellationToken);

        if (session is null)
        {
            return new CompleteSessionResult
            {
                SessionNotFound = true,
                ErrorMessage = "Session not found."
            };
        }

        if (session.Status == InterviewSessionStatus.Completed)
        {
            return new CompleteSessionResult
            {
                SessionAlreadyCompleted = true,
                ErrorMessage = "Session is already completed.",
                Result = new InterviewResultDto
                {
                    SessionId = session.Id,
                    OverallScore = session.OverallScore ?? 0,
                    Feedback = session.Feedback ?? string.Empty,
                    CompletedAtUtc = session.CompletedAtUtc ?? session.CreatedAtUtc
                }
            };
        }

        var questions = await _interviewSessionRepository.GetQuestionsBySessionIdAsync(sessionId, cancellationToken);
        var answers = await _interviewSessionRepository.GetAnswersBySessionIdAsync(sessionId, cancellationToken);

        if (answers.Count == 0)
        {
            return new CompleteSessionResult
            {
                NoAnswersSubmitted = true,
                ErrorMessage = "You cannot complete a session without answers."
            };
        }

        var evaluation = await _interviewEvaluatorService.EvaluateAsync(
            session,
            questions,
            answers,
            cancellationToken);

        session.OverallScore = evaluation.OverallScore;
        session.Feedback = evaluation.Feedback;
        session.Status = InterviewSessionStatus.Completed;
        session.CompletedAtUtc = DateTime.UtcNow;

        await _interviewSessionRepository.UpdateAsync(session, cancellationToken);

        return new CompleteSessionResult
        {
            IsSuccess = true,
            Result = new InterviewResultDto
            {
                SessionId = session.Id,
                OverallScore = session.OverallScore ?? 0,
                Feedback = session.Feedback ?? string.Empty,
                CompletedAtUtc = session.CompletedAtUtc ?? DateTime.UtcNow
            }
        };
    }
}