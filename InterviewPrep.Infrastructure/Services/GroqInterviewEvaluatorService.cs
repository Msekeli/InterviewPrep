using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using InterviewPrep.Application.DTOs;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace InterviewPrep.Infrastructure.Services;

public class GroqInterviewEvaluatorService : IInterviewEvaluatorService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly string _baseUrl;

    public GroqInterviewEvaluatorService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;

        _apiKey = configuration["Groq:ApiKey"]
            ?? throw new InvalidOperationException(
                "Groq API key is not configured.");

        _model = configuration["Groq:Model"]
            ?? "openai/gpt-oss-20b";

        _baseUrl = configuration["Groq:BaseUrl"]
            ?? "https://api.groq.com/openai/v1";
    }

    public async Task<EvaluationResultDto> EvaluateAsync(
        InterviewSession session,
        IReadOnlyList<InterviewQuestion> questions,
        IReadOnlyList<InterviewAnswer> answers,
        CancellationToken cancellationToken)
    {
        if (answers.Count == 0)
        {
            return new EvaluationResultDto
            {
                Observation =
                    "No interview responses were available for coaching.",

                Strengths =
                    "No strengths could be identified yet.",

                Communication =
                    "No communication patterns were observed.",

                GrowthOpportunity =
                    "Complete an interview session to receive coaching insights.",

                OverallImpression =
                    "Interview session incomplete.",

                NextFocus =
                    "Return when you're ready for another round."
            };
        }

        var interview = string.Join(
            "\n\n",
            questions.Select(question =>
            {
                var answer = answers.FirstOrDefault(
                    a => a.InterviewQuestionId == question.Id);

                return $"""
                    Question:
                    {question.Text}

                    Candidate Answer:
                    {answer?.Transcript ?? "[No answer provided]"}
                    """;
            }));

        var prompt = $"""
            Evaluate this candidate's interview performance as an expert
            interview coach.

            Candidate CV:
            {session.CvText}

            Job Description:
            {session.JobSpecText}

            Company:
            {session.CompanyText}

            Target Level:
            {session.TargetLevel}

            Interview Questions and Answers:
            {interview}

            Provide constructive, evidence-based feedback.

            Return ONLY valid JSON.

            The JSON must contain these six properties:

            Observation
            Strengths
            Communication
            GrowthOpportunity
            OverallImpression
            NextFocus

            Every property must contain a plain text string.

            Do not return arrays.
            Do not return objects inside the properties.
            Do not return numbers.
            Do not return Markdown.
            Do not include any text before or after the JSON.
            """;

        var request = new
        {
            model = _model,

            messages = new[]
            {
                new
                {
                    role = "system",
                    content =
                        "You are an expert interview coach. " +
                        "Evaluate answers objectively and provide " +
                        "specific, useful coaching feedback. " +
                        "Always return valid JSON matching the requested structure."
                },

                new
                {
                    role = "user",
                    content = prompt
                }
            },

            temperature = 0.2,

            response_format = new
            {
                type = "json_object"
            }
        };

        using var requestMessage = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_baseUrl.TrimEnd('/')}/chat/completions");

        requestMessage.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                _apiKey);

        requestMessage.Content =
            JsonContent.Create(request);

        using var response = await _httpClient.SendAsync(
            requestMessage,
            cancellationToken);

        var responseBody =
            await response.Content.ReadAsStringAsync(
                cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Groq evaluation request failed with status " +
                $"{(int)response.StatusCode}: {responseBody}");
        }

        var completion =
            JsonSerializer.Deserialize<GroqChatCompletion>(
                responseBody,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        var content = completion?
            .Choices?
            .FirstOrDefault()?
            .Message?
            .Content;

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidOperationException(
                "Groq returned an empty evaluation.");
        }

        var evaluation =
            JsonSerializer.Deserialize<EvaluationResultDto>(
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (evaluation is null)
        {
            throw new InvalidOperationException(
                "Groq did not return a valid interview evaluation.");
        }

        return evaluation;
    }

    private sealed class GroqChatCompletion
    {
        public List<Choice>? Choices { get; set; }
    }

    private sealed class Choice
    {
        public Message? Message { get; set; }
    }

    private sealed class Message
    {
        public string? Content { get; set; }
    }
}