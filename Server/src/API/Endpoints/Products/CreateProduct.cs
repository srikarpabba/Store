using API.Extensions;
using API.Responses;
using Application.Abstractions.Messaging;
using Application.Products.CreateProduct;
using SharedKernel.Authorization;

namespace API.Endpoints.Products;

internal sealed class CreateProduct : IEndpoint
{
    public sealed record VariantRequest(Guid ColorId, Guid SizeId, decimal Price, int QuantityInStock, string SKU);

    public sealed record Request(string Name, string Description, Guid CategoryId, Guid BrandId, List<Guid> GenderIds, List<VariantRequest> Variants);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/", async (
            Request request,
            ICommandHandler<CreateProductCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateProductCommand(
                request.Name,
                request.Description,
                request.CategoryId,
                request.BrandId,
                request.GenderIds,
                request.Variants
                    .Select(v => new CreateVariantRequest(
                        v.ColorId,
                        v.SizeId,
                        v.Price,
                        v.QuantityInStock,
                        v.SKU))
                    .ToList());

            SharedKernel.Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.CreatedAtRoute(nameof(GetProduct), new { id }, id),
                CustomResults.Problem);
        })
        .HasPermission(Permissions.ProductsCreate)
        .WithRequestValidation<Request>()
        .WithName(nameof(CreateProduct))
        .WithSummary("Creates a product with its initial variants.")
        .WithDescription("At least one variant is required. Images are uploaded separately after creation via /products/{id}/images.")
        .Produces<Guid>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
