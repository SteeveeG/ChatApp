using SkiaSharp;

public static class MimeTypeMapping
{
    public static string GetMimeTypeFromExtension(string extension)
    {
        if (string.IsNullOrEmpty(extension))
        {
            throw new ArgumentNullException(nameof(extension));
        }

        return _mimeTypeMapping.TryGetValue(extension, out string mimeType) ? mimeType : DEFAULT_MIME_TYPE;
    }

    public static SKEncodedImageFormat GetSkiaSharpImageFormatFromExtension(string extension)
    {
        if (string.IsNullOrEmpty(extension))
        {
            throw new ArgumentNullException(nameof(extension));
        }

        return _skiaSharpImageFormatMapping.TryGetValue(extension, out SKEncodedImageFormat imageFormat) ? imageFormat : DEFAULT_IMAGE_FORMAT;
    }

    private const string DEFAULT_MIME_TYPE = "application/octet-stream";
    private const SKEncodedImageFormat DEFAULT_IMAGE_FORMAT = SKEncodedImageFormat.Png;

    private static readonly Dictionary<string, string> _mimeTypeMapping = new(StringComparer.InvariantCultureIgnoreCase)
    {
        {".jpe", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".png", "image/png"}
    };

    private static readonly Dictionary<string, SKEncodedImageFormat> _skiaSharpImageFormatMapping = new(StringComparer.InvariantCultureIgnoreCase)
    {
        {".png", SKEncodedImageFormat.Png },
        {".jpg", SKEncodedImageFormat.Jpeg },
        {".jpeg", SKEncodedImageFormat.Jpeg },
        {".jpe", SKEncodedImageFormat.Jpeg }
    };
}