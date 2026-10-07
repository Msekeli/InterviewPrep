using System.Net.Http.Json;
using System.Text.Json;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
using InterviewPrep.Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace InterviewPrep.Infrastructure.Services;

public class OllamaQuestionService : IQuestionService
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly string _baseUrl;

    public OllamaQuestionService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _model = configuration["Ollama:Model"] ?? "llama3.2";
        _baseUrl = configuration["Ollama:BaseUrl"] ?? "http://localhost:11434";
    }

    public async Task<IReadOnlyList<InterviewQuestion>> GenerateQuestionsAsync(
        InterviewSession session,
        CancellationToken cancellationToken = default)
    {
        var prompt = $"""
            Generate interview questions for this candidate.

            Candidate CV:
            {session.CvText}

            Job Description:
            {session.JobSpecText}

            Company Information:
            {session.CompanyText}

            Target Level:
            {session.TargetLevel}

            Generate exactly 5 interview questions.

            Questions should be a useful mixture of:
            - Cv
            - Technical
            - Behavioural
            - CultureFit

            Return a JSON array only.
            Each item must contain:
            - Category
            - Text
            """;

        var request = new
        {
            model = _model,
            prompt,
            stream = false,
            format = "json"
        };

        using var response = await _httpClient.PostAsJsonAsync(
            $"{_baseUrl.TrimEnd('/')}/api/generate",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaResponse>(
            cancellationToken: cancellationToken);

        if (result is null || string.IsNullOrWhiteSpace(result.Response))
        {
            throw new InvalidOperationException(
                "Ollama returned an empty response.");
        }

        var generated = JsonSerializer.Deserialize<List<OllamaQuestion>>(
            result.Response,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (generated is null || generated.Count == 0)
        {
            throw new InvalidOperationException(
                "Ollama did not return valid interview questions.");
        }

        return generated
            .Take(5)
            .Select((question, index) =>
                new InterviewQuestion(
                    session.Id,
                    ParseCategory(question.Category),
                    question.Text,
                    index + 1))
            .ToList();
    }

    private static QuestionCategory ParseCategory(string category)
    {
        return Enum.TryParse<QuestionCategory>(
            category,
            ignoreCase: true,
            out var result)
            ? result
            : QuestionCategory.Technical;
    }

    private sealed class OllamaResponse
    {
        public string Response { get; set; } = string.Empty;
    }

    private sealed class OllamaQuestion
    {
        public string Category { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;
    }
}