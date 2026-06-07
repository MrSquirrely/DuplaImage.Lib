using System;
using System.IO;
using System.Numerics;
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
        public ulong CalculateAverageHash64(string pathToImage) {
            using var stream = new FileStream(pathToImage, FileMode.Open, FileAccess.Read);
            return AverageHash64.Calculate(stream, _transformer);
        }

        /// <summary>
        /// Calculates a 64 bit hash for the given image using average algorithm.
        /// </summary>
        /// <param name="image">Stream containing image.</param>
        /// <returns>64 bit average hash of the input image.</returns>
        public ulong CalculateAverageHash64(Stream image) => AverageHash64.Calculate(image, _transformer);

        /// <summary>
        /// Calculates a 64 bit hash for the given image using median algorithm.
        /// 
        /// Works by converting the image to 8x8 greyscale image, finding the median pixel value from it, and then marking
        /// all pixels where value is greater than median value as 1 in the resulting hash. Should be more resistant to non-linear
        /// image edits when compared against average based implementation.
        /// </summary>
        /// <param name="pathToImage">Path to an image to be hashed.</param>
        /// <returns>64 bit median hash of the input image.</returns>
        public ulong CalculateMedianHash64(string pathToImage) {
            using var stream = new FileStream(pathToImage, FileMode.Open, FileAccess.Read);
            return MedianHash64.Calculate(stream, _transformer);
        }

        /// <summary>
        /// Calculates a 64 bit hash for the given image using median algorithm.
        /// 
        /// Works by converting the image to 8x8 greyscale image, finding the median pixel value from it, and then marking
        /// all pixels where value is greater than median value as 1 in the resulting hash. Should be more resistant to non-linear
        /// image edits when compared against average based implementation.
        /// </summary>
        /// <param name="image">Stream containing image.</param>
        /// <returns>64 bit median hash of the input image.</returns>
        public ulong CalculateMedianHash64(Stream image) => MedianHash64.Calculate(image, _transformer);

        /// <summary>
        /// Calculates a 256 bit hash for the given image using median algorithm.
        /// 
        /// Works by converting the image to 16x16 greyscale image, finding the median pixel value from it, and then marking
        /// all pixels where value is greater than median value as 1 in the resulting hash. Should be more resistant to non-linear
        /// image edits when compared against average based implementation.
        /// </summary>
        /// <param name="pathToImage">Path to an image to be hashed.</param>
        /// <returns>256 bit median hash of the input image. Composed of 4 uLongs.</returns>
        public ulong[] CalculateMedianHash256(string pathToImage) {
            using var stream = new FileStream(pathToImage, FileMode.Open, FileAccess.Read);
            return MedianHash256.Calculate(stream, _transformer);
        }

        /// <summary>
        /// Calculates a 256 bit hash for the given image using median algorithm.
        /// 
        /// Works by converting the image to 16x16 greyscale image, finding the median pixel value from it, and then marking
        /// all pixels where value is greater than median value as 1 in the resulting hash. Should be more resistant to non-linear
        /// image edits when compared against average based implementation.
        /// </summary>
        /// <param name="image">Stream containing image.</param>
        /// <returns>256 bit median hash of the input image. Composed of 4 uLongs.</returns>
        public ulong[] CalculateMedianHash256(Stream image) => MedianHash256.Calculate(image, _transformer);

        /// <summary>
        /// Calculates 64 bit hash for the given image using difference hash.
        /// 
        /// See http://www.hackerfactor.com/blog/index.php?/archives/529-Kind-of-Like-That.html for algorithm description.
        /// </summary>
        /// <param name="pathToImage">Path to an image to be hashed.</param>
        /// <returns>64 bit difference hash of the input image.</returns>
        public ulong CalculateDifferenceHash64(string pathToImage) {
            using var stream = new FileStream(pathToImage, FileMode.Open, FileAccess.Read);
            return DifferenceHash64.Calculate(stream, _transformer);
        }

        /// <summary>
        /// Calculates 64 bit hash for the given image using difference hash.
        /// 
        /// See http://www.hackerfactor.com/blog/index.php?/archives/529-Kind-of-Like-That.html for algorithm description.
        /// </summary>
        /// <param name="image">Stream containing image.</param>
        /// <returns>64 bit difference hash of the input image.</returns>
        public ulong CalculateDifferenceHash64(Stream image) => DifferenceHash64.Calculate(image, _transformer);

        /// <summary>
        /// Calculates 256 bit hash for the given image using difference hash.
        /// 
        /// See http://www.hackerfactor.com/blog/index.php?/archives/529-Kind-of-Like-That.html for algorithm description.
        /// </summary>
        /// <param name="pathToImage">Path to an image to be hashed.</param>
        /// <returns>64 bit difference hash of the input image.</returns>
        public ulong[] CalculateDifferenceHash256(string pathToImage) {
            using var stream = new FileStream(pathToImage, FileMode.Open, FileAccess.Read);
            return DifferenceHash256.Calculate(stream, _transformer);
        }

        /// <summary>
        /// Calculates 256 bit hash for the given image using difference hash.
        /// 
        /// See http://www.hackerfactor.com/blog/index.php?/archives/529-Kind-of-Like-That.html for algorithm description.
        /// </summary>
        /// <param name="image">Stream containing image.</param>
        /// <returns>64 bit difference hash of the input image.</returns>
        public ulong[] CalculateDifferenceHash256(Stream image) => DifferenceHash256.Calculate(image, _transformer);

        /// <summary>
        /// Calculates a hash for the given image using dct algorithm
        /// </summary>
        /// <param name="path">Path to the image used for hash calculation.</param>
        /// <returns>64 bit difference hash of the input image.</returns>
        public ulong CalculateDctHash(string path) {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            return new DCTHash().Calculate(stream, _transformer);
        }

        /// <summary>
        /// Calculates a hash for the given image using dct algorithm
        /// </summary>
        /// <param name="image">Stream containing image.</param>
        /// <returns>64 bit difference hash of the input image.</returns>
        public ulong CalculateDctHash(Stream image) => new DCTHash().Calculate(image, _transformer);

        /// <summary>
        /// Compare hashes of two images using Hamming distance. Result of 1 indicates images being 
        /// same, while result of 0 indicates completely different images.
        /// </summary>
        /// <param name="hash1">First hash to be compared</param>
        /// <param name="hash2">Second hash to be compared</param>
        /// <returns>Image similarity in range [0,1]</returns>
        public float CompareHashes(ulong hash1, ulong hash2) {
            // XOR hashes and calculate ones using hardware intrinsic PopCount
            int onesInHash = BitOperations.PopCount(hash1 ^ hash2);

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
            int onesInHash = 0;

            // XOR hashes and calculate ones using hardware intrinsic PopCount, avoiding allocations
            for (int i = 0; i < hashSize; i++) {
                onesInHash += BitOperations.PopCount(hash1[i] ^ hash2[i]);
            }

            // Return result as a float between 0 and 1.
            return 1.0f - (onesInHash / (hashSize * 64.0f));    //Assuming 64bit variables
        }
    }
}
