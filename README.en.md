<div align="center">

<p align="center">
  <a href="README.md">Bahasa Indonesia</a> · <b>English</b>
</p>

# Sharphy: Reverse Engineer .NET

### One MCP for reverse engineering across .NET assemblies, runtime memory, and CIL bytecode.
**Inspect a type or method you like. Understand how it works, down to the IL & native runtime level.**

<p align="center">
  <a href="https://www.nuget.org/packages/Sharphy.Cli"><img src="https://img.shields.io/badge/nuget-v0.1.0--preview-blue?logo=nuget&logoColor=white" alt="NuGet Package" /></a>
  <a href="#"><img src="https://img.shields.io/badge/MCP-tool%20catalog-8A2BE2" alt="MCP Catalog" /></a>
  <a href="https://dotnet.microsoft.com/download"><img src="https://img.shields.io/badge/.NET-8%20|%209%20|%2010-512BD4?logo=dotnet&logoColor=white" alt=".NET 8/9/10" /></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/license-MIT-green" alt="MIT License" /></a>
  <a href="https://github.com/baharmaulidiii/sharphy"><img src="https://img.shields.io/badge/GitHub-baharmaulidiii%2Fsharphy-181717?logo=github" alt="GitHub Repo" /></a>
</p>

<p align="center">
  <a href="#quick-start"><b>Quick Start</b></a> ·
  <a href="#how-sharphy-works"><b>How Sharphy Works</b></a> ·
  <a href="#what-you-can-analyze"><b>What You Can Analyze</b></a> ·
  <a href="#mcp-tools-catalog"><b>MCP Tools Catalog</b></a> ·
  <a href="#faq"><b>FAQ</b></a> ·
  <a href="CONTRIBUTING.md"><b>Contributing</b></a>
</p>

```bash
dotnet tool install -g Sharphy.Cli
```

</div>

> [!CAUTION]
> **Disclaimer & Legal Notice**
> Sharphy is designed strictly for educational, security research, vulnerability auditing, debugging, and authorized reverse engineering. Users are solely responsible for ensuring compliance with all local laws, End User License Agreements (EULAs), and intellectual property rights. The authors and maintainers assume no liability for misuse. See [DISCLAIMER.md](DISCLAIMER.md) for full terms.

> [!IMPORTANT]
> Install and run Sharphy only from official channels: NuGet (`Sharphy.Cli`), or building directly from source at [github.com/baharmaulidiii/sharphy](https://github.com/baharmaulidiii/sharphy).

---

## What You Can Analyze

Sharphy turns any MCP-enabled AI coding agent (Cursor, Claude Code, Windsurf, Copilot, Gemini) into an **Autonomous CLR Engineer**:

* **Decompilation & Source Reconstruction:** Decompile assemblies, types, or nested methods into modern readable C# using ILSpy and ECMA-335 metadata parsing.
* **CLR Crash Dump Forensics:** Load Windows/Linux memory dumps (`.dmp`), detect deadlocks, analyze threads, and track heap memory leaks.
* **Anti-Obfuscation Unpacking:** Analyze Control Flow Graph (CFG) flattening, decrypt hidden strings, and extract tokens in isolated `AssemblyLoadContext`.
* **CIL Instruction Mutation & Patching:** Safely replace CIL opcodes (`nop`, return values, branch targets) and re-emit assemblies without original source code.

---

## Quick Start

### 1. Install Global CLI Tool

```bash
dotnet tool install -g Sharphy.Cli
```

### 2. Configure Your AI Agent (Claude Code / Cursor / Windsurf)

Add Sharphy to your agent's MCP configuration (`mcpServers`):

```json
{
  "mcpServers": {
    "sharphy": {
      "command": "sharphy",
      "args": ["mcp", "start"]
    }
  }
}
```

Or run directly via `dotnet run`:

```json
{
  "mcpServers": {
    "sharphy": {
      "command": "dotnet",
      "args": ["run", "--project", "D:/Programming/GitHub/sharphy/src/Sharphy.Cli/Sharphy.Cli.csproj", "--", "mcp", "start"]
    }
  }
}
```

---

## MCP Tools Catalog

| Tool Name | Category | Description |
| :--- | :--- | :--- |
| `sharphy_decompile_method` | Decompiler | Extracts clean, decompiled C# representation of a specific method with compiler-generated attributes. |
| `sharphy_diff_assemblies` | Decompiler | Compares two `.dll`/`.exe` versions to identify breaking API changes and hidden alterations. |
| `sharphy_inspect_metadata` | Metadata | Dumps ECMA-335 type hierarchies, P/Invoke signatures, attributes, and embedded resources. |
| `sharphy_analyze_dump` | Diagnostics | Performs automated Root Cause Analysis (RCA) on crash dumps (`.dmp`). |
| `sharphy_inspect_heap` | Diagnostics | Inspects managed heap, object retention, string duplicates, and Large Object Heap (LOH). |
| `sharphy_deadlock_detector`| Diagnostics | Traces thread synchronization objects, monitors, and lock dependency chains. |
| `sharphy_clean_obfuscation`| Unpacker | Cleans basic obfuscation techniques (CFG flattening, proxy delegates, string decrypters). |
| `sharphy_extract_tokens` | Unpacker | Evaluates decryption routines within an isolated `AssemblyLoadContext`. |
| `sharphy_patch_il` | Patching | Direct CIL instruction patcher and assembler without needing original project files. |
| `sharphy_generate_wrapper` | Interop | Generates high-performance strongly-typed wrappers via C# `UnsafeAccessor`. |

---

## Architecture & Multi-Targeting

Sharphy is engineered to target modern .NET runtimes:
* **.NET 8.0 (LTS)**
* **.NET 9.0 (STS)**
* **.NET 10.0 (Preview / Latest)**

```text
sharphy/
├── src/
│   ├── Sharphy.Core/           # Core engine (ILSpy, ClrMD, Roslyn, Cecil, dnlib)
│   ├── Sharphy.Server/         # MCP protocol dispatcher (Stdio & SSE)
│   └── Sharphy.Cli/            # CLI launcher (dotnet tool)
├── samples/
│   └── CrackmeDemo/            # Testbed target binary
├── tests/
│   └── Sharphy.Tests/          # Multi-framework unit tests
├── .github/workflows/          # CI matrix workflows (.NET 8/9/10)
└── README.md
```

---

## FAQ

<details>
<summary><b>Does Sharphy require source code?</b></summary>
No. Sharphy operates directly on compiled CLI binaries (<code>.dll</code>, <code>.exe</code>), memory dumps (<code>.dmp</code>), or runtime memory.
</details>

<details>
<summary><b>Can I use Sharphy across Windows and Linux?</b></summary>
Yes. Sharphy is built on cross-platform .NET (8/9/10) and supports both Windows and Linux memory dumps and assemblies.
</details>

---

## Contributing & Security

- Contributions are welcome! Read [CONTRIBUTING.md](CONTRIBUTING.md) to get started.
- For security vulnerabilities, refer to [SECURITY.md](SECURITY.md).
- Detailed legal terms are outlined in [DISCLAIMER.md](DISCLAIMER.md).

## License

Sharphy is released under the [MIT License](LICENSE).
