using FluentAssertions;
using InterviewPrep.Application.DTOs;
using InterviewPrep.Application.Features.Sessions;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Domain.Entities;
using InterviewPrep.Domain.Enums;
using Moq;

namespace InterviewPrep.Tests.Application;

public class CreateSessionHandlerTests
{
    [Fact]
    public async Task HandleAsync_Should_Create_Session_And_Return_Dto()
    {
        // Arrange
        var repositoryMock = new Mock<IInterviewSessionRepository>();

        repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<InterviewSession>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateSessionHandler(repositoryMock.Object);

        var request = new CreateSessionRequest
        {
            CvText = "C# .NET Developer",
            JobSpecText = "Backend role",
            CompanyText = "Tech company",
            TargetLevel = InterviewLevel.Intermediate
        };

        // Act
        var result = await handler.HandleAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.CvText.Should().Be("C# .NET Developer");
        result.JobSpecText.Should().Be("Backend role");
        result.CompanyText.Should().Be("Tech company");
        result.TargetLevel.Should().Be(InterviewLevel.Intermediate);
        result.Status.Should().Be(InterviewSessionStatus.Draft);
        result.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.Feedback.Should().BeEmpty();

        repositoryMock.Verify(
            x => x.AddAsync(It.Is<InterviewSession>(s =>
                s.CvText == request.CvText &&
                s.JobSpecText == request.JobSpecText &&
                s.CompanyText == request.CompanyText &&
                s.TargetLevel == request.TargetLevel &&
                s.Status == InterviewSessionStatus.Draft),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}