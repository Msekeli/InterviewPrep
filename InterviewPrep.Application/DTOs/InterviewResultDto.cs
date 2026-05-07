namespace InterviewPrep.Application.DTOs;

public class InterviewResultDto
{
    public Guid SessionId { get; set; }

    public string Observation { get; set; } = string.Empty;
    public string Strengths { get; set; } = string.Empty;
    public string Communication { get; set; } = string.Empty;
    public string GrowthOpportunity { get; set; } = string.Empty;
    public string OverallImpression { get; set; } = string.Empty;
    public string NextFocus { get; set; } = string.Empty;

    public DateTime CompletedAtUtc { get; set; }
}