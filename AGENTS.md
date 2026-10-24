# AGENTS.md - AI Coding Agent Operational Blueprint for Sharphy

> **Sharphy: CLR Intelligence & Runtime Forensic Bridge**  
> A native Model Context Protocol (MCP) server turning AI coding agents into deep .NET runtime diagnosticians, memory forensic analysts, and binary architects.

This document serves as the official operational instructions for AI Coding Agents (Cursor, Claude Code, Windsurf, Copilot, Antigravity) working within the **Sharphy** repository.

---

## 1. Core Principles & Agent Role

Every Agent operating in this repository acts as an **Autonomous CLR Engineer**:
1. **Metadata Preservation & CIL Standards:** Prioritize ECMA-335 metadata, low-level CIL instructions, SOS/ClrMD runtime inspection, and Roslyn compiler APIs over generic text heuristics.
2. **Multi-Targeting Awareness:** All code within `src/Sharphy.Core`, `src/Sharphy.Server`, and `src/Sharphy.Cli` must support multi-targeting across `.NET 8.0`, `.NET 9.0`, and `.NET 10.0`. Use conditional compilation preprocessor directives (`#if NET10_0_OR_GREATER`, `#if NET9_0_OR_GREATER`) whenever leveraging newer BCL/CLR features.
3. **Safety & Sandboxing:** Binary unpacking and dynamic assembly inspection must execute within an isolated, collectible `AssemblyLoadContext` (`isCollectible: true`) to avoid polluting the host runtime process or locking files on disk.
4. **Single-File Unified Documentation:** Maintain single authoritative Markdown files per topic. Bahasa Indonesia is the default primary language in the main document body, with English translations integrated cleanly in collapsible `<details>` sections. Do not create separate `.en.md` files.
5. **Absolute Git Commit & Push Prohibition:** Agents are strictly forbidden from executing `git commit` or `git push` under any circumstances. Only the repository owner/human user is authorized to perform git commit and push operations. Agents must leave all modifications uncommitted in the local working directory.

---

## 2. Repository Structure & Boundary Responsibilities

```text
sharphy/
├── .agents/                    # Agent workflow definitions & skill guidelines
├── src/
│   ├── Sharphy.Core/           # Core analysis engines:
│   │                           # - Diagnostics (ClrMD, SOS)
│   │                           # - Decompilation (ILSpy engine)
│   │                           # - Bytecode & Metadata (Mono.Cecil, dnlib, Roslyn)
│   ├── Sharphy.Server/         # Model Context Protocol (MCP) handlers:
│   │                           # - Stdio / SSE Transport
│   │                           # - Tool & Resource dispatchers
│   └── Sharphy.Cli/            # CLI executable & global dotnet tool wrapper
├── samples/
│   └── CrackmeDemo/            # Isolated testbed binary for feature validation
├── tests/
│   └── Sharphy.Tests/          # Multi-target unit and integration testing suite
├── Directory.Build.props       # Global properties, nullable checks, target frameworks
└── Sharphy.sln                 # Root solution file
```

---

## 3. Agent Development Workflow

```
[Agent Trigger]
       │
       ▼
1. Research & Inspect  ───> Review Directory.Build.props & project architecture
       │
       ▼
2. Implementation      ───> Write code in Sharphy.Core/Server with isolated context
       │
       ▼
3. Multi-Target Verify ───> Run: dotnet build Sharphy.sln
       │
       ▼
4. Test Suite          ───> Run: dotnet test Sharphy.sln
       │
       ▼
5. Docs & Clean Git    ───> Ensure README / docs remain synchronized bilingually
```

---

## 4. Standard CLI Commands for Agents

Agents should use the following commands during iteration cycles:

```powershell
# Restore dependencies
dotnet restore Sharphy.sln

# Build solution in Debug configuration
dotnet build Sharphy.sln -c Debug

# Execute test suite across all targeted frameworks
dotnet test Sharphy.sln --no-build

# Run local CLI tool
dotnet run --project src/Sharphy.Cli/Sharphy.Cli.csproj -- mcp start
```

---

## 5. The `.agents/` Directory

The [.agents](.agents) directory contains automated workflow rules and tool schemas:
- `.agents/rules.md`: Coding standards, memory safety, and error handling policies.
- `.agents/mcp-schema.json`: Formal JSON schema definitions for MCP tools implemented by `Sharphy.Server`.
- `.agents/testing-policy.md`: Testing guidelines, memory dump validation, and testbed procedures.
