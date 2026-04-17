using InterviewPrep.Domain.Entities;
using InterviewPrep.Application.DTOs;

namespace InterviewPrep.Application.Interfaces;

public interface IInterviewEvaluatorService
{
    Task<InterviewResultDto> EvaluateAsync(
        InterviewSession session,
        IReadOnlyList<InterviewQuestion> questions,
        IReadOnlyList<InterviewAnswer> answers,
        CancellationToken cancellationToken = default);
}