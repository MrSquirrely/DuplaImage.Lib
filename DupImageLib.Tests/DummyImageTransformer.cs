using System.IO;

namespace DuplaImage.Lib.Tests {
    public class DummyImageTransformer : IImageTransformer {
        public byte[] TransformImage(Stream stream, int width, int height) {
            byte[] pixels = new byte[width * height];
            byte pixelValue = 0;
            for (int i = 0; i < pixels.Length; i++) {
                pixels[i] = pixelValue;

                if (pixelValue == 255) {
                    pixelValue = 0;
                }
                else {
                    pixelValue++;
                }
            }

            return pixels;
        }
    }
}