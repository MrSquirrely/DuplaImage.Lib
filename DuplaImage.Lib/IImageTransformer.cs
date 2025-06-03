﻿using System.IO;

namespace DuplaImage.Lib {
    /// <summary>
    /// Interface used for implementing image conversion operations in the hashing functions in ImageHashes class.
    /// </summary>
    public interface IImageTransformer {
        /// <summary>
        /// Converts given image in a stream to a gray-scale image with single 8 bit color channel and resizes it to the given width and height.
        /// Aspect ratio should be ignored during resizing operation.
        /// </summary>
        /// <param name="stream">Stream to the image to be converted.</param>
        /// <param name="width">Width of the resized image.</param>
        /// <param name="height">Height of the resized image.</param>
        /// <returns>Byte array containing 8 bit pixel values of the converted image.</returns>
        byte[] TransformImage(Stream stream, uint width, uint height);
    }
}