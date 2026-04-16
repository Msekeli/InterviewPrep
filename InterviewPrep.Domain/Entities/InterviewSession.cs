using InterviewPrep.Domain.Enums;

namespace InterviewPrep.Domain.Entities;

public class InterviewSession
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CvText { get; set; } = string.Empty;
    public string JobSpecText { get; set; } = string.Empty;
    public string CompanyText { get; set; } = string.Empty;
    public InterviewLevel TargetLevel { get; set; }
    public InterviewSessionStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public decimal? OverallScore { get; set; }
    public string Feedback { get; set; } = string.Empty;
}