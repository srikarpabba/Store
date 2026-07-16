namespace Application.Abstractions.Authentication;

public interface IUserContext
{
    Guid? DefaultorNullUserId { get; }
    Guid UserId { get; }
}
