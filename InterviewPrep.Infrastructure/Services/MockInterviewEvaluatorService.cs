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
        if (answers.Count == 0)
        {
            return Task.FromResult(new EvaluationResultDto
            {
                OverallScore = 0,
                Feedback = "No answers were provided for evaluation."
            });
        }

        var scores = new List<int>();

        foreach (var answer in answers)
        {
            var text = answer.Transcript?.Trim() ?? string.Empty;

            var score = CalculateScore(text);
            scores.Add(score);
        }

        var overall = scores.Count > 0
            ? (int)scores.Average()
            : 0;

        return Task.FromResult(new EvaluationResultDto
        {
            OverallScore = overall,
            Feedback = GenerateFeedback(overall)
        });
    }

    private static int CalculateScore(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 10;

        int score = 20;

        // Length scoring
        if (text.Length > 500) score += 50;
        else if (text.Length > 250) score += 35;
        else if (text.Length > 100) score += 20;
        else score += 10;

        // Keyword bonus
        string[] keywords =
        [
            "azure", ".net", "api", "sql", "react", "architecture",
            "performance", "blazor", "c#", "database"
        ];

        foreach (var keyword in keywords)
        {
            if (text.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                score += 5;
            }
        }

        // Structure bonus
        if (text.Contains("I ", StringComparison.OrdinalIgnoreCase))
            score += 5;

        if (text.Contains("for example", StringComparison.OrdinalIgnoreCase))
            score += 5;

        if (text.Split('.').Length > 3)
            score += 5;

        return Math.Clamp(score, 0, 100);
    }

    private static string GenerateFeedback(int score)
    {
        return score switch
        {
            >= 80 =>
                "Strong performance. Your answers are well-structured, detailed, and demonstrate real-world experience with clear technical depth.",

            >= 60 =>
                "Good overall performance. You show solid understanding, but your answers could benefit from more specific examples and deeper technical detail.",

            >= 40 =>
                "Basic understanding is visible, but your answers are often too generic or brief. Try to include real examples and clearer structure.",

            _ =>
                "Your answers need significant improvement. Focus on clarity, structure, and providing real-world examples to support your responses."
        };
    }
}