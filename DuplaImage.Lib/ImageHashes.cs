using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DuplaImage.Lib.Hashes;

namespace DuplaImage.Lib {
    public class ImageHashes {
        private readonly IImageTransformer _transformer;

        /// <summary>
        /// Initializes a new instance of the ImageHashes class using the given IImageTransformer.
        /// </summary>
        /// <param name="transformer">Implementation of the IImageTransformer to be used for image transformation.</param>
        public ImageHashes(IImageTransformer transformer) => _transformer = transformer;

        /// <summary>
        /// Calculates a 64 bit hash for the given image using average algorithm.
        /// </summary>
        /// <param name="pathToImage">Path to an image to be hashed.</param>
        /// <returns>64 bit average hash of the input image.</returns>
        public ulong CalculateAverageHash64(string pathToImage) => AverageHash64.Calculate(new FileStream(pathToImage, FileMode.Open, FileAccess.Read), _transformer);

        /// <summary>
        /// Calculates a 64 bit hash for the given image using median algorithm.
        /// 
        /// Works by converting the image to 8x8 greyscale image, finding the median pixel value from it, and then marking
        /// all pixels where value is greater than median value as 1 in the resulting hash. Should be more resistant to non-linear
        /// image edits when compared against average based implementation.
        /// </summary>
        /// <param name="pathToImage">Path to an image to be hashed.</param>
        /// <returns>64 bit median hash of the input image.</returns>
        public ulong CalculateMedianHash64(string pathToImage) => MedianHash64.Calculate(new FileStream(pathToImage, FileMode.Open, FileAccess.Read), _transformer);

        /// <summary>
        /// Calculates a 256 bit hash for the given image using median algorithm.
        /// 
        /// Works by converting the image to 16x16 greyscale image, finding the median pixel value from it, and then marking
        /// all pixels where value is greater than median value as 1 in the resulting hash. Should be more resistant to non-linear
        /// image edits when compared against average based implementation.
        /// </summary>
        /// <param name="pathToImage">Path to an image to be hashed.</param>
        /// <returns>256 bit median hash of the input image. Composed of 4 ulongs.</returns>
        public ulong[] CalculateMedianHash256(string pathToImage) => MedianHash256.Calculate(new FileStream(pathToImage, FileMode.Open, FileAccess.Read), _transformer);

        /// <summary>
        /// Calculates 64 bit hash for the given image using difference hash.
        /// 
        /// See http://www.hackerfactor.com/blog/index.php?/archives/529-Kind-of-Like-That.html for algorithm description.
        /// </summary>
        /// <param name="pathToImage">Path to an image to be hashed.</param>
        /// <returns>64 bit difference hash of the input image.</returns>
        public ulong CalculateDifferenceHash64(string pathToImage) => DifferenceHash64.Calculate(new FileStream(pathToImage, FileMode.Open, FileAccess.Read), _transformer);

        /// <summary>
        /// Calculates 256 bit hash for the given image using difference hash.
        /// 
        /// See http://www.hackerfactor.com/blog/index.php?/archives/529-Kind-of-Like-That.html for algorithm description.
        /// </summary>
        /// <param name="pathToImage">Path to an image to be hashed.</param>
        /// <returns>64 bit difference hash of the input image.</returns>
        public ulong[] CalculateDifferenceHash256(string pathToImage) => DifferenceHash256.Calculate(new FileStream(pathToImage, FileMode.Open, FileAccess.Read), _transformer);

        /// <summary>
        /// Calculates a hash for the given image using dct algorithm
        /// </summary>
        /// <param name="path">Path to the image used for hash calculation.</param>
        /// <returns>64 bit difference hash of the input image.</returns>
        public ulong CalculateDctHash(string path) => new DCTHash().Calculate(new FileStream(path, FileMode.Open, FileAccess.Read),_transformer);

        /// <summary>
        /// Compare hashes of two images using Hamming distance. Result of 1 indicates images being 
        /// same, while result of 0 indicates completely different images.
        /// </summary>
        /// <param name="hash1">First hash to be compared</param>
        /// <param name="hash2">Second hash to be compared</param>
        /// <returns>Image similarity in range [0,1]</returns>
        public float CompareHashes(ulong hash1, ulong hash2) {
            // XOR hashes
            ulong hashDifference = hash1 ^ hash2;

            // Calculate ones
            ulong onesInHash = HammingWeight(hashDifference);

            // Return result as a float between 0 and 1.
            return 1.0f - (onesInHash / 64.0f);
        }

        /// <summary>
        /// Compare hashes of two images using Hamming distance. Result of 1 indicates images being 
        /// same, while result of 0 indicates completely different images. Hash size is inferred from 
        /// the size of Hash array in first image.
        /// </summary>
        /// <param name="hash1">First hash to be compared</param>
        /// <param name="hash2">Second hash to be compared</param>
        /// <returns>Image similarity in range [0,1]</returns>
        public float CompareHashes(ulong[] hash1, ulong[] hash2) {
            // Check that hash lengths are same
            if (hash1.Length != hash2.Length) {
                throw new ArgumentException("Lengths of hash1 and hash2 do not match.");
            }

            int hashSize = hash1.Length;
            ulong onesInHash = 0;

            // XOR hashes
            ulong[] hashDifference = new ulong[hashSize];
            for (int i = 0; i < hashSize; i++)  // Slightly faster than foreach
            {
                hashDifference[i] = hash1[i] ^ hash2[i];
            }

            // Calculate ones
            for (int i = 0; i < hashSize; i++) {
                onesInHash += HammingWeight(hashDifference[i]);
            }

            // Return result as a float between 0 and 1.
            return 1.0f - (onesInHash / (hashSize * 64.0f));    //Assuming 64bit variables
        }

        /// <summary>
        /// Calculate ones in hash using Hamming weight. See http://en.wikipedia.org/wiki/Hamming_weight
        /// </summary>
        /// <param name="hash">Input value</param>
        /// <returns>Count of ones in input value</returns>
        private ulong HammingWeight(ulong hash) {
            hash -= (hash >> 1) & M1;
            hash = (hash & M2) + ((hash >> 2) & M2);
            hash = (hash + (hash >> 4)) & M4;
            ulong onesInHash = (hash * H01) >> 56;

            return onesInHash;
        }

        // Hamming distance constants. See http://en.wikipedia.org/wiki/Hamming_weight for explanation.
        private const ulong M1 = 0x5555555555555555; //binary: 0101...
        private const ulong M2 = 0x3333333333333333; //binary: 00110011..
        private const ulong M4 = 0x0f0f0f0f0f0f0f0f; //binary:  4 zeros,  4 ones ...
        private const ulong H01 = 0x0101010101010101; //the sum of 256 to the power of 0,1,2,3...
    }
}
