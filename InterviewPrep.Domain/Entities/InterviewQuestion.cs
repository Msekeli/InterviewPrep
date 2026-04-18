using InterviewPrep.Domain.Enums;

namespace InterviewPrep.Domain.Entities;

public class InterviewQuestion
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid InterviewSessionId { get; private set; }

    public QuestionCategory Category { get; private set; }

    public string Text { get; private set; } = string.Empty;

    public int Order { get; private set; }

    // EF Core constructor
    private InterviewQuestion() { }

    public InterviewQuestion(
        Guid interviewSessionId,
        QuestionCategory category,
        string text,
        int order)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Question text cannot be empty.");

        InterviewSessionId = interviewSessionId;
        Category = category;
        Text = text.Trim();
        Order = order;
    }

    public void SetOrder(int order)
    {
        if (order <= 0)
            throw new ArgumentException("Order must be greater than zero.");

        Order = order;
    }
}