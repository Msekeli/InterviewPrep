using InterviewPrep.Application.DTOs;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;

namespace InterviewPrep.Infrastructure.Services;

public class MockInterviewEvaluatorService : IInterviewEvaluatorService
{
    public Task<EvaluationResultDto> EvaluateAsync(
        InterviewSession session,
        IReadOnlyList<InterviewQuestion> questions,
        IReadOnlyList<InterviewAnswer> answers,
        CancellationToken cancellationToken)
    {
        var answeredCount = answers.Count;
        var questionCount = questions.Count;

        decimal score;

        if (questionCount == 0)
        {
            score = 0;
        }
        else
        {
            score = Math.Round((decimal)answeredCount / questionCount * 100m, 2);
        }

        var feedback = score switch
        {
            >= 80 => "Strong interview performance. Your answers were relevant and covered most of the expected ground.",
            >= 60 => "Good foundation. Your answers showed potential, but some areas could be sharper and more detailed.",
            >= 40 => "Fair attempt. You answered some questions, but your responses need stronger structure and depth.",
            _ => "The interview needs improvement. Focus on clearer, more complete answers with examples from real experience."
        };

        return Task.FromResult(new EvaluationResultDto
        {
            OverallScore = score,
            Feedback = feedback
        });
    }
}