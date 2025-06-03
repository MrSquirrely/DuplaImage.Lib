using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace DuplaImage.Lib.ImageSharpTransformer {
    /// <summary>
    /// Implements IImageTransformer interface using Magick.NET for image transforms.
    /// </summary>
    public class ImageSharpTransformer : IImageTransformer {
        public byte[] TransformImage(Stream stream, uint width, uint height) {
            using Image<Rgba32> image = Image.Load<Rgba32>(stream);

            image.Mutate(x => x
                .Resize((int)width, (int)height, KnownResamplers.Bicubic)
                .Grayscale());

            // Convert to grayscale and extract pixel data
            byte[] result = new byte[width * height];
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    result[(y * width) + x] = image[x, y].R; // In grayscale, R=G=B
                }
            }

            return result;
        }
    }
}
