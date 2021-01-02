using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DuplaImage.Lib.Hashes {
    internal static class DifferenceHash64 {
        /// <summary>
        /// Calculates 64 bit hash for the given image using difference hash.
        /// 
        /// See http://www.hackerfactor.com/blog/index.php?/archives/529-Kind-of-Like-That.html for algorithm description.
        /// </summary>
        /// <param name="sourceStream">Stream containing an image to be hashed.</param>
        /// <returns>64 bit difference hash of the input image.</returns>
        internal static ulong Calculate(Stream sourceStream, IImageTransformer _transformer) {
            byte[] pixels = _transformer.TransformImage(sourceStream, 9, 8);

            // Iterate pixels and set hash to 1 if the left pixel is brighter than the right pixel.
            ulong hash = 0UL;
            int hashPos = 0;
            for (int i = 0; i < 8; i++) {
                int rowStart = i * 9;
                for (int j = 0; j < 8; j++) {
                    if (pixels[rowStart + j] > pixels[rowStart + j + 1]) {
                        hash |= 1UL << hashPos;
                    }
                    hashPos++;
                }
            }

            // Done
            return hash;
        }
    }
}
