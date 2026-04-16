namespace InterviewPrep.Domain.Entities;

public class InterviewAnswer
{
    public Guid Id { get; set; }
    public Guid InterviewSessionId { get; set; }
    public Guid InterviewQuestionId { get; set; }
    public string Transcript { get; set; } = string.Empty;
    public decimal? Score { get; set; }
}