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
            CreatedAtUtc = createdAt,
            Feedback = string.Empty
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
        session.OverallScore.Should().BeNull();
        session.Feedback.Should().BeEmpty();
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
            CreatedAtUtc = DateTime.UtcNow,
            Feedback = string.Empty
        };

        // Act
        session.Status = InterviewSessionStatus.Completed;
        session.OverallScore = 82.5m;
        session.Feedback = "Strong performance overall.";
        session.CompletedAtUtc = completedAt;

        // Assert
        session.Status.Should().Be(InterviewSessionStatus.Completed);
        session.OverallScore.Should().Be(82.5m);
        session.Feedback.Should().Be("Strong performance overall.");
        session.CompletedAtUtc.Should().Be(completedAt);
    }

    [Fact]
    public void Should_Allow_Null_Score_And_CompletedAt_For_New_Draft_Session()
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
            CreatedAtUtc = DateTime.UtcNow,
            Feedback = string.Empty
        };

        // Assert
        session.Status.Should().Be(InterviewSessionStatus.Draft);
        session.OverallScore.Should().BeNull();
        session.CompletedAtUtc.Should().BeNull();
    }
}