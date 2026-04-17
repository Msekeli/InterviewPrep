using InterviewPrep.Application.DTOs;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
using InterviewPrep.Domain.Enums;

namespace InterviewPrep.Application.Features.Results;

public class SubmitAnswerHandler
{
    private readonly IInterviewSessionRepository _interviewSessionRepository;

    public SubmitAnswerHandler(IInterviewSessionRepository interviewSessionRepository)
    {
        _interviewSessionRepository = interviewSessionRepository;
    }

    public async Task<SubmitAnswerResult> HandleAsync(
        Guid sessionId,
        SubmitAnswerRequest request,
        CancellationToken cancellationToken)
    {
        var session = await _interviewSessionRepository.GetByIdAsync(sessionId, cancellationToken);

        if (session is null)
        {
            return new SubmitAnswerResult
            {
                SessionNotFound = true,
                ErrorMessage = "Session not found."
            };
        }

        var questions = await _interviewSessionRepository.GetQuestionsBySessionIdAsync(sessionId, cancellationToken);
        var questionExists = questions.Any(x => x.Id == request.InterviewQuestionId);

        if (!questionExists)
        {
            return new SubmitAnswerResult
            {
                InvalidQuestion = true,
                ErrorMessage = "Question does not belong to this session."
            };
        }

        var answer = new InterviewAnswer
        {
            Id = Guid.NewGuid(),
            InterviewSessionId = sessionId,
            InterviewQuestionId = request.InterviewQuestionId,
            Transcript = request.Transcript,
            Score = null
        };

        await _interviewSessionRepository.AddAnswerAsync(answer, cancellationToken);

        if (session.Status == InterviewSessionStatus.Draft)
        {
            session.Status = InterviewSessionStatus.InProgress;
            await _interviewSessionRepository.UpdateAsync(session, cancellationToken);
        }

        return new SubmitAnswerResult
        {
            IsSuccess = true,
            Answer = new AnswerDto
            {
                Id = answer.Id,
                InterviewSessionId = answer.InterviewSessionId,
                InterviewQuestionId = answer.InterviewQuestionId,
                Transcript = answer.Transcript,
                Score = answer.Score
            }
        };
    }
}