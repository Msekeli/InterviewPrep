using InterviewPrep.Application.DTOs;

namespace InterviewPrep.Application.Features.Results;

public class CompleteSessionResult
{
    public bool IsSuccess { get; set; }
    public bool SessionNotFound { get; set; }
    public bool SessionAlreadyCompleted { get; set; }
    public bool NoAnswersSubmitted { get; set; }
    public string? ErrorMessage { get; set; }
    public InterviewResultDto? Result { get; set; }
}