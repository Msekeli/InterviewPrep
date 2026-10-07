using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
using InterviewPrep.Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace InterviewPrep.Infrastructure.Services;

public class GroqQuestionService : IQuestionService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly string _baseUrl;

    public GroqQuestionService(
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

    public async Task<IReadOnlyList<InterviewQuestion>> GenerateQuestionsAsync(
        InterviewSession session,
        CancellationToken cancellationToken = default)
    {
        var prompt = $"""
            Generate exactly 5 interview questions for this candidate.

            Candidate CV:
            {session.CvText}

            Job Description:
            {session.JobSpecText}

            Company Information:
            {session.CompanyText}

            Target Level:
            {session.TargetLevel}

            Questions must be a useful mixture of:
            - Cv
            - Technical
            - Behavioural
            - CultureFit

            Return only the JSON structure defined by the response schema.
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
                        "You are an expert technical interviewer. " +
                        "Generate relevant, realistic interview questions."
                },
                new
                {
                    role = "user",
                    content = prompt
                }
            },
            temperature = 0.3,
            response_format = new
            {
                type = "json_schema",
                json_schema = new
                {
                    name = "interview_questions",
                    strict = true,
                    schema = new
                    {
                        type = "object",
                        properties = new
                        {
                            questions = new
                            {
                                type = "array",
                                minItems = 5,
                                maxItems = 5,
                                items = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        category = new
                                        {
                                            type = "string",
                                            @enum = new[]
                                            {
                                                "Cv",
                                                "Technical",
                                                "Behavioural",
                                                "CultureFit"
                                            }
                                        },
                                        text = new
                                        {
                                            type = "string"
                                        }
                                    },
                                    required = new[]
                                    {
                                        "category",
                                        "text"
                                    },
                                    additionalProperties = false
                                }
                            }
                        },
                        required = new[]
                        {
                            "questions"
                        },
                        additionalProperties = false
                    }
                }
            }
        };

        using var requestMessage = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_baseUrl.TrimEnd('/')}/chat/completions");

        requestMessage.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _apiKey);

        requestMessage.Content =
            JsonContent.Create(request);

        using var response = await _httpClient.SendAsync(
            requestMessage,
            cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Groq request failed with status " +
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
                "Groq returned an empty response.");
        }

        var generated =
            JsonSerializer.Deserialize<GroqQuestionResponse>(
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (generated?.Questions is null ||
            generated.Questions.Count == 0)
        {
            throw new InvalidOperationException(
                "Groq did not return valid interview questions.");
        }

        return generated.Questions
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

    private sealed class GroqQuestionResponse
    {
        public List<GroqQuestion>? Questions { get; set; }
    }

    private sealed class GroqQuestion
    {
        public string Category { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;
    }
}