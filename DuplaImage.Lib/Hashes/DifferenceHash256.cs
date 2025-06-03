using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace DuplaImage.Lib.Hashes {
    internal static class DifferenceHash256 {
        /// <summary>
        /// Calculates 256 bit hash for the given image using difference hash.
        /// 
        /// See http://www.hackerfactor.com/blog/index.php?/archives/529-Kind-of-Like-That.html for algorithm description.
        /// </summary>
        /// <param name="sourceStream">Stream containing an image to be hashed.</param>
        /// <param name="transformer">Transformer to use</param>
        /// <returns>64 bit difference hash of the input image.</returns>
        internal static ulong[] Calculate(Stream sourceStream, IImageTransformer transformer) {
            byte[] pixels = transformer.TransformImage(sourceStream, 17, 16);
            ReadOnlySpan<byte> pixelSpan = pixels;

            // Iterate pixels and set hash to 1 if the left pixel is brighter than the right pixel.
            ulong[] hash = new ulong[4];

            CalculateHashes(pixelSpan, hash);

            // Done
            return hash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CalculateHashes(ReadOnlySpan<byte> pixelSpan, ulong[] hash) {
            int hashPos = 0;
            int hashPart = 0;

            for (int i = 0; i < 16; i++) {
                int rowStart = i * 17;
                for (int j = 0; j < 16; j++) {
                    if (pixelSpan[rowStart + j] > pixelSpan[rowStart + j + 1]) {
                        hash[hashPart] |= 1UL << hashPos;
                    }

                    if (hashPos == 63) {
                        hashPos = 0;
                        hashPart++;
                    }
                    else {
                        hashPos++;
                    }
                }
            }
        }
    }
}
