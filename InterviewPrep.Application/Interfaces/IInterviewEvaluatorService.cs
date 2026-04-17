using InterviewPrep.Application.DTOs;
using InterviewPrep.Domain.Entities;

namespace InterviewPrep.Application.Interfaces;

public interface IInterviewEvaluatorService
{
    Task<EvaluationResultDto> EvaluateAsync(
        InterviewSession session,
        IReadOnlyList<InterviewQuestion> questions,
        IReadOnlyList<InterviewAnswer> answers,
        CancellationToken cancellationToken);
}