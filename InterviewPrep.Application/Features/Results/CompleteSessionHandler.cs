using InterviewPrep.Application.DTOs;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
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
        var session = await _interviewSessionRepository.GetByIdAsync(
            sessionId,
            cancellationToken);

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
                Result = MapResult(session)
            };
        }

        var questions =
            await _interviewSessionRepository.GetQuestionsBySessionIdAsync(
                sessionId,
                cancellationToken);

        var answers =
            await _interviewSessionRepository.GetAnswersBySessionIdAsync(
                sessionId,
                cancellationToken);

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

        session.Observation = evaluation.Observation;
        session.Strengths = evaluation.Strengths;
        session.Communication = evaluation.Communication;
        session.GrowthOpportunity = evaluation.GrowthOpportunity;
        session.OverallImpression = evaluation.OverallImpression;
        session.NextFocus = evaluation.NextFocus;

        session.Status = InterviewSessionStatus.Completed;
        session.CompletedAtUtc = DateTime.UtcNow;

        await _interviewSessionRepository.UpdateAsync(
            session,
            cancellationToken);

        return new CompleteSessionResult
        {
            IsSuccess = true,
            Result = MapResult(session)
        };
    }

    private static InterviewResultDto MapResult(
        InterviewSession session)
    {
        return new InterviewResultDto
        {
            SessionId = session.Id,
            Observation = session.Observation,
            Strengths = session.Strengths,
            Communication = session.Communication,
            GrowthOpportunity = session.GrowthOpportunity,
            OverallImpression = session.OverallImpression,
            NextFocus = session.NextFocus,
            CompletedAtUtc =
                session.CompletedAtUtc ?? session.CreatedAtUtc
        };
    }
}