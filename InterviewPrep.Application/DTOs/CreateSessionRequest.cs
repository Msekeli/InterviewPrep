using InterviewPrep.Domain.Enums;

namespace InterviewPrep.Application.DTOs;

public class CreateSessionRequest
{
    public string CvText { get; set; } = string.Empty;
    public string JobSpecText { get; set; } = string.Empty;
    public string CompanyText { get; set; } = string.Empty;
    public InterviewLevel TargetLevel { get; set; }
}