using System;
using System.IO;

namespace DuplaImage.Lib {
    /// <summary>
    /// Implements IImageTransformer interface using Magick.NET for image transforms.
    /// </summary>
    public class ImageMagickTransformer : IImageTransformer {
        [Obsolete("Use DuplaImage.Lib.ImageMagick on nuget.", true)]
        public byte[] TransformImage(Stream stream, int width, int height) {
            return null;
        }
    }
}