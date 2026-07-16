using Application.Abstractions.Messaging;

namespace Application.Home.GetHome;

public sealed record GetHomeQuery(
    string Storefront)
    : IQuery<HomeResponse>;
