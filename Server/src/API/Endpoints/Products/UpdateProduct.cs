using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Products.UpdateProduct;
using SharedKernel.Authorization;

namespace API.Endpoints.Products;

internal sealed class UpdateProduct : IEndpoint
{
    public sealed record VariantRequest(Guid? Id, Guid ColorId, Guid SizeId, decimal Price, int QuantityInStock, string SKU);

    public sealed record Request(string Name, string Description, Guid CategoryId, Guid? SubcategoryId, Guid BrandId, List<Guid> GenderIds, List<VariantRequest> Variants);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateProductCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateProductCommand(
                id,
                request.Name,
                request.Description,
                request.CategoryId,
                request.SubcategoryId,
                request.BrandId,
                request.GenderIds,
                request.Variants
                    .Select(v => new UpdateVariantRequest(
                        v.Id,
                        v.ColorId,
                        v.SizeId,
                        v.Price,
                        v.QuantityInStock,
                        v.SKU))
                    .ToList());

            SharedKernel.Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.ProductsUpdate)
        .WithRequestValidation<Request>()
        .WithName(nameof(UpdateProduct))
        .WithSummary("Updates a product's details and syncs its variants.")
        .WithDescription("Variants with an id are updated in place; variants without one are added; omitted variants are removed.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
