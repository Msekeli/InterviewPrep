using System.ComponentModel.DataAnnotations;

namespace InterviewPrep.Application.DTOs;

public class SubmitAnswerRequest
{
    public Guid InterviewSessionId { get; set; }
    public Guid InterviewQuestionId { get; set; }

    [Required]
    [StringLength(5000, MinimumLength = 1)]
    public string Transcript { get; set; } = string.Empty;
}