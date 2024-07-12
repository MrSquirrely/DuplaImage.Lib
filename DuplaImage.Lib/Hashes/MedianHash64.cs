using System.Collections.Generic;
using System.IO;

namespace DuplaImage.Lib.Hashes {
    internal static class MedianHash64 {
        /// <summary>
        /// Calculates a 64 bit hash for the given image using median algorithm.
        /// 
        /// Works by converting the image to 8x8 greyscale image, finding the median pixel value from it, and then marking
        /// all pixels where value is greater than median value as 1 in the resulting hash. Should be more resistant to non-linear
        /// image edits when compared against average based implementation.
        /// </summary>
        /// <param name="sourceStream">Stream containing an image to be hashed.</param>
        /// <param name="transformer">Transformer to use.</param>
        /// <returns>64 bit median hash of the input image.</returns>
        internal static ulong Calculate(Stream sourceStream, IImageTransformer transformer) {
            byte[] pixels = transformer.TransformImage(sourceStream, 8, 8);

            // Calculate median
            List<byte> pixelList = new(pixels);
            pixelList.Sort();
            // Even amount of pixels
            byte median = (byte)((pixelList[31] + pixelList[32]) / 2);

            // Iterate pixels and set them to 1 if over median and 0 if lower.
            ulong hash = 0UL;
            for (int i = 0; i < 64; i++) {
                if (pixels[i] > median) {
                    hash |= 1UL << i;
                }
            }

            // Done
            return hash;
        }
    }
}
