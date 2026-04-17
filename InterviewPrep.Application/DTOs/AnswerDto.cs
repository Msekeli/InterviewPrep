namespace InterviewPrep.Application.DTOs;

public class AnswerDto
{
    public Guid Id { get; set; }
    public Guid InterviewSessionId { get; set; }
    public Guid InterviewQuestionId { get; set; }
    public string Transcript { get; set; } = string.Empty;
    public decimal? Score { get; set; }
}