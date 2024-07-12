using System;
using System.Collections.Generic;
using System.IO;

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

            // Calculate dct
            float[][] dctPixels = ComputeDct(fPixels, _dctMatrix);

            // Get 8*8 area from 1,1 to 8,8, ignoring lowest frequencies for improved detection
            float[] dctHashPixels = new float[64];
            for (int x = 0; x < 8; x++) {
                for (int y = 0; y < 8; y++) {
                    dctHashPixels[x + (y * 8)] = dctPixels[x + 1][y + 1];
                }
            }

            // Calculate median
            List<float> pixelList = new(dctHashPixels);
            pixelList.Sort();
            // Even amount of pixels
            float median = (pixelList[31] + pixelList[32]) / 2;

            // Iterate pixels and set them to 1 if over median and 0 if lower.
            ulong hash = 0UL;
            for (int i = 0; i < 64; i++) {
                if (dctHashPixels[i] > median) {
                    hash |= 1UL << i;
                }
            }

            // Done
            return hash;
        }

        /// <summary>
        /// Compute DCT for the image.
        /// </summary>
        /// <param name="image">Image to calculate the dct.</param>
        /// <param name="dctMatrix">DCT coefficient matrix</param>
        /// <returns>DCT transform of the image</returns>
        private static float[][] ComputeDct(IReadOnlyList<float> image, float[][] dctMatrix) {
            // Get the size of dct matrix. We assume that the image is same size as dctMatrix
            int size = dctMatrix.GetLength(0);

            // Make image matrix
            float[][] imageMat = new float[size][];
            for (int i = 0; i < size; i++) {
                imageMat[i] = new float[size];
            }

            for (int y = 0; y < size; y++) {
                for (int x = 0; x < size; x++) {
                    imageMat[y][x] = image[x + (y * size)];
                }
            }

            return Multiply(Multiply(dctMatrix, imageMat), Transpose(dctMatrix));
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

        /// <summary>
        /// Matrix multiplication.
        /// </summary>
        /// <param name="a">First matrix.</param>
        /// <param name="b">Second matrix.</param>
        /// <returns>Result matrix.</returns>
        private static float[][] Multiply(IReadOnlyList<float[]> a, IReadOnlyList<float[]> b) {
            int n = a[0].Length;
            float[][] c = new float[n][];
            for (int i = 0; i < n; i++) {
                c[i] = new float[n];
            }

            for (int i = 0; i < n; i++) {
                for (int k = 0; k < n; k++) {
                    for (int j = 0; j < n; j++)
                        c[i][j] += a[i][k] * b[k][j];
                }
            }

            return c;
        }

        /// <summary>
        /// Transposes square matrix.
        /// </summary>
        /// <param name="mat">Matrix to be transposed</param>
        /// <returns>Transposed matrix</returns>
        private static float[][] Transpose(IReadOnlyList<float[]> mat) {
            int size = mat[0].Length;
            float[][] transpose = new float[size][];

            for (int i = 0; i < size; i++) {
                transpose[i] = new float[size];
                for (int j = 0; j < size; j++)
                    transpose[i][j] = mat[j][i];
            }
            return transpose;
        }

    }
}
