namespace InterviewPrep.Application.DTOs;

public class SubmitAnswerRequest
{
    public Guid InterviewSessionId { get; set; }
    public Guid InterviewQuestionId { get; set; }
    public string Transcript { get; set; } = string.Empty;
}