using System.Text;
using System.Text.Json;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
using InterviewPrep.Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace InterviewPrep.Infrastructure.Services;

public class GeminiQuestionService : IQuestionService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public GeminiQuestionService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<InterviewQuestion>> GenerateQuestionsAsync(
        InterviewSession session,
        CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["Gemini:ApiKey"];
        var model = _configuration["Gemini:Model"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Gemini API key is missing.");
        }

        if (string.IsNullOrWhiteSpace(model))
        {
            throw new InvalidOperationException("Gemini model is missing.");
        }

        var prompt =
            "Generate exactly 3 interview questions based on the following:\n\n" +
            $"CV:\n{session.CvText}\n\n" +
            $"Job Spec:\n{session.JobSpecText}\n\n" +
            $"Company Info:\n{session.CompanyText}\n\n" +
            $"Level:\n{session.TargetLevel}\n\n" +
            "Return only JSON in this format:\n" +
            "[\n" +
            "  {\n" +
            "    \"category\": \"Cv\",\n" +
            "    \"text\": \"Question text here\"\n" +
            "  },\n" +
            "  {\n" +
            "    \"category\": \"Technical\",\n" +
            "    \"text\": \"Question text here\"\n" +
            "  },\n" +
            "  {\n" +
            "    \"category\": \"Behavioural\",\n" +
            "    \"text\": \"Question text here\"\n" +
            "  }\n" +
            "]";

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
            }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json");

        var url =
            $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

        var response = await _httpClient.PostAsync(url, content, cancellationToken);

        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
        Console.WriteLine(responseString);

        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(responseString);

        var outputText = document.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        if (string.IsNullOrWhiteSpace(outputText))
        {
            throw new InvalidOperationException("Gemini returned empty question content.");
        }

        outputText = outputText
            .Replace("```json", "", StringComparison.OrdinalIgnoreCase)
            .Replace("```", "", StringComparison.OrdinalIgnoreCase)
            .Trim();

        var generatedQuestions = JsonSerializer.Deserialize<List<GeneratedQuestion>>(
            outputText,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (generatedQuestions is null || generatedQuestions.Count == 0)
        {
            throw new InvalidOperationException("Failed to parse generated questions.");
        }

        var questions = generatedQuestions
            .Select((q, index) => new InterviewQuestion
            {
                Id = Guid.NewGuid(),
                InterviewSessionId = session.Id,
                Category = MapCategory(q.Category),
                Text = q.Text,
                Order = index + 1
            })
            .ToList();

        return questions;
    }

    private static QuestionCategory MapCategory(string? category)
    {
        return category?.Trim().ToLowerInvariant() switch
        {
            "cv" => QuestionCategory.Cv,
            "technical" => QuestionCategory.Technical,
            "behavioural" => QuestionCategory.Behavioural,
            "behavioral" => QuestionCategory.Behavioural,
            "culturefit" => QuestionCategory.CultureFit,
            "culture fit" => QuestionCategory.CultureFit,
            _ => QuestionCategory.Technical
        };
    }

    private sealed class GeneratedQuestion
    {
        public string Category { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}