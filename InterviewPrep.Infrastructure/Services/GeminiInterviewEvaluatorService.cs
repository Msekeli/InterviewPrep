using System.Text;
using System.Text.Json;
using InterviewPrep.Application.DTOs;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace InterviewPrep.Infrastructure.Services;

public class GeminiInterviewEvaluatorService : IInterviewEvaluatorService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GeminiInterviewEvaluatorService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<EvaluationResultDto> EvaluateAsync(
        InterviewSession session,
        IReadOnlyList<InterviewQuestion> questions,
        IReadOnlyList<InterviewAnswer> answers,
        CancellationToken cancellationToken)
    {
        var apiKey = _configuration["Gemini:ApiKey"];
        var model = _configuration["Gemini:Model"] ?? "gemini-1.5-flash";

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Gemini API key is not configured.");
        }

        var prompt = BuildPrompt(session, questions, answers);

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = prompt
                        }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0.4,
                responseMimeType = "application/json"
            }
        };

        var requestJson = JsonSerializer.Serialize(requestBody);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}")
        {
            Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
        };

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Gemini evaluation request failed with status {(int)response.StatusCode}: {responseContent}");
        }

        var rawJson = ExtractTextFromGeminiResponse(responseContent);

        if (string.IsNullOrWhiteSpace(rawJson))
        {
            return BuildFallbackEvaluation(answers);
        }

        var parsed = ParseEvaluationResult(rawJson);

        if (parsed is null)
        {
            return BuildFallbackEvaluation(answers);
        }

        return parsed;
    }

    private static string BuildPrompt(
        InterviewSession session,
        IReadOnlyList<InterviewQuestion> questions,
        IReadOnlyList<InterviewAnswer> answers)
    {
        var orderedQuestions = questions
            .OrderBy(q => q.Order)
            .ToDictionary(q => q.Id, q => q);

        var sb = new StringBuilder();

        sb.AppendLine("""
You are an interview evaluator.
Evaluate the candidate's interview answers based on relevance, clarity, depth, communication, and real-world grounding.

Return ONLY valid JSON.
Do not wrap the JSON in markdown fences.
Use this exact shape:
{
  "overallScore": 0,
  "feedback": "string"
}

Scoring rules:
- overallScore must be an integer from 0 to 100
- do not give 100 unless the answers are truly exceptional
- weak, vague, short, generic, or incomplete answers should score much lower
- feedback must be honest, specific, and useful
- feedback should be 1 to 3 short paragraphs
""");

        sb.AppendLine();
        sb.AppendLine("INTERVIEW CONTEXT:");
        sb.AppendLine($"Target Level: {session.TargetLevel}");
        sb.AppendLine();
        sb.AppendLine("CV:");
        sb.AppendLine(session.CvText);
        sb.AppendLine();
        sb.AppendLine("JOB SPEC:");
        sb.AppendLine(session.JobSpecText);
        sb.AppendLine();
        sb.AppendLine("COMPANY CONTEXT:");
        sb.AppendLine(session.CompanyText);
        sb.AppendLine();
        sb.AppendLine("QUESTIONS AND ANSWERS:");

        foreach (var answer in answers)
        {
            orderedQuestions.TryGetValue(answer.InterviewQuestionId, out var question);

            sb.AppendLine();
            sb.AppendLine($"Question: {question?.Text ?? "Unknown question"}");
            sb.AppendLine($"Answer: {answer.Transcript}");
        }

        sb.AppendLine();
        sb.AppendLine("Now return the JSON only.");

        return sb.ToString();
    }

    private static string? ExtractTextFromGeminiResponse(string responseContent)
    {
        try
        {
            using var document = JsonDocument.Parse(responseContent);

            var root = document.RootElement;

            if (!root.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
            {
                return null;
            }

            var firstCandidate = candidates[0];

            if (!firstCandidate.TryGetProperty("content", out var content))
            {
                return null;
            }

            if (!content.TryGetProperty("parts", out var parts) || parts.GetArrayLength() == 0)
            {
                return null;
            }

            var firstPart = parts[0];

            if (!firstPart.TryGetProperty("text", out var textElement))
            {
                return null;
            }

            return textElement.GetString();
        }
        catch
        {
            return null;
        }
    }

    private static EvaluationResultDto? ParseEvaluationResult(string rawJson)
    {
        try
        {
            var cleaned = rawJson.Trim();

            if (cleaned.StartsWith("```"))
            {
                cleaned = cleaned
                    .Replace("```json", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Replace("```", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Trim();
            }

            using var document = JsonDocument.Parse(cleaned);
            var root = document.RootElement;

            int overallScore = 0;
            string feedback = "No feedback was returned.";

            if (root.TryGetProperty("overallScore", out var scoreElement))
            {
                if (scoreElement.ValueKind == JsonValueKind.Number)
                {
                    overallScore = scoreElement.GetInt32();
                }
                else if (scoreElement.ValueKind == JsonValueKind.String &&
                         int.TryParse(scoreElement.GetString(), out var parsedScore))
                {
                    overallScore = parsedScore;
                }
            }

            if (root.TryGetProperty("feedback", out var feedbackElement) &&
                feedbackElement.ValueKind == JsonValueKind.String)
            {
                feedback = feedbackElement.GetString() ?? feedback;
            }

            overallScore = Math.Clamp(overallScore, 0, 100);

            return new EvaluationResultDto
            {
                OverallScore = overallScore,
                Feedback = feedback
            };
        }
        catch
        {
            return null;
        }
    }

    private static EvaluationResultDto BuildFallbackEvaluation(IReadOnlyList<InterviewAnswer> answers)
    {
        var averageLength = answers.Count == 0
            ? 0
            : (int)answers.Average(a => (a.Transcript ?? string.Empty).Trim().Length);

        var score = averageLength switch
        {
            >= 900 => 78,
            >= 700 => 68,
            >= 500 => 58,
            >= 300 => 48,
            >= 150 => 38,
            _ => 25
        };

        return new EvaluationResultDto
        {
            OverallScore = score,
            Feedback =
                "Your interview was completed, but the AI evaluator response could not be fully parsed. " +
                "This fallback result is based on answer length only, so treat it as temporary. " +
                "Review your answers for clearer structure, stronger examples, and more direct relevance to the role."
        };
    }
}