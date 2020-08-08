using System.IO;
using ImageMagick;

namespace DuplaImage.Lib.ImageMagick {
    /// <summary>
    /// Implements IImageTransformer interface using Magick.NET for image transforms.
    /// </summary>
    public class ImageMagickTransformer : IImageTransformer {
        public byte[] TransformImage(Stream stream, int width, int height) {
            // Read image
            MagickImage magickImage = new MagickImage(stream);
            QuantizeSettings settings = new QuantizeSettings {
                ColorSpace = ColorSpace.Gray,
                Colors = 256
            };
            magickImage.Quantize(settings);
            MagickGeometry size = new MagickGeometry(width, height) { IgnoreAspectRatio = true };
            magickImage.Resize(size);
            magickImage.Format = MagickFormat.Gray;
            return magickImage.ToByteArray();
        }
    }
}