using System;
using System.IO;
using Xunit;

namespace DuplaImage.Lib.Tests {
    public class ImageHashesTests {
        private readonly ImageHashes _imgHashes = new ImageHashes(new DummyImageTransformer());
        
        [Fact]
        public void CalculateDifferenceHash64() => Assert.Equal(0UL, _imgHashes.CalculateDifferenceHash64((Stream)null));

        [Fact]
        public void CalculateDifferenceHash256() {
            ulong[] hash = _imgHashes.CalculateDifferenceHash256((Stream) null);

            Assert.Equal(0UL, hash[0]);
            Assert.Equal(0UL, hash[1]);
            Assert.Equal(0UL, hash[2]);
            Assert.Equal(0x0001000000000000UL, hash[3]);
        }

        [Fact]
        public void CalculateMedianHash64() => Assert.Equal(0xffffffff00000000, _imgHashes.CalculateMedianHash64((Stream)null));

        [Fact]
        public void CalculateMedianHash256() {
            ulong[] hash = _imgHashes.CalculateMedianHash256((Stream) null);

            Assert.Equal(0x0000000000000000UL, hash[0]);
            Assert.Equal(0x0000000000000000UL, hash[1]);
            Assert.Equal(0xffffffffffffffffUL, hash[2]);
            Assert.Equal(0xffffffffffffffffUL, hash[3]);
        }

        [Fact]
        public void CalculateAverageHash64() => Assert.Equal(0xffffffff00000000, _imgHashes.CalculateAverageHash64((Stream)null));

        [Fact]
        public void CalculateDctHash64() => Assert.Equal(0xa4f8d63986aa52ad, _imgHashes.CalculateDctHash((Stream)null));

        [Fact]
        public void CompareHashes_notEqualLength() {
            ulong[] hash1 = new ulong[1];
            ulong[] hash2 = new ulong[2];

            Exception exception = Record.Exception(() => ImageHashes.CompareHashes(hash1, hash2));
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        public void CompareHashes_identicalHashesSize64() {
            ulong[] hash1 = new ulong[1];
            ulong[] hash2 = new ulong[1];

            hash1[0] = 0x0fff0000ffff0000;
            hash2[0] = 0x0fff0000ffff0000;

            float result = ImageHashes.CompareHashes(hash1, hash2);
            Assert.Equal(1.0f, result, 4);
        }

        [Fact]
        public void CompareHashes_identicalHashesSize256() {
            ulong[] hash1 = new ulong[4];
            ulong[] hash2 = new ulong[4];

            hash1[0] = 0x0fff0000ffff0000;
            hash1[1] = 0x0fff0000ffff0000;
            hash1[2] = 0x0fff0000ffff0000;
            hash1[3] = 0x0fff0000ffff0000;

            hash2[0] = 0x0fff0000ffff0000;
            hash2[1] = 0x0fff0000ffff0000;
            hash2[2] = 0x0fff0000ffff0000;
            hash2[3] = 0x0fff0000ffff0000;

            float result = ImageHashes.CompareHashes(hash1, hash2);
            Assert.Equal(1.0f, result, 4);
        }

        [Fact]
        public void CompareHashes_nonIdenticalHashes() {
            ulong[] hash1 = new ulong[1];
            ulong[] hash2 = new ulong[1];

            hash1[0] = 0L;
            hash2[0] = ulong.MaxValue;

            float result = ImageHashes.CompareHashes(hash1, hash2);
            Assert.Equal(0.0f, result, 4);
        }

        [Fact]
        public void CompareHashes_halfIdenticalHashes() {
            ulong[] hash1 = new ulong[1];
            ulong[] hash2 = new ulong[1];

            hash1[0] = 0x00000000ffffffff;
            hash2[0] = ulong.MaxValue;

            float result = ImageHashes.CompareHashes(hash1, hash2);
            Assert.Equal(0.5f, result, 4);
        }

        [Theory, InlineData(0x0fff0000ffff0000, 0x0fff0000ffff0000, 1.0f),
         InlineData(0UL, ulong.MaxValue, 0.0f),
         InlineData(0x00000000ffffffff, ulong.MaxValue, 0.5f)]
        public void CompareHashes_ulongVersion(ulong hash1, ulong hash2, float similarity) {
            float result = ImageHashes.CompareHashes(hash1, hash2);
            Assert.Equal(similarity, result, 4);
        }
    }
}
