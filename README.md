<!-- mcp-name: io.github.baharmaulidiii/sharphy -->
<div align="center">

<img src="assets/sharphy.svg" alt="Sharphy Logo" width="120" />

# Sharphy: CLR Intelligence & Runtime Forensic Bridge

### Native Model Context Protocol (MCP) Server Empowering AI Coding Agents as Advanced .NET Runtime Diagnosticians, Memory Forensic Analysts, and Binary Architects.
**Leveraging ECMA-335 Metadata Semantics, Cross-Platform Crash Dump Telemetry, and Rigid CIL Bytecode Navigation.**

<p align="center">
  <a href="https://www.nuget.org/profiles/baharmaulidiii"><img src="https://img.shields.io/badge/nuget-v1.0.5-blue?logo=nuget&logoColor=white" alt="NuGet Package" /></a>
  <a href="https://www.nuget.org/profiles/baharmaulidiii"><img src="https://img.shields.io/badge/nuget-prefix%20reserved%20%E2%9C%94-004880?logo=nuget&logoColor=white" alt="NuGet Prefix Reserved" /></a>
  <a href="https://glama.ai/mcp/servers/baharmaulidiii/sharphy"><img src="https://glama.ai/mcp/servers/baharmaulidiii/sharphy/badges/score.svg" alt="Glama MCP Server" /></a>
  <a href="#"><img src="https://img.shields.io/badge/MCP-10%20tools%20catalog-8A2BE2" alt="MCP Catalog" /></a>
  <a href="https://dotnet.microsoft.com/download"><img src="https://img.shields.io/badge/8%20|%209%20|%2010-512BD4?logo=dotnet&logoColor=white" alt=".NET 8/9/10" /></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/license-Apache%202.0-blue" alt="Apache License 2.0" /></a>
  <a href="CODE_OF_CONDUCT.md"><img src="https://img.shields.io/badge/Contributor%20Covenant-2.1-4baaaa.svg" alt="Contributor Covenant" /></a>
  <a href="https://github.com/baharmaulidiii/sharphy"><img src="https://img.shields.io/badge/GitHub-baharmaulidiii%2Fsharphy-181717?logo=github" alt="GitHub Repo" /></a>
</p>

<p align="center">
  <a href="#architecture-overview--philosophy"><b>Architecture</b></a> ·
  <a href="#package-ecosystem--roles"><b>Ecosystem</b></a> ·
  <a href="#10-mcp-tools-catalog"><b>MCP Catalog</b></a> ·
  <a href="#installation--agent-integration"><b>Agent Setup</b></a> ·
  <a href="#community-security--legal-policies"><b>Policies & Legal</b></a> ·
  <a href="CHANGELOG.md"><b>Changelog</b></a> ·
  <a href="CONTRIBUTING.md"><b>Contributing</b></a> ·
  <a href="CODE_OF_CONDUCT.md"><b>Code of Conduct</b></a>
</p>

</div>

> [!CAUTION]
> **Legal Disclaimer & Limitation of Liability**
> Sharphy is engineered exclusively for good-faith security research, production crash dump diagnostics, vulnerability assessments, incident response, and lawful software engineering. Users bear sole civil and criminal responsibility for complying with applicable territorial cyber laws, third-party End User License Agreements (EULAs), and intellectual property rights. The developer disclaims any liability for malicious misuse or unlawful reverse engineering. Full details: [DISCLAIMER.md](DISCLAIMER.md).

> [!IMPORTANT]
> Install and execute Sharphy only from official distribution channels: verified NuGet packages (`Sharphy.Cli` under the verified `Sharphy.*` reserved prefix) or direct compilation from the official repository at [github.com/baharmaulidiii/sharphy](https://github.com/baharmaulidiii/sharphy).

---

## Architecture Overview & Philosophy

Mainstream AI coding agents (*Cursor*, *Claude Code*, *Windsurf*, *Copilot*) operate primarily on raw source code heuristics in an editor. They suffer from runtime blindness when confronting:
- Production memory crashes and unhandled fatal exceptions
- Multi-threaded synchronization deadlocks and lock cycle contention
- Large Object Heap (LOH) fragmentation and memory leaks
- Compiled third-party binaries lacking source code availability

**Sharphy** serves as a deterministic runtime forensic bridge implementing the open **Model Context Protocol (MCP)** specification. Over an isolated JSON-RPC 2.0 Stdio channel, Sharphy exposes Microsoft low-level diagnostics and decompilation engines directly to semantic AI reasoning loops.

```
┌─────────────────────────────────────────────────────────────┐
│                   AI Coding Agent Client                    │
│           (Cursor / Claude Code / Windsurf / LLM)           │
└──────────────────────────────┬──────────────────────────────┘
                               │ JSON-RPC 2.0 over Stdio
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                    Sharphy.Server (Host)                    │
│   ┌─────────────────────────────────────────────────────┐   │
│   │  Pure Stdio Channel (stdout: Payloads / stderr: Log)│   │
│   │  JSON-RPC Dispatcher & Tool Registry (10 Tools)     │   │
│   └──────────────────────────┬──────────────────────────┘   │
└──────────────────────────────┼──────────────────────────────┘
                               │ Strongly-Typed In-Process Calls
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                     Sharphy.Core Engine                     │
│  ┌───────────────────────┐       ┌───────────────────────┐  │
│  │   ClrMD Forensics     │       │     ILSpy Engine      │  │
│  │ (Crash Dump & Heap)   │       │ (C# Decompilation)    │  │
│  ├───────────────────────┤       ├───────────────────────┤  │
│  │   Mono.Cecil Engine   │       │   Roslyn & Sandbox    │  │
│  │ (Metadata & Mutation) │       │ (UnsafeAccessor & ALC)│  │
│  └───────────────────────┘       └───────────────────────┘  │
└──────────────────────────────┬──────────────────────────────┘
                               │ Deterministic Low-Level Inspection
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                  Runtime Analysis Target                    │
│  • Windows / Linux Crash Dump Files (.dmp / ELF core dump)  │
│  • Compiled Binary Assemblies (.dll / .exe ECMA-335)        │
│  • Active Runtime Processes & Isolated AssemblyLoadContext  │
└─────────────────────────────────────────────────────────────┘
```

---

## Package Ecosystem & Roles

The Sharphy solution is organized into three modular projects with distinct boundaries:

| Package / Module | Distribution | Target Audience | Architectural Role & Technical Scope |
| :--- | :--- | :--- | :--- |
| **`Sharphy.Cli`** | `.NET Global Tool` | End Users & Agents | Command-line entry point. Operates as an instant zero-configuration launcher for the MCP host server (`sharphy mcp start`) or quick inspection utilities. |
| **`Sharphy.Server`** | `Class Library / Host` | Agent Frameworks & Hosts | Implements the official Model Context Protocol (MCP) using JSON-RPC 2.0. Enforces pure Stdio stream separation: `stdout` is dedicated to JSON-RPC payloads, while all internal diagnostic logging is routed to `stderr`. |
| **`Sharphy.Core`** | `Class Library Engine` | CLR Engineers & Builders | Pure analytical engine with no MCP protocol dependencies. Abstacts low-level libraries: `Microsoft.Diagnostics.Runtime` (ClrMD), `ICSharpCode.Decompiler` (ILSpy), `Mono.Cecil`, and `Microsoft.CodeAnalysis.CSharp` (Roslyn). |

---

## Operational Architecture: Zero-Footprint Production Forensics

A common enterprise architectural question is whether diagnostic servers must be deployed to live production machines.

> **Does Sharphy need to be installed on your production servers? NO.**

Sharphy follows a **Zero-Footprint, Post-Mortem Forensic Workflow**. The analytical engines, decompilers, and AI agents run strictly in isolated development, CI/CD, or security analyst workstations:

```
┌──────────────────────────────────────────────┐
│         PRODUCTION ENVIRONMENT               │
│  (Zero Sharphy Footprint Required)           │
│                                              │
│  1. Unhandled crash / freeze occurs          │
│  2. Collect memory dump via official CLI:    │
│     dotnet-dump collect -p <PID>             │
│  3. Output: crash.dmp (Windows) / core (ELF) │
└──────────────────────┬───────────────────────┘
                       │ Secure SCP / S3 Dump Transfer
                       ▼
┌──────────────────────────────────────────────┐
│     ANALYST WORKSTATION / AI ENVIRONMENT     │
│  (Isolated Off-Site Inspection)              │
│                                              │
│  • AI Coding Agent (Cursor / Claude Code)    │
│  • Sharphy MCP Server (Sharphy.Cli / Server) │
│  • Automated RCA:                            │
│    - sharphy_analyze_dump                    │
│    - sharphy_inspect_heap                    │
│    - sharphy_deadlock_detector               │
└──────────────────────────────────────────────┘
```

### Key Operational Benefits:
1. **Zero Production Risk:** No third-party daemons, background agents, or persistent listeners are installed on mission-critical production servers.
2. **Standard Microsoft Tooling:** Dump collection uses standard, officially supported Microsoft utilities (`dotnet-dump`, `createdump`, or OS crash reporting).
3. **Regulatory & Audit Compliance:** Keeps production environments compliant with strict compliance standards (PCI-DSS, ISO/IEC 27001, SOC 2) by avoiding intrusive monitoring hooks.
4. **Air-Gapped & Offline Ready:** Analysis can be performed in completely isolated, offline lab environments without remote runtime connectivity.

---

## 10 MCP Tools Catalog

Sharphy exposes ten specialized tools covering the full spectrum of .NET runtime analysis:

```
┌────────────────────────────────────────────────────────────────────────────┐
│                     SHARPHY 10 MCP TOOLS CATALOG                           │
├────────────────────────┬─────────────────────────┬─────────────────────────┤
│ Memory & Forensics     │ Decompilation & Diff    │ Bytecode & Metadata     │
├────────────────────────┼─────────────────────────┼─────────────────────────┤
│ • sharphy_analyze_dump │ • sharphy_decompile_... │ • sharphy_inspect_me... │
│ • sharphy_inspect_heap │ • sharphy_diff_assem... │ • sharphy_extract_to... │
│ • sharphy_deadlock_... │ • sharphy_clean_obfu... │ • sharphy_patch_il      │
│                        │                         │ • sharphy_generate_w... │
└────────────────────────┴─────────────────────────┴─────────────────────────┘
```

### 1. `sharphy_analyze_dump`
Automated Root Cause Analysis (RCA) on Windows (`.dmp`) and Linux (`ELF core dump`) CoreCLR memory crash dumps. Extracts crashing thread call stacks, unhandled exception objects, and faulty HRESULT codes.

### 2. `sharphy_inspect_heap`
Deep managed heap inspection, traversing GC segments, measuring generation distribution (Gen 0, Gen 1, Gen 2, LOH, POH), calculating fragmentation, and pinpointing memory leak retainers.

### 3. `sharphy_deadlock_detector`
Traverses the CLR `SyncBlock` synchronization cache and active thread Monitor locks, building a Wait-For Graph (WFG) to detect thread synchronization deadlock cycles.

### 4. `sharphy_decompile_method`
Reconstructs clean, modern idiomatic C# source code from compiled CIL bytecode using the embedded ILSpy engine, reconstructing async state machines and lambda captures.

### 5. `sharphy_diff_assemblies`
Performs semantic binary comparisons between two versions of an assembly, flagging added/removed types, breaking changes to public/protected APIs, and CIL instruction size shifts.

### 6. `sharphy_inspect_metadata`
Inspects ECMA-335 metadata tables, decoding TypeDef, MethodDef, FieldDef, custom attributes, P/Invoke native imports, and embedded resources.

### 7. `sharphy_clean_obfuscation`
Analyzes obfuscated assemblies, demangling Control Flow Graph (CFG) switch-flattening patterns, trimming excessive NOP sleds, and identifying dynamic proxy delegates.

### 8. `sharphy_extract_tokens`
Extracts tokens and types by loading assemblies into an isolated, collectible `AssemblyLoadContext` (`isCollectible: true`) without locking files on disk or polluting the host process.

### 9. `sharphy_patch_il`
Surgically modifies CIL bytecode instructions by replacing target opcodes, adjusting branch offsets, and re-emitting patched binary assemblies.

### 10. `sharphy_generate_wrapper`
Generates zero-overhead, strongly-typed C# wrapper code leveraging `.NET 8+` `[UnsafeAccessor]` attributes to access private members and fields without reflection overhead.

---

## Installation & Agent Integration

### Quick Start
Install the global CLI tool via the .NET SDK:
```bash
dotnet tool install -g Sharphy.Cli
```

### Agent Configuration (MCP Setup)
Add Sharphy to your AI coding client configuration:

**Cursor (`~/.cursor/mcp.json`) or Claude Desktop (`claude_desktop_config.json`):**
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

---

## Multi-Targeting Architecture

Sharphy is engineered with native multi-targeting without emulation layers:
* **.NET 8.0 (LTS):** Long-term enterprise stability.
* **.NET 9.0 (STS):** Modern GC and performance optimizations.
* **.NET 10.0 (Latest):** Cutting-edge compiler APIs and runtime features.

---

## Community Standards, Security & Legal Policies

- Engineering standards and workflows in [CONTRIBUTING.md](CONTRIBUTING.md).
- Community code of conduct in [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md).
- Threat modeling and vulnerability reporting in [SECURITY.md](SECURITY.md).
- Accessibility commitments in [ACCESSIBILITY.md](ACCESSIBILITY.md).
- Trademark and brand guidelines in [TRADEMARK.md](TRADEMARK.md).
- Liability limitations in [DISCLAIMER.md](DISCLAIMER.md).
- Third-party attributions and licensing in [THIRD_PARTY_NOTICE.md](THIRD_PARTY_NOTICE.md).

## License

Sharphy is distributed under the **[Apache License, Version 2.0](LICENSE)**. Copyright (c) 2026 Moh. Baharudin Yuda Maulidi. All rights reserved.
