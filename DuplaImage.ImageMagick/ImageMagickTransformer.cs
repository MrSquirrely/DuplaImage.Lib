using System.IO;
using ImageMagick;
using ImageMagick.Configuration;

namespace DuplaImage.Lib.ImageMagick {
    /// <summary>
    /// Implements IImageTransformer interface using Magick.NET for image transforms.
    /// </summary>
    public class ImageMagickTransformer : IImageTransformer {

        static ImageMagickTransformer() {
            var configFiles = ConfigurationFiles.Default;
            configFiles.Policy.Data = @"<policymap>
  <policy domain=""coder"" rights=""none"" pattern=""HTTP"" />
  <policy domain=""coder"" rights=""none"" pattern=""HTTPS"" />
  <policy domain=""coder"" rights=""none"" pattern=""MVG"" />
  <policy domain=""coder"" rights=""none"" pattern=""MSL"" />
  <policy domain=""coder"" rights=""none"" pattern=""EPHEMERAL"" />
  <policy domain=""coder"" rights=""none"" pattern=""URL"" />
  <policy domain=""path"" rights=""none"" pattern=""@*"" />
  <policy domain=""path"" rights=""none"" pattern=""|*"" />
</policymap>";
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