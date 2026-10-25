# Security Policy & Threat Modeling

Security and host integrity are paramount in the engineering of **Sharphy**. Operating as a low-level runtime diagnostic bridge, Sharphy incorporates rigid defensive mechanisms against malicious code execution and process hijacking vectors.

---

## 1. Supported Versions & Security Update Lifecycle

Active security patch support is provided for the following release lines:

| Release Line | Status | Security Patch Policy |
| :--- | :--- | :--- |
| `1.0.x` (General Availability / Stable) | :white_check_mark: Supported | Active security patches for critical vulnerabilities, runtime updates, and MCP server stability. |
| `0.1.x` (Preview Releases) | :white_check_mark: Supported | Maintenance transition path towards stable releases. |
| `< 0.1.0` | :x: Deprecated | Unsupported. Users must upgrade to the stable `1.0.x` release line. |

---

## 2. Threat Modeling & Architectural Defenses

Sharphy defends against untrusted code and hostile execution vectors through proactive architectural boundaries:

### 2.1 Untrusted Binary Ingestion
- **Threat Vector:** Audited binary assemblies may contain malicious static constructors (`.cctor`) designed to execute hostile payloads when types are initialized via conventional reflection.
- **Defensive Layer:** Sharphy leverages static metadata parsing via `Mono.Cecil` without executing module constructors. When dynamic evaluation is required (e.g., in `sharphy_extract_tokens`), reflection runs strictly inside an isolated collectible `AssemblyLoadContext` (`isCollectible: true`) that is immediately unloaded upon completion.

### 2.2 Massive Crash Dumps & Denial of Service (DoS)
- **Threat Vector:** Memory crash dump files measuring tens of gigabytes could trigger Out of Memory (OOM) conditions if mapped entirely into host RAM.
- **Defensive Layer:** ClrMD forensics utilize lazy virtual memory mapping and selective segment traversal, limiting reads to specific data structures or segments actively queried by the agent.

### 2.3 Path Traversal & Protocol Channel Purity
- **Threat Vector:** Malicious path injection through JSON-RPC payloads seeking to read unauthorized disk files (*arbitrary file read / path traversal*).
- **Defensive Layer:** All absolute path arguments are normalized, sanitized, and validated prior to executing file system operations.

---

## 3. Reporting a Vulnerability

If you identify a security vulnerability, sandbox escape, or protocol weakness in Sharphy:

1. **DO NOT file a public issue or publish exploit demonstrations publicly.**
2. Report your findings privately through the official **GitHub Security Advisories** workflow:
   * Portal: [https://github.com/baharmaulidiii/sharphy/security/advisories/new](https://github.com/baharmaulidiii/sharphy/security/advisories/new)
3. Include the following details:
   - Technical summary and estimated impact.
   - Step-by-step reproduction guide.
   - Proof-of-concept (POC) minimal sample binary or MCP client config.
   - .NET SDK version, CPU architecture, and host Operating System.

Maintainers acknowledge reports within 48 business hours following standard Coordinated Vulnerability Disclosure (CVD) timelines.
