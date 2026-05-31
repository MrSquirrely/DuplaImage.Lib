using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace DuplaImage.Lib.Hashes {
    internal class DCTHash {

        private float[][] _dctMatrix;
        private bool _isDctMatrixInitialized;
        private readonly object _dctMatrixLockObject = new();

        /// <summary>
        /// Calculates a hash for the given image using dct algorithm
        /// </summary>
        /// <param name="sourceStream">Stream to the image used for hash calculation.</param>
        /// <param name="transformer">Transformer to use</param>
        /// <returns>64 bit difference hash of the input image.</returns>
        internal ulong Calculate(Stream sourceStream, IImageTransformer transformer) {
            lock (_dctMatrixLockObject) {
                if (!_isDctMatrixInitialized) {
                    _dctMatrix = GenerateDctMatrix(32);
                    _isDctMatrixInitialized = true;
                }
            }

            byte[] pixels = transformer.TransformImage(sourceStream, 32, 32);

            // Copy pixel data and convert to float
            float[] fPixels = new float[1024];
            for (int i = 0; i < 1024; i++) {
                fPixels[i] = pixels[i] / 255.0f;
            }

            // Calculate dct and get 8*8 area from 1,1 to 8,8, ignoring lowest frequencies for improved detection
            float[] dctHashPixels = ComputeDctHashPixels(fPixels, _dctMatrix);

            // Calculate median
            Span<float> sortedPixels = stackalloc float[64];
            dctHashPixels.AsSpan().CopyTo(sortedPixels);
            sortedPixels.Sort();
            // Even amount of pixels
            float median = (sortedPixels[31] + sortedPixels[32]) / 2;

            // Iterate pixels and set them to 1 if over median and 0 if lower.
            ulong hash = CalculateHash(dctHashPixels, median);

            // Done
            return hash;
        }

        /// <summary>
        /// Compute DCT for the image and return the 8x8 crop of frequencies used for the hash.
        /// Optimized to only calculate the necessary 8x8 subset instead of the full 32x32 matrix.
        /// </summary>
        /// <param name="image">Image to calculate the dct.</param>
        /// <param name="dctMatrix">DCT coefficient matrix</param>
        /// <returns>8x8 DCT transform subset of the image</returns>
        private static float[] ComputeDctHashPixels(float[] image, float[][] dctMatrix) {
            // Hardcoded size for DCT hash images
            int size = 32;

            float[] bRow = new float[size];
            float[] dctHashPixels = new float[64];

            for (int x = 0; x < 8; x++) {
                int i = x + 1;
                float[] dctRowI = dctMatrix[i];
                Array.Clear(bRow, 0, size);

                for (int k = 0; k < size; k++) {
                    float a_ik = dctRowI[k];
                    int kSize = k * size;
                    for (int j = 0; j < size; j++) {
                        bRow[j] += a_ik * image[kSize + j];
                    }
                }

                for (int y = 0; y < 8; y++) {
                    int j = y + 1;
                    float sum = 0;
                    float[] dctRowJ = dctMatrix[j];
                    for (int k = 0; k < size; k++) {
                        sum += bRow[k] * dctRowJ[k];
                    }
                    dctHashPixels[x + (y * 8)] = sum;
                }
            }

            return dctHashPixels;
        }

        /// <summary>
        /// Generates DCT coefficient matrix.
        /// </summary>
        /// <param name="size">Size of the matrix.</param>
        /// <returns>Coefficient matrix.</returns>
        private static float[][] GenerateDctMatrix(int size) {
            float[][] matrix = new float[size][];
            for (int i = 0; i < size; i++) {
                matrix[i] = new float[size];
            }

            double c1 = Math.Sqrt(2.0f / size);

            for (int j = 0; j < size; j++) {
                matrix[0][j] = (float)Math.Sqrt(1.0d / size);
            }

            for (int j = 0; j < size; j++) {
                for (int i = 1; i < size; i++) {
                    matrix[i][j] = (float)(c1 * Math.Cos(((2 * j) + 1) * i * Math.PI / (2.0d * size)));
                }
            }
            return matrix;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong CalculateHash(float[] dctHashPixels, float median) {
            ulong hash = 0UL;
            for (int i = 0; i < 64; i++) {
                if (dctHashPixels[i] > median) {
                    hash |= 1UL << i;
                }
            }
            return hash;
        }
    }
}
