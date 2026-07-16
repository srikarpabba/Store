namespace Application.Home.GetHome;


public sealed record HomeSectionResponse(
    string Key,
    string Title,
    HomeSectionType Type,
    int DisplayOrder,
    object Items);
