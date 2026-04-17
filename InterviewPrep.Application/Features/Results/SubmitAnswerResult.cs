using InterviewPrep.Application.DTOs;

namespace InterviewPrep.Application.Features.Results;

public class SubmitAnswerResult
{
    public bool IsSuccess { get; set; }
    public bool SessionNotFound { get; set; }
    public bool InvalidQuestion { get; set; }
    public string? ErrorMessage { get; set; }
    public AnswerDto? Answer { get; set; }
}