<div align="center">

<p align="center">
  <a href="README.md">Bahasa Indonesia</a> · <b>English</b>
</p>

# Sharphy: CLR Intelligence & Runtime Forensic Bridge

### The native Model Context Protocol (MCP) server turning AI agents into deep .NET runtime diagnosticians, binary inspectors, and CIL architects.
**Harness rich ECMA-335 metadata, diagnose crash dumps directly from runtime memory, and navigate CIL bytecode with precision.**

<p align="center">
  <a href="https://www.nuget.org/packages/Sharphy.Cli"><img src="https://img.shields.io/badge/nuget-v0.1.0--preview-blue?logo=nuget&logoColor=white" alt="NuGet Package" /></a>
  <a href="#"><img src="https://img.shields.io/badge/MCP-tool%20catalog-8A2BE2" alt="MCP Catalog" /></a>
  <a href="https://dotnet.microsoft.com/download"><img src="https://img.shields.io/badge/.NET-8%20|%209%20|%2010-512BD4?logo=dotnet&logoColor=white" alt=".NET 8/9/10" /></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/license-MIT-green" alt="MIT License" /></a>
  <a href="https://github.com/baharmaulidiii/sharphy"><img src="https://img.shields.io/badge/GitHub-baharmaulidiii%2Fsharphy-181717?logo=github" alt="GitHub Repo" /></a>
</p>

<p align="center">
  <a href="#quick-start"><b>Quick Start</b></a> ·
  <a href="#package-ecosystem--benefits"><b>Packages & Benefits</b></a> ·
  <a href="#core-capabilities"><b>Core Capabilities</b></a> ·
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
> Sharphy is designed strictly for educational, security research, vulnerability auditing, incident investigation, and authorized binary diagnostics. Users are solely responsible for ensuring compliance with all local laws, End User License Agreements (EULAs), and intellectual property rights. The authors and maintainers assume no liability for misuse. See [DISCLAIMER.md](DISCLAIMER.md).

> [!IMPORTANT]
> Install and run Sharphy only from trusted sources: NuGet (`Sharphy.Cli`), or by building directly from source at [github.com/baharmaulidiii/sharphy](https://github.com/baharmaulidiii/sharphy).

---

## Package Ecosystem & Benefits

Sharphy is designed modularly into 3 distinct packages, each providing targeted benefits:

| NuGet Package | Target Type | Primary Role & Value Proposition |
| :--- | :--- | :--- |
| **`Sharphy.Cli`** | `.NET Global Tool` | **For Users & Terminal Agents:** Global executable command-line tool (`sharphy`) installed in a single command (`dotnet tool install -g Sharphy.Cli`). Serves as a zero-setup runner to launch the MCP server (`sharphy mcp start`) or inspect crash dumps and assemblies directly from any terminal. |
| **`Sharphy.Core`** | `Class Library` | **For .NET Engineers & Diagnostic Builders:** Core engine encapsulating low-level CLR diagnostics (ClrMD), C# decompiler engines (ILSpy), bytecode manipulation (Mono.Cecil/dnlib), and Roslyn compilers. Fully usable as a standalone library in any .NET application without MCP dependencies. |
| **`Sharphy.Server`** | `Class Library / Host` | **For Agent Frameworks & Runtime Hosts:** Standardized Model Context Protocol (MCP) server dispatcher implementing JSON-RPC 2.0 (Stdio & SSE). Bridges `Sharphy.Core` capabilities directly to LLM coding agents (Cursor, Claude Code, Windsurf, Copilot, etc.). |

---

Sharphy connects AI coding agents (Claude Code, Cursor, Windsurf, Copilot, Gemini) natively into .NET internals:

* **Memory Dump Forensics & CLR Telemetry:** Ingest crash dumps (`.dmp`) across Windows and Linux CoreCLR via ClrMD, detect thread deadlocks, analyze Large Object Heap (LOH) fragmentation, and pinpoint memory leaks.
* **Source Reconstruction & Metadata Inspection:** Parse ECMA-335 hierarchies, compiler-generated attributes, and P/Invoke signatures, reconstructing clean modern C# methods via the ILSpy decompiler engine.
* **Integrity & Unpacking Heuristics:** Detect CFG flattening patterns, resolve proxy delegates, and execute decryption wrappers safely in isolated `AssemblyLoadContext` sandboxes.
* **CIL Instruction Mutation & Dynamic Patching:** Mutate bytecode opcodes (nop-out checks, alter branch targets) and generate high-throughput wrappers via C# `UnsafeAccessor` or Roslyn.

---

## Quick Start

### 1. Install Global CLI Tool

```bash
dotnet tool install -g Sharphy.Cli
```

### 2. Configure Your AI Agent (Claude Code / Cursor / Windsurf)

Add Sharphy to your MCP client configuration (`mcpServers`):

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

Or run directly from source via `dotnet run`:

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
| `sharphy_analyze_dump` | Diagnostics | Performs automated Root Cause Analysis (RCA) on crash dump (`.dmp`) files. |
| `sharphy_inspect_heap` | Diagnostics | Managed heap audit: memory leaks, object retention, string deduplication, and LOH fragmentation. |
| `sharphy_deadlock_detector`| Diagnostics | Traces thread synchronization objects, monitors, and lock contention dependency chains. |
| `sharphy_decompile_method` | Reconstruction | Decompiles target CIL methods into clean, readable C# with compiler attributes. |
| `sharphy_diff_assemblies` | Metadata | Compares two assembly versions (`.dll`/`.exe`) to catch breaking API changes and hidden alterations. |
| `sharphy_inspect_metadata` | Metadata | Dumps ECMA-335 type hierarchies, native P/Invoke signatures, generic arguments, and embedded resources. |
| `sharphy_clean_obfuscation`| Integrity | Analyzes and demangles standard obfuscation patterns (CFG flattening, proxy delegates). |
| `sharphy_extract_tokens` | Integrity | Evaluates decryption routines inside an isolated `AssemblyLoadContext` sandbox. |
| `sharphy_patch_il` | Mutation | Patches CIL instructions directly and re-emits binaries without requiring original project source. |
| `sharphy_generate_wrapper` | Mutation | Automatically synthesizes strongly-typed C# wrappers via high-performance `UnsafeAccessor`. |

---

## Architecture & Multi-Targeting

Sharphy is engineered to target modern .NET runtime LTS and STS versions:
* **.NET 8.0 (LTS)**
* **.NET 9.0 (STS)**
* **.NET 10.0 (Preview / Latest)**

```text
sharphy/
├── src/
│   ├── Sharphy.Core/           # CLR Diagnostics, ILSpy, ClrMD, Roslyn, Cecil
│   ├── Sharphy.Server/         # MCP Protocol Layer (Stdio & SSE Transport)
│   └── Sharphy.Cli/            # Global CLI Launcher
├── samples/
│   └── CrackmeDemo/            # Testbed binary for agent verification
├── tests/
│   └── Sharphy.Tests/          # Unit & integration test suites
├── .github/workflows/          # CI matrix build & test pipeline
└── README.md
```

---

## FAQ

<details>
<summary><b>Does Sharphy require source code to inspect binaries?</b></summary>
No. Sharphy operates directly on compiled CLI binaries (.dll, .exe), memory dumps (.dmp), or live runtime processes.
</details>

<details>
<summary><b>Can I use Sharphy across Windows and Linux?</b></summary>
Yes. Sharphy is built on modern cross-platform .NET 8/9/10 and supports Windows dumps as well as Linux CoreCLR ELF dumps.
</details>

---

## Contributing & Security

- Contributions are welcome! Read [CONTRIBUTING.md](CONTRIBUTING.md) to get started.
- For security vulnerabilities, refer to [SECURITY.md](SECURITY.md).
- Detailed legal terms are outlined in [DISCLAIMER.md](DISCLAIMER.md).

## License

Sharphy is released under the [MIT License](LICENSE).
