namespace Application.Home.GetHome;

public sealed record HomeResponse(
    IReadOnlyList<HomeSectionResponse> Sections);
