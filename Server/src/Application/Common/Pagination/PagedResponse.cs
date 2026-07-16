namespace Application.Common.Pagination;

public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int PageIndex,
    int PageSize,
    int TotalCount)
{
    public int TotalPages =>
        (int)Math.Ceiling((double)TotalCount / PageSize);

    public bool HasPrevious => PageIndex > 1;

    public bool HasNext => PageIndex < TotalPages;
}
