using InterviewPrep.Domain.Enums;

namespace InterviewPrep.Application.DTOs;

public class QuestionDto
{
    public Guid Id { get; set; }
    public QuestionCategory Category { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }
}