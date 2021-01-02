using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DuplaImage.Lib.Hashes {
    internal static class AverageHash64 {
        /// <summary>
        /// Calculates a 64 bit hash for the given image using average algorithm.
        /// </summary>
        /// <param name="sourceStream">Stream containing an image to be hashed.</param>
        /// <returns>64 bit average hash of the input image.</returns>
        internal static ulong Calculate(Stream sourceStream, IImageTransformer _transformer) {
            byte[] pixels = _transformer.TransformImage(sourceStream, 8, 8);

            // Calculate average
            List<byte> pixelList = new List<byte>(pixels);
            int total = pixelList.Aggregate(0, (current, pixel) => current + pixel);
            int average = total / 64;

            // Iterate pixels and set them to 1 if over average and 0 if lower.
            ulong hash = 0UL;
            for (int i = 0; i < 64; i++) {
                if (pixels[i] > average) {
                    hash |= 1UL << i;
                }
            }

            // Done
            return hash;
        }
    }
}
