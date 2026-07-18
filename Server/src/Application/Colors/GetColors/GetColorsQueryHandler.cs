using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Colors.GetColors;

internal sealed class GetColorsQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetColorsQuery, IReadOnlyList<ColorResponse>>
{
    public async Task<Result<IReadOnlyList<ColorResponse>>> Handle(
        GetColorsQuery query,
        CancellationToken cancellationToken)
    {
        List<ColorResponse> colors = await context.Colors
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new ColorResponse(c.Id, c.Name, c.HexCode))
            .ToListAsync(cancellationToken);

        return colors;
    }
}
