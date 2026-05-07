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
            throw new InvalidOperationException("Gemini API key is not configured.");

        var prompt = BuildPrompt(session, questions, answers);

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
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
            throw new InvalidOperationException(
                $"Gemini evaluation failed with status {(int)response.StatusCode}: {responseContent}");

        var rawJson = ExtractTextFromGeminiResponse(responseContent);

        if (string.IsNullOrWhiteSpace(rawJson))
            return BuildFallbackEvaluation();

        var parsed = ParseEvaluationResult(rawJson);

        return parsed ?? BuildFallbackEvaluation();
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

Return ONLY valid JSON.
Do not wrap the JSON in markdown fences.

Use EXACTLY this shape:

{
  "observation": "string",
  "strengths": "string",
  "communication": "string",
  "growthOpportunity": "string",
  "overallImpression": "string",
  "nextFocus": "string"
}

Each field must be 1–3 short paragraphs.
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
            sb.AppendLine($"Question: {question?.Text ?? "Unknown"}");
            sb.AppendLine($"Answer: {answer.Transcript}");
        }

        sb.AppendLine();
        sb.AppendLine("Return JSON only.");

        return sb.ToString();
    }

    private static string? ExtractTextFromGeminiResponse(string responseContent)
    {
        try
        {
            using var doc = JsonDocument.Parse(responseContent);

            if (!doc.RootElement.TryGetProperty("candidates", out var candidates))
                return null;

            var first = candidates[0];

            if (!first.TryGetProperty("content", out var content))
                return null;

            if (!content.TryGetProperty("parts", out var parts))
                return null;

            return parts[0].GetProperty("text").GetString();
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
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();
            }

            using var doc = JsonDocument.Parse(cleaned);
            var root = doc.RootElement;

            return new EvaluationResultDto
            {
                Observation = root.GetProperty("observation").GetString() ?? "",
                Strengths = root.GetProperty("strengths").GetString() ?? "",
                Communication = root.GetProperty("communication").GetString() ?? "",
                GrowthOpportunity = root.GetProperty("growthOpportunity").GetString() ?? "",
                OverallImpression = root.GetProperty("overallImpression").GetString() ?? "",
                NextFocus = root.GetProperty("nextFocus").GetString() ?? ""
            };
        }
        catch
        {
            return null;
        }
    }

    private static EvaluationResultDto BuildFallbackEvaluation()
    {
        return new EvaluationResultDto
        {
            Observation = "The AI response could not be parsed. Provide clearer, more structured answers.",
            Strengths = "Fallback evaluation: strengths could not be determined.",
            Communication = "Fallback evaluation: communication clarity could not be assessed.",
            GrowthOpportunity = "Fallback evaluation: consider improving depth and specificity.",
            OverallImpression = "Fallback evaluation: insufficient data for a full impression.",
            NextFocus = "Fallback evaluation: focus on structured examples and measurable outcomes."
        };
    }
}
