using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Authentication;

internal static class RefreshTokenHasher
{
    internal static string Hash(string token)
    {
        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}
