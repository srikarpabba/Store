using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Colors.GetColor;

internal sealed class GetColorQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetColorQuery, ColorResponse>
{
    public async Task<Result<ColorResponse>> Handle(GetColorQuery query, CancellationToken cancellationToken)
    {
        ColorResponse? color = await context.Colors
            .AsNoTracking()
            .Where(c => c.Id == query.Id)
            .Select(c => new ColorResponse(c.Id, c.Name, c.HexCode))
            .FirstOrDefaultAsync(cancellationToken);

        if (color is null)
        {
            return Result.Failure<ColorResponse>(ColorErrors.NotFound(query.Id));
        }

        return color;
    }
}
