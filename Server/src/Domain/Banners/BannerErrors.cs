using SharedKernel;

namespace Domain.Banners;

public static class BannerErrors
{
    public static Error NotFound(Guid bannerId) => Error.NotFound(
        "Banners.NotFound",
        $"The banner with the Id = '{bannerId}' was not found");
}
