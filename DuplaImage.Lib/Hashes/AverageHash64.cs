using System;
using System.IO;

namespace DuplaImage.Lib.Hashes {
    internal static class AverageHash64 {
        /// <summary>
        /// Calculates a 64 bit hash for the given image using average algorithm.
        /// </summary>
        /// <param name="sourceStream">Stream containing an image to be hashed.</param>
        /// <param name="transformer">Transformer to use</param>
        /// <returns>64 bit average hash of the input image.</returns>
        internal static ulong Calculate(Stream sourceStream, IImageTransformer transformer) {
            ReadOnlySpan<byte> pixelSpan = transformer.TransformImage(sourceStream, 8, 8);

            // Calculate average using direct sum without allocations
            int total = 0;
            for (int i = 0; i < 64; i++) {
                total += pixelSpan[i];
            }
            byte average = (byte)(total >> 6); // Divide by 64 using bit shift

            // Build hash using optimized bit operations
            ulong hash = 0UL;
            for (int i = 0; i < 64; i += 8) {
                byte chunk = 0;
                // Process 8 pixels at once
                for (int j = 0; j < 8; j++) {
                    if (pixelSpan[i + j] > average) {
                        chunk |= (byte)(1 << j);
                    }
                }
                hash |= (ulong)chunk << i;
            }

            return hash;
        }
    }
}
