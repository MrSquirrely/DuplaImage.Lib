## 2024-05-26 - [Unclosed FileStream in Hashing Methods]
 **Vulnerability:** Unclosed FileStream instances in various hash calculation methods created a resource leak.
 **Learning:** Instantiating `IDisposable` resources directly in method arguments (e.g., `Method(new FileStream(...))`) without assigning them to a variable or using a `using` block prevents proper disposal and leads to file handle leaks.
 **Prevention:** Always assign `IDisposable` instances to a variable using a `using` block or the `using var` declaration to ensure proper cleanup, especially when handling files.
