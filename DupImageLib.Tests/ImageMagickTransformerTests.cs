using Xunit;

namespace DuplaImage.Lib.Tests {
    public class ImageMagickTransformerTests {
        private readonly ImageHashes _imgHashes = new ImageHashes(new ImageMagickTransformer());
        
        [Fact]
        public void ComputeHash() => Assert.Equal(0x0f0f0f0f0f0f0f0fUL, _imgHashes.CalculateMedianHash64("testPattern1.png"));

        [Fact]
        public void CompareHashes_Size64() => Assert.InRange(
            ImageHashes.CompareHashes(_imgHashes.CalculateMedianHash64("testPattern1.png"), 
                _imgHashes.CalculateMedianHash64("testPattern2.png")), 0.49, 0.51);
    }
}
