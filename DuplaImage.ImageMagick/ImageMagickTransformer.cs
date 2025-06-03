using System.IO;
using ImageMagick;

namespace DuplaImage.Lib.ImageMagick {
    /// <summary>
    /// Implements IImageTransformer interface using Magick.NET for image transforms.
    /// </summary>
    public class ImageMagickTransformer : IImageTransformer {

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