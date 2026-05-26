## 2024-05-26 - Span<T> Allocation over List<T> for Hashing
**Learning:** In C#, replacing `List<T>` with `stackalloc Span<T>` for small, fixed-size pixel arrays in hot paths (like hashing algorithms) provides a substantial performance boost by eliminating heap allocations and GC pressure.
**Action:** When finding medians or processing small, fixed-size data within inner loops, default to using stack-allocated Spans instead of List or Array instantiations.
