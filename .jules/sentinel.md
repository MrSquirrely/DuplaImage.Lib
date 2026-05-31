
## 2024-05-31 - [Secure ImageMagick Configuration]
 **Vulnerability:** Unrestricted ImageMagick configuration can lead to SSRF, XXE, or other file-read vulnerabilities when parsing user-provided files due to various legacy and complex delegates and coders.
 **Learning:** ImageMagick's delegates such as HTTP, HTTPS, MVG, MSL, EPHEMERAL, and URL can fetch remote content, or files from the disk when parsing an image.
 **Prevention:** Use `ConfigurationFiles.Default.Policy.Data` to define a strict `<policymap>` disabling unwanted coders and paths before initializing `MagickNET`.
