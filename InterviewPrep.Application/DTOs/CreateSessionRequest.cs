using System.ComponentModel.DataAnnotations;
using InterviewPrep.Domain.Enums;

namespace InterviewPrep.Application.DTOs;

public class CreateSessionRequest
{
    [Required]
    [StringLength(20000, MinimumLength = 1)]
    public string CvText { get; set; } = string.Empty;

    [Required]
    [StringLength(10000, MinimumLength = 1)]
    public string JobSpecText { get; set; } = string.Empty;

    [StringLength(5000)]
    public string CompanyText { get; set; } = string.Empty;

    [EnumDataType(typeof(InterviewLevel))]
    public InterviewLevel? TargetLevel { get; set; }
}