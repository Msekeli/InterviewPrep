using System.Net.Http.Json;
using System.Text.Json;
using InterviewPrep.Application.DTOs;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace InterviewPrep.Infrastructure.Services;

public class OllamaInterviewEvaluatorService : IInterviewEvaluatorService
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly string _baseUrl;

    public OllamaInterviewEvaluatorService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _model = configuration["Ollama:Model"] ?? "llama3.2";
        _baseUrl = configuration["Ollama:BaseUrl"] ?? "http://localhost:11434";
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
                Observation = "No interview responses were available for coaching.",
                Strengths = "No strengths could be identified yet.",
                Communication = "No communication patterns were observed.",
                GrowthOpportunity = "Complete an interview session to receive coaching insights.",
                OverallImpression = "Interview session incomplete.",
                NextFocus = "Return when you're ready for another round."
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
            You are an expert interview coach.

            Evaluate this candidate's interview performance.

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

            Return JSON only.

            Each field MUST be a JSON string.
            Do not return arrays.
            Do not return objects inside any field.
            Do not return numbers or lists.
            Do not return Markdown.

            The JSON must contain exactly these six fields:
            Observation
            Strengths
            Communication
            GrowthOpportunity
            OverallImpression
            NextFocus

            Every field must contain a plain text string.
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
                "Ollama returned an empty evaluation.");
        }

        var evaluation =
            JsonSerializer.Deserialize<EvaluationResultDto>(
                result.Response,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (evaluation is null)
        {
            throw new InvalidOperationException(
                "Ollama did not return a valid interview evaluation.");
        }

        return evaluation;
    }

    private sealed class OllamaResponse
    {
        public string Response { get; set; } = string.Empty;
    }
}