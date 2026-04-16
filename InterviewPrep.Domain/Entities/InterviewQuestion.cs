using InterviewPrep.Domain.Enums;

namespace InterviewPrep.Domain.Entities;

public class InterviewQuestion
{
    public Guid Id { get; set; }
    public Guid InterviewSessionId { get; set; }
    public QuestionCategory Category { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }
}