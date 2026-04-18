using System.Text;
using System.Text.Json;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
using InterviewPrep.Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace InterviewPrep.Infrastructure.Services;

public class GeminiQuestionService : IQuestionService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GeminiQuestionService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<IReadOnlyList<InterviewQuestion>> GenerateQuestionsAsync(
        InterviewSession session,
        CancellationToken cancellationToken)
    {
        var apiKey = _configuration["Gemini:ApiKey"];
        var model = _configuration["Gemini:Model"] ?? "gemini-2.0-flash";

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Gemini API key is not configured.");
        }

        var prompt = BuildPrompt(session);

        var requestBody = new GeminiGenerateContentRequest
        {
            Contents =
            [
                new GeminiContent
                {
                    Parts =
                    [
                        new GeminiPart
                        {
                            Text = prompt
                        }
                    ]
                }
            ],
            GenerationConfig = new GeminiGenerationConfig
            {
                Temperature = 0.7,
                ResponseMimeType = "application/json"
            }
        };

        var requestJson = JsonSerializer.Serialize(
            requestBody,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

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
            throw new InvalidOperationException($"Gemini question generation failed: {responseContent}");
        }

        var geminiResponse = JsonSerializer.Deserialize<GeminiGenerateContentResponse>(
            responseContent,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        var rawJson = ExtractJsonText(geminiResponse);

        if (string.IsNullOrWhiteSpace(rawJson))
        {
            throw new InvalidOperationException("Gemini returned an empty response.");
        }

        var generatedQuestions = JsonSerializer.Deserialize<List<GeneratedQuestionItem>>(
            rawJson,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (generatedQuestions is null || generatedQuestions.Count == 0)
        {
            throw new InvalidOperationException("Gemini did not return any valid questions.");
        }

        var questions = generatedQuestions
            .Where(question => !string.IsNullOrWhiteSpace(question.Text))
            .OrderBy(question => question.Order)
            .Select((question, index) =>
                new InterviewQuestion(
                    session.Id,
                    ParseCategory(question.Category),
                    question.Text.Trim(),
                    index + 1))
            .ToList();

        return questions;
    }

  private static string BuildPrompt(InterviewSession session)
{
    return $$"""
You are generating interview questions for a mock interview application.

Generate a JSON array only.
Do not include markdown.
Do not include explanation.
Do not wrap the JSON in backticks.

Return each item in this exact shape:
[
  {
    "category": "Technical",
    "text": "Question text here",
    "order": 1
  }
]

Rules:
- Generate 6 questions total.
- Prioritize CV-driven and project-based questions.
- Mix technical, behavioural, and culture-fit style questions if relevant.
- Keep questions realistic and specific to the candidate.
- Make order values sequential starting from 1.
- category must be a short label.

Candidate CV:
{{session.CvText}}

Job Specification:
{{session.JobSpecText}}

Company Information:
{{session.CompanyText}}
""";
}

    private static string? ExtractJsonText(GeminiGenerateContentResponse? response)
    {
        var text = response?.Candidates?
            .FirstOrDefault()?
            .Content?
            .Parts?
            .FirstOrDefault()?
            .Text;

        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        text = text.Trim();

        if (text.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
        {
            text = text["```json".Length..].Trim();
        }
        else if (text.StartsWith("```", StringComparison.OrdinalIgnoreCase))
        {
            text = text["```".Length..].Trim();
        }

        if (text.EndsWith("```", StringComparison.OrdinalIgnoreCase))
        {
            text = text[..^3].Trim();
        }

        return text;
    }

    private static QuestionCategory ParseCategory(string? rawCategory)
    {
        var fallback = Enum.GetValues<QuestionCategory>().First();

        if (string.IsNullOrWhiteSpace(rawCategory))
        {
            return fallback;
        }

        var normalizedInput = Normalize(rawCategory);

        foreach (var value in Enum.GetValues<QuestionCategory>())
        {
            var enumName = Normalize(value.ToString());

            if (enumName == normalizedInput)
            {
                return value;
            }
        }

        foreach (var value in Enum.GetValues<QuestionCategory>())
        {
            var enumName = Normalize(value.ToString());

            if (enumName.Contains(normalizedInput) || normalizedInput.Contains(enumName))
            {
                return value;
            }
        }

        return fallback;
    }

    private static string Normalize(string value)
    {
        return value
            .Trim()
            .Replace("-", string.Empty)
            .Replace("_", string.Empty)
            .Replace(" ", string.Empty)
            .ToLowerInvariant();
    }

    private sealed class GeneratedQuestionItem
    {
        public string Category { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public int Order { get; set; }
    }

    private sealed class GeminiGenerateContentRequest
    {
        public List<GeminiContent> Contents { get; set; } = [];
        public GeminiGenerationConfig GenerationConfig { get; set; } = new();
    }

    private sealed class GeminiContent
    {
        public List<GeminiPart> Parts { get; set; } = [];
    }

    private sealed class GeminiPart
    {
        public string Text { get; set; } = string.Empty;
    }

    private sealed class GeminiGenerationConfig
    {
        public double Temperature { get; set; }
        public string ResponseMimeType { get; set; } = "application/json";
    }

    private sealed class GeminiGenerateContentResponse
    {
        public List<GeminiCandidate>? Candidates { get; set; }
    }

    private sealed class GeminiCandidate
    {
        public GeminiContentResponse? Content { get; set; }
    }

    private sealed class GeminiContentResponse
    {
        public List<GeminiTextPart>? Parts { get; set; }
    }

    private sealed class GeminiTextPart
    {
        public string? Text { get; set; }
    }
}