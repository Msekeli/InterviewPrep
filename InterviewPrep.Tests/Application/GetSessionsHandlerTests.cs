using FluentAssertions;
using InterviewPrep.Application.Features.Sessions;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
using InterviewPrep.Domain.Enums;
using Moq;

namespace InterviewPrep.Tests.Application;

public class GetSessionsHandlerTests
{
    private static readonly Guid TemporaryUserId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public async Task HandleAsync_Should_Return_Mapped_Sessions_When_Sessions_Exist()
    {
        // Arrange
        var repositoryMock = new Mock<IInterviewSessionRepository>();

        var sessions = new List<InterviewSession>
        {
            new InterviewSession
            {
                Id = Guid.NewGuid(),
                UserId = TemporaryUserId,
                CvText = "C# .NET developer",
                JobSpecText = "Backend API role",
                CompanyText = "Tech company",
                TargetLevel = InterviewLevel.Intermediate,
                Status = InterviewSessionStatus.Draft,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-2)
            },
            new InterviewSession
            {
                Id = Guid.NewGuid(),
                UserId = TemporaryUserId,
                CvText = "React developer",
                JobSpecText = "Frontend role",
                CompanyText = "Startup",
                TargetLevel = InterviewLevel.Senior,
                Status = InterviewSessionStatus.Completed,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-1),
                CompletedAtUtc = DateTime.UtcNow,
                Observation = "Strong performance overall.",
                Strengths = "Clear communication.",
                Communication = "Concise and confident.",
                GrowthOpportunity = "Add more measurable outcomes.",
                OverallImpression = "A strong candidate.",
                NextFocus = "Practice a leadership example."
            }
        };

        repositoryMock
            .Setup(x => x.GetByUserIdAsync(TemporaryUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessions);

        var handler = new GetSessionsHandler(repositoryMock.Object);

        // Act
        var result = await handler.HandleAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        result[0].Id.Should().Be(sessions[0].Id);
        result[0].CvText.Should().Be(sessions[0].CvText);
        result[0].JobSpecText.Should().Be(sessions[0].JobSpecText);
        result[0].CompanyText.Should().Be(sessions[0].CompanyText);
        result[0].TargetLevel.Should().Be(sessions[0].TargetLevel);
        result[0].Status.Should().Be(sessions[0].Status);
        result[0].CreatedAtUtc.Should().Be(sessions[0].CreatedAtUtc);
        result[0].CompletedAtUtc.Should().Be(sessions[0].CompletedAtUtc);
        result[0].Observation.Should().Be(sessions[0].Observation);
        result[0].NextFocus.Should().Be(sessions[0].NextFocus);

        result[1].Id.Should().Be(sessions[1].Id);
        result[1].Status.Should().Be(InterviewSessionStatus.Completed);
        result[1].Observation.Should().Be("Strong performance overall.");
        result[1].NextFocus.Should().Be("Practice a leadership example.");

        repositoryMock.Verify(
            x => x.GetByUserIdAsync(TemporaryUserId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Empty_List_When_No_Sessions_Exist()
    {
        // Arrange
        var repositoryMock = new Mock<IInterviewSessionRepository>();

        repositoryMock
            .Setup(x => x.GetByUserIdAsync(TemporaryUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InterviewSession>());

        var handler = new GetSessionsHandler(repositoryMock.Object);

        // Act
        var result = await handler.HandleAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        repositoryMock.Verify(
            x => x.GetByUserIdAsync(TemporaryUserId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}