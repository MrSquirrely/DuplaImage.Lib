using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DuplaImage.Lib.Hashes {
    internal static class MedianHash256 {

        /// <summary>
        /// Calculates a 256 bit hash for the given image using median algorithm.
        /// 
        /// Works by converting the image to 16x16 greyscale image, finding the median pixel value from it, and then marking
        /// all pixels where value is greater than median value as 1 in the resulting hash. Should be more resistant to non-linear
        /// image edits when compared agains average based implementation.
        /// </summary>
        /// <param name="sourceStream">Stream containing an image to be hashed.</param>
        /// <returns>256 bit median hash of the input image. Composed of 4 ulongs.</returns>
        internal static ulong[] Calculate(Stream sourceStream, IImageTransformer _transformer) {
            byte[] pixels = _transformer.TransformImage(sourceStream, 16, 16);

            // Calculate median
            List<byte> pixelList = new List<byte>(pixels);
            pixelList.Sort();
            // Even amount of pixels
            byte median = (byte)((pixelList[127] + pixelList[128]) / 2);

            // Iterate pixels and set them to 1 if over median and 0 if lower.
            ulong hash64 = 0UL;
            ulong[] hash = new ulong[4];
            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 64; j++) {
                    if (pixels[(64 * i) + j] > median) {
                        hash64 |= 1UL << j;
                    }
                }
                hash[i] = hash64;
                hash64 = 0UL;
            }

            // Done
            return hash;
        }

    }
}
