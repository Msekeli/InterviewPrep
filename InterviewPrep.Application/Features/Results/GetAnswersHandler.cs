using InterviewPrep.Application.DTOs;
using InterviewPrep.Application.Interfaces;

namespace InterviewPrep.Application.Features.Results;

public class GetAnswersHandler
{
    private readonly IInterviewSessionRepository _interviewSessionRepository;

    public GetAnswersHandler(IInterviewSessionRepository interviewSessionRepository)
    {
        _interviewSessionRepository = interviewSessionRepository;
    }

    public async Task<IReadOnlyList<AnswerDto>?> HandleAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        var session = await _interviewSessionRepository.GetByIdAsync(sessionId, cancellationToken);

        if (session is null)
        {
            return null;
        }

        var answers = await _interviewSessionRepository.GetAnswersBySessionIdAsync(sessionId, cancellationToken);

        return answers
            .Select(answer => new AnswerDto
            {
                Id = answer.Id,
                InterviewSessionId = answer.InterviewSessionId,
                InterviewQuestionId = answer.InterviewQuestionId,
                Transcript = answer.Transcript,
                Score = answer.Score
            })
            .ToList();
    }
}