using InterviewPrep.Application.DTOs;
using InterviewPrep.Application.Interfaces;

namespace InterviewPrep.Application.Features.Questions;

public class GetQuestionsHandler
{
    private readonly IInterviewSessionRepository _interviewSessionRepository;

    public GetQuestionsHandler(IInterviewSessionRepository interviewSessionRepository)
    {
        _interviewSessionRepository = interviewSessionRepository;
    }

    public async Task<IReadOnlyList<QuestionDto>?> HandleAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        var session = await _interviewSessionRepository.GetByIdAsync(sessionId, cancellationToken);

        if (session is null)
        {
            return null;
        }

        var questions = await _interviewSessionRepository.GetQuestionsBySessionIdAsync(sessionId, cancellationToken);

        return questions
            .Select(question => new QuestionDto
            {
                Id = question.Id,
                Category = question.Category,
                Text = question.Text,
                Order = question.Order
            })
            .ToList();
    }
}