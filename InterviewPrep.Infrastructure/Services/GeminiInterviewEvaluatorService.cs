using System.Text.Json;
using Google.GenAI;
using Google.GenAI.Types;
using InterviewPrep.Application.DTOs;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace InterviewPrep.Infrastructure.Services;

public class GeminiInterviewEvaluatorService : IInterviewEvaluatorService
{
    private readonly Client _client;
    private readonly string _model;

    public GeminiInterviewEvaluatorService(
        Client client,
        IConfiguration configuration)
    {
        _client = client;
        _model = configuration["Gemini:Model"] ?? "gemini-flash-latest";
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

            Return JSON only with exactly these fields:

            Observation
            Strengths
            Communication
            GrowthOpportunity
            OverallImpression
            NextFocus
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
                "Gemini returned an empty evaluation.");
        }

        var evaluation =
            JsonSerializer.Deserialize<EvaluationResultDto>(
                text,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (evaluation is null)
        {
            throw new InvalidOperationException(
                "Gemini did not return a valid interview evaluation.");
        }

        return evaluation;
    }
}