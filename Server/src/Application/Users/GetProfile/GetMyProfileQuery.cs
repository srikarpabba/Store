using Application.Abstractions.Messaging;

namespace Application.Users.GetProfile;

public sealed record GetMyProfileQuery : IQuery<ProfileResponse>;
