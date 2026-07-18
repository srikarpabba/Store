namespace Application.Common.Validation;

/// <summary>
/// Confirms an uploaded file's actual bytes are a real image of an allowed
/// type, instead of trusting the client-supplied Content-Type/extension. It
/// deliberately accepts a file whose bytes match *any* allowed format even if
/// its extension/content-type claims a different one (e.g. a WebP saved as
/// ".jpg" — common with modern CDN downloads) — the goal is to reject
/// non-images (a renamed executable), not to police mislabeled-but-valid ones.
/// </summary>
public static class ImageSignatureValidator
{
    private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    public static async Task<bool> IsRecognizedImageAsync(
        Stream content,
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

        // JPEG
        if (totalRead >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
        {
            return true;
        }

        // PNG
        if (totalRead >= 8 && header.AsSpan(0, 8).SequenceEqual(PngSignature))
        {
            return true;
        }

        // WebP: "RIFF" .... "WEBP"
        if (totalRead >= 12 && header.AsSpan(0, 4).SequenceEqual("RIFF"u8) && header.AsSpan(8, 4).SequenceEqual("WEBP"u8))
        {
            return true;
        }

        // AVIF: ISOBMFF 'ftyp' box
        if (totalRead >= 12 && header.AsSpan(4, 4).SequenceEqual("ftyp"u8))
        {
            return true;
        }

        return false;
    }
}
