using InterviewPrep.Application.DTOs;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;

namespace InterviewPrep.Infrastructure.Services;

public class MockInterviewEvaluatorService : IInterviewEvaluatorService
{
    public Task<EvaluationResultDto> EvaluateAsync(
        InterviewSession session,
        IReadOnlyList<InterviewQuestion> questions,
        IReadOnlyList<InterviewAnswer> answers,
        CancellationToken cancellationToken)
    {
        if (answers.Count == 0)
        {
            return Task.FromResult(new EvaluationResultDto
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
            });
        }

        return Task.FromResult(new EvaluationResultDto
        {
            Observation =
                "You demonstrated strong full-stack awareness across frontend, backend, cloud, and software architecture.",

            Strengths =
                "You supported your answers with real production examples from customer-facing platforms and enterprise systems, showing both technical breadth and practical ownership.",

            Communication =
                "Your responses were clear, concise, and well structured, making your technical decisions easy to follow.",

            GrowthOpportunity =
                "Try highlighting measurable business outcomes more often, such as performance improvements, delivery impact, or user value created.",

            OverallImpression =
                "You presented as a thoughtful engineer with practical experience in modern web development, cloud technologies, and maintainable system design.",

            NextFocus =
                "In future interviews, lean even further into leadership stories, architectural trade-offs, and business impact."
        });
    }
}