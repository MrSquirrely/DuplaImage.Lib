## 2024-05-26 - Span<T> Allocation over List<T> for Hashing
**Learning:** In C#, replacing `List<T>` with `stackalloc Span<T>` for small, fixed-size pixel arrays in hot paths (like hashing algorithms) provides a substantial performance boost by eliminating heap allocations and GC pressure.
**Action:** When finding medians or processing small, fixed-size data within inner loops, default to using stack-allocated Spans instead of List or Array instantiations.

## 2024-06-07 - Hardware Intrinsics over Software Bitwise Operations
**Learning:** For bitwise operations like popcount (Hamming weight), using modern hardware intrinsics provided by .NET (like `System.Numerics.BitOperations.PopCount`) is vastly faster than a manual software implementation (e.g. shifts and masks). This relies on fast hardware instructions like `POPCNT`. In `ImageHashes`, replacing the custom `HammingWeight` method with `PopCount` significantly reduced the hash comparison duration, with micro-benchmarks showing operations taking roughly 30% of the previous time on 64-bit comparisons, and up to a 30x speedup when eliminating unnecessary array allocations on array-based comparisons.
**Action:** Always prefer `System.Numerics.BitOperations` or `System.Runtime.Intrinsics` for common bit-twiddling and bit counting problems instead of manual bit manipulation algorithms to leverage modern CPU instructions.
