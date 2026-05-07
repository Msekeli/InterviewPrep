using InterviewPrep.Domain.Enums;

namespace InterviewPrep.Application.DTOs;

public class InterviewSessionDto
{
    public Guid Id { get; set; }

    public string CvText { get; set; } = string.Empty;
    public string JobSpecText { get; set; } = string.Empty;
    public string CompanyText { get; set; } = string.Empty;

    public InterviewLevel TargetLevel { get; set; }
    public InterviewSessionStatus Status { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }

    public string Observation { get; set; } = string.Empty;
    public string Strengths { get; set; } = string.Empty;
    public string Communication { get; set; } = string.Empty;
    public string GrowthOpportunity { get; set; } = string.Empty;
    public string OverallImpression { get; set; } = string.Empty;
    public string NextFocus { get; set; } = string.Empty;
}