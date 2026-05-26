## 2024-05-26 - Prevent File Handle Leaks
 **Vulnerability:** Unclosed `FileStream` resource leaks in image hash calculation methods (e.g., `CalculateDifferenceHash64`, `CalculateAverageHash64`, etc.).
 **Learning:** Direct instantiation of IDisposable objects like `FileStream` within expression-bodied method arguments leads to resource leaks since they are never explicitly disposed, potentially causing denial of service or locking files.
 **Prevention:** Use `using` statements with block-bodied methods or `using var` declarations to ensure IDisposable objects are deterministically disposed after use.
