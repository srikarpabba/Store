using Application.Abstractions.Messaging;

namespace Application.Storefront.GetStorefrontSections;

public sealed record GetStorefrontSectionsQuery(
    string Storefront)
    : IQuery<StorefrontSectionsResponse>;
