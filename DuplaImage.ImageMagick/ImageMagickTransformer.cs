using System.IO;
using ImageMagick;
using ImageMagick.Configuration;

namespace DuplaImage.Lib.ImageMagick {
    /// <summary>
    /// Implements IImageTransformer interface using Magick.NET for image transforms.
    /// </summary>
    public class ImageMagickTransformer : IImageTransformer {
        static ImageMagickTransformer() {
            // Secure Magick.NET by disabling unsafe coders to prevent SSRF and XXE
            string policyXml = @"<policymap>
  <policy domain=""coder"" rights=""none"" pattern=""HTTP"" />
  <policy domain=""coder"" rights=""none"" pattern=""HTTPS"" />
  <policy domain=""coder"" rights=""none"" pattern=""MVG"" />
  <policy domain=""coder"" rights=""none"" pattern=""MSL"" />
  <policy domain=""coder"" rights=""none"" pattern=""EPHEMERAL"" />
  <policy domain=""coder"" rights=""none"" pattern=""URL"" />
</policymap>";
            var configFiles = ConfigurationFiles.Default;
            configFiles.Policy.Data = policyXml;
            MagickNET.Initialize(configFiles);
        }

        private readonly QuantizeSettings _settings = new() {
            ColorSpace = ColorSpace.Gray,
            Colors = 256
        };

        public byte[] TransformImage(Stream stream, uint width, uint height) {
            // Read image
            MagickImage magickImage = new(stream);
            MagickGeometry size = new(width, height) { IgnoreAspectRatio = true };
            magickImage.Resize(size);
            _ = magickImage.Quantize(_settings);
            magickImage.Format = MagickFormat.Gray;
            return magickImage.ToByteArray();
        }
    }
}