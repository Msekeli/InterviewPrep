using InterviewPrep.Domain.Entities;
using InterviewPrep.Domain.Enums;
using FluentAssertions;

namespace InterviewPrep.Tests.Domain;

public class InterviewSessionTests
{
    [Fact]
    public void Should_Create_InterviewSession_With_Expected_Properties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // Act
        var session = new InterviewSession
        {
            Id = sessionId,
            UserId = userId,
            CvText = "C# .NET developer",
            JobSpecText = "Build backend APIs",
            CompanyText = "Tech company",
            TargetLevel = InterviewLevel.Intermediate,
            Status = InterviewSessionStatus.Draft,
            CreatedAtUtc = createdAt
        };

        // Assert
        session.Id.Should().Be(sessionId);
        session.UserId.Should().Be(userId);
        session.CvText.Should().Be("C# .NET developer");
        session.JobSpecText.Should().Be("Build backend APIs");
        session.CompanyText.Should().Be("Tech company");
        session.TargetLevel.Should().Be(InterviewLevel.Intermediate);
        session.Status.Should().Be(InterviewSessionStatus.Draft);
        session.CreatedAtUtc.Should().Be(createdAt);
        session.CompletedAtUtc.Should().BeNull();
        session.Observation.Should().BeEmpty();
        session.Strengths.Should().BeEmpty();
        session.Communication.Should().BeEmpty();
        session.GrowthOpportunity.Should().BeEmpty();
        session.OverallImpression.Should().BeEmpty();
        session.NextFocus.Should().BeEmpty();
    }

    [Fact]
    public void Should_Allow_Completed_State_To_Be_Set()
    {
        // Arrange
        var completedAt = DateTime.UtcNow;

        var session = new InterviewSession
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            CvText = "CV text",
            JobSpecText = "Job spec",
            CompanyText = "Company text",
            TargetLevel = InterviewLevel.Senior,
            Status = InterviewSessionStatus.InProgress,
            CreatedAtUtc = DateTime.UtcNow
        };

        // Act
        session.Status = InterviewSessionStatus.Completed;
        session.Observation = "Strong performance overall.";
        session.Strengths = "Clear, structured answers.";
        session.Communication = "Confident and concise.";
        session.GrowthOpportunity = "Go deeper on trade-offs made.";
        session.OverallImpression = "A composed, credible interview.";
        session.NextFocus = "Practice a failure-story example.";
        session.CompletedAtUtc = completedAt;

        // Assert
        session.Status.Should().Be(InterviewSessionStatus.Completed);
        session.Observation.Should().Be("Strong performance overall.");
        session.Strengths.Should().Be("Clear, structured answers.");
        session.Communication.Should().Be("Confident and concise.");
        session.GrowthOpportunity.Should().Be("Go deeper on trade-offs made.");
        session.OverallImpression.Should().Be("A composed, credible interview.");
        session.NextFocus.Should().Be("Practice a failure-story example.");
        session.CompletedAtUtc.Should().Be(completedAt);
    }

    [Fact]
    public void Should_Default_New_Draft_Session_To_Empty_Feedback_And_Null_CompletedAt()
    {
        // Act
        var session = new InterviewSession
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            CvText = "CV text",
            JobSpecText = "Job spec",
            CompanyText = "Company text",
            TargetLevel = InterviewLevel.Junior,
            Status = InterviewSessionStatus.Draft,
            CreatedAtUtc = DateTime.UtcNow
        };

        // Assert
        session.Status.Should().Be(InterviewSessionStatus.Draft);
        session.Observation.Should().BeEmpty();
        session.NextFocus.Should().BeEmpty();
        session.CompletedAtUtc.Should().BeNull();
    }
}
