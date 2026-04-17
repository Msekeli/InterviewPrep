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
                .Select(question => new QuestionDto
                {
                    Id = question.Id,
                    Category = question.Category,
                    Text = question.Text,
                    Order = question.Order
                })
                .ToList();
        }

       var questions = await _questionService.GenerateQuestionsAsync(session, cancellationToken);

        foreach (var question in questions)
        {
            question.InterviewSessionId = sessionId;
        }

        await _interviewSessionRepository.AddQuestionsAsync(questions, cancellationToken);

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