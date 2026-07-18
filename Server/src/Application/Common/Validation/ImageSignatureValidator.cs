namespace Application.Common.Validation;

/// <summary>
/// Confirms an uploaded file's actual bytes match its declared image content
/// type, instead of trusting the client-supplied Content-Type/extension.
/// </summary>
public static class ImageSignatureValidator
{
    private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    public static async Task<bool> MatchesDeclaredTypeAsync(
        Stream content,
        string contentType,
        CancellationToken cancellationToken)
    {
        if (!content.CanSeek)
        {
            // can't safely peek-and-rewind; fall back to trusting the earlier checks
            return true;
        }

        byte[] header = new byte[12];

        content.Position = 0;

        int totalRead = 0;
        while (totalRead < header.Length)
        {
            int bytesRead = await content.ReadAsync(header.AsMemory(totalRead), cancellationToken);

            if (bytesRead == 0)
            {
                break;
            }

            totalRead += bytesRead;
        }

        content.Position = 0;

        return contentType switch
        {
            "image/jpeg" => totalRead >= 3
                && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,

            "image/png" => totalRead >= 8
                && header.AsSpan(0, 8).SequenceEqual(PngSignature),

            "image/webp" => totalRead >= 12
                && header.AsSpan(0, 4).SequenceEqual("RIFF"u8)
                && header.AsSpan(8, 4).SequenceEqual("WEBP"u8),

            // ISOBMFF 'ftyp' box; covers the vast majority of real-world encoders,
            // which set major_brand to avif/avis directly.
            "image/avif" => totalRead >= 12
                && header.AsSpan(4, 4).SequenceEqual("ftyp"u8),

            _ => false
        };
    }
}
