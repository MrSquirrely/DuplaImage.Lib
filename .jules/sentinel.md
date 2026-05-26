## 2024-05-26 - Fix Unclosed FileStreams
 **Vulnerability:** Unclosed FileStreams causing resource leaks in ImageHashes methods
 **Learning:** Passing `new FileStream(...)` directly to a method argument causes the stream to be leaked if the method does not dispose of it.
 **Prevention:** Use `using` statements (or `using var` declarations in C# 8.0+) to ensure deterministic disposal of resources like FileStream.
