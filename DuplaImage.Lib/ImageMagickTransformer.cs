using System.IO;
using ImageMagick;

namespace DuplaImage.Lib {
    /// <summary>
    /// Implements IImageTransformer interface using Magick.NET for image transforms.
    /// </summary>
    public class ImageMagickTransformer : IImageTransformer {
        public byte[] TransformImage(Stream stream, int width, int height) {
            // Read image
            MagickImage img = new MagickImage(stream);
            QuantizeSettings settings = new QuantizeSettings {
                ColorSpace = ColorSpace.Gray,
                Colors = 256
            };
            img.Quantize(settings);
            MagickGeometry size = new MagickGeometry(width, height) { IgnoreAspectRatio = true };
            img.Resize(size);
            img.Format = MagickFormat.Gray;
            return img.ToByteArray();
        }
    }
}