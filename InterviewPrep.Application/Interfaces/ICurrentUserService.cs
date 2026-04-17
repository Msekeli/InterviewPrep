namespace InterviewPrep.Application.Interfaces;

public interface ICurrentUserService
{
    Guid GetUserId();
    string GetUserEmail();
}