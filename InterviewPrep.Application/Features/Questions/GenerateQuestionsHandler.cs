using InterviewPrep.Application.DTOs;
using InterviewPrep.Application.Interfaces;

namespace InterviewPrep.Application.Features.Questions;

public class GenerateQuestionsHandler
{
    private readonly IInterviewSessionRepository _interviewSessionRepository;
    private readonly IQuestionService _questionService;

    public GenerateQuestionsHandler(
        IInterviewSessionRepository interviewSessionRepository,
        IQuestionService questionService)
    {
        _interviewSessionRepository = interviewSessionRepository;
        _questionService = questionService;
    }

    public async Task<IReadOnlyList<QuestionDto>?> HandleAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        var session = await _interviewSessionRepository.GetByIdAsync(sessionId, cancellationToken);

        if (session is null)
        {
            return null;
        }

        var existingQuestions = await _interviewSessionRepository.GetQuestionsBySessionIdAsync(sessionId, cancellationToken);

        if (existingQuestions.Count > 0)
        {
            return existingQuestions
                .OrderBy(question => question.Order)
                .Select(question => new QuestionDto
                {
                    Id = question.Id,
                    Category = question.Category,
                    Text = question.Text,
                    Order = question.Order
                })
                .ToList();
        }

        var generatedQuestions = await _questionService.GenerateQuestionsAsync(session, cancellationToken);

        var validQuestions = generatedQuestions
            .Where(question => !string.IsNullOrWhiteSpace(question.Text))
            .OrderBy(question => question.Order)
            .ToList();

        if (validQuestions.Count == 0)
        {
            throw new InvalidOperationException("No valid questions were generated.");
        }

        for (var i = 0; i < validQuestions.Count; i++)
        {
            validQuestions[i].SetOrder(i + 1);
        }

        await _interviewSessionRepository.AddQuestionsAsync(validQuestions, cancellationToken);

        return validQuestions
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