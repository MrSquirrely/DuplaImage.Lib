## 2024-05-26 - Span<T> Allocation over List<T> for Hashing
**Learning:** In C#, replacing `List<T>` with `stackalloc Span<T>` for small, fixed-size pixel arrays in hot paths (like hashing algorithms) provides a substantial performance boost by eliminating heap allocations and GC pressure.
**Action:** When finding medians or processing small, fixed-size data within inner loops, default to using stack-allocated Spans instead of List or Array instantiations.
## 2023-10-25 - Extraneous Math Computation in Matrix Operations
**Learning:** The DCT image hashing implementation in C# performed full $O(n^3)$ multi-dimensional array matrix multiplication and transposition to calculate a 32x32 transformation, only to discard 98% of the data and use an 8x8 crop.
**Action:** When working with image transformations or mathematics algorithms that compute intermediate states only to slice a small fragment, refactor to compute the subset directly without allocations. Removing large multi-dimensional array intermediate states speeds up hashing functions by up to 40x.
