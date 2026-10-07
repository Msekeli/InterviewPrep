using System.Text.Json;
using Google.GenAI;
using Google.GenAI.Types;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
using InterviewPrep.Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace InterviewPrep.Infrastructure.Services;

public class GeminiQuestionService : IQuestionService
{
    private readonly Client _client;
    private readonly string _model;

    public GeminiQuestionService(
        Client client,
        IConfiguration configuration)
    {
        _client = client;
        _model = configuration["Gemini:Model"] ?? "gemini-flash-latest";
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

            Generate 5 interview questions.

            Questions should be a useful mixture of:
            - Cv
            - Technical
            - Behavioural
            - CultureFit

            Return JSON only.
            """;

        var response = await _client.Models.GenerateContentAsync(
            model: _model,
            contents: prompt,
            config: new GenerateContentConfig
            {
                ResponseMimeType = "application/json"
            });

        var text = response.Candidates?
            .FirstOrDefault()?
            .Content?
            .Parts?
            .FirstOrDefault()?
            .Text;

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new InvalidOperationException(
                "Gemini returned an empty response.");
        }

        var generated = JsonSerializer.Deserialize<List<GeminiQuestion>>(
            text,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (generated is null || generated.Count == 0)
        {
            throw new InvalidOperationException(
                "Gemini did not return valid interview questions.");
        }

        return generated
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

    private sealed class GeminiQuestion
    {
        public string Category { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}