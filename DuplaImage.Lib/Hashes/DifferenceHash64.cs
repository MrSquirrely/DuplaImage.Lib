using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace DuplaImage.Lib.Hashes {
    internal static class DifferenceHash64 {
        /// <summary>
        /// Calculates 64 bit hash for the given image using difference hash.
        /// 
        /// See http://www.hackerfactor.com/blog/index.php?/archives/529-Kind-of-Like-That.html for algorithm description.
        /// </summary>
        /// <param name="sourceStream">Stream containing an image to be hashed.</param>
        /// <param name="transformer">Transformer to use</param>
        /// <returns>64 bit difference hash of the input image.</returns>
        internal static ulong Calculate(Stream sourceStream, IImageTransformer transformer) {
            byte[] pixels = transformer.TransformImage(sourceStream, 9, 8);
            ReadOnlySpan<byte> pixelSpan = pixels;

            // Iterate pixels and set hash to 1 if the left pixel is brighter than the right pixel.
            ulong hash = CalculateHash(pixelSpan);

            // Done
            return hash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong CalculateHash(ReadOnlySpan<byte> pixelSpan) {
            ulong hash = 0UL;
            int hashPos = 0;

            for (int i = 0; i < 8; i++) {
                int rowStart = i * 9;
                for (int j = 0; j < 8; j++) {
                    if (pixelSpan[rowStart + j] > pixelSpan[rowStart + j + 1]) {
                        hash |= 1UL << hashPos;
                    }
                    hashPos++;
                }
            }

            return hash;
        }
    }
}
