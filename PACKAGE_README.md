<!-- mcp-name: io.github.baharmaulidiii/sharphy -->
# Sharphy: CLR Intelligence & Runtime Forensic Bridge

Sharphy is a native **Model Context Protocol (MCP)** server that equips AI Coding Agents (Cursor, Claude Code, Windsurf, Copilot, Antigravity) with deep .NET Common Language Runtime (CLR) diagnostics, memory forensics, and binary reverse-engineering capabilities.

---

## Why Sharphy?

Standard AI coding assistants only inspect raw source code text in an editor. They suffer from runtime blindness when encountering:
- Production crash dumps and unhandled exceptions
- Thread deadlocks and synchronization bottlenecks
- Memory leaks and Large Object Heap (LOH) fragmentation
- Compiled third-party assemblies without source code

**Sharphy bridges this gap** by exposing low-level CLR runtime diagnostic engines directly to semantic AI agents over an isolated JSON-RPC 2.0 Stdio channel.

---

## Key Features

- **CIL to C# Decompilation:** Reconstruct modern C# syntax from raw CIL bytecode using the embedded ILSpy engine (`sharphy_decompile_method`).
- **Memory & Crash Dump Forensics:** Inspect managed heaps, analyze full-memory dumps, and detect thread deadlocks via Microsoft ClrMD (`sharphy_analyze_dump`, `sharphy_inspect_heap`, `sharphy_deadlock_detector`).
- **ECMA-335 Metadata Inspection:** Query types, methods, fields, and assembly tokens with Mono.Cecil (`sharphy_inspect_metadata`, `sharphy_extract_tokens`).
- **CIL Instruction Patching:** Inspect, modify, or inject bytecode instructions into existing assemblies (`sharphy_patch_il`).
- **Zero-Overhead Wrappers:** Automatically synthesize high-performance accessors for private members using `.NET 8+` `[UnsafeAccessor]` (`sharphy_generate_wrapper`).
- **Safe Sandboxing:** Execute dynamic inspections within collectible `AssemblyLoadContext` without locking files on disk.

---

## Quick Start & Installation

Install the global CLI tool via the .NET SDK:

```bash
dotnet tool install -g Sharphy.Cli
```

Verify your installation:

```bash
sharphy --help
```

---

## Integrating with AI Coding Agents (MCP Setup)

To use Sharphy with **Cursor**, **Claude Code**, **Windsurf**, or any other MCP-compatible client, add it to your agent configuration:

### Cursor (`~/.cursor/mcp.json`) or Claude Desktop (`claude_desktop_config.json`):

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

Once connected, your AI assistant will automatically discover Sharphy's catalog of 10 specialized runtime inspection tools.

---

## Ecosystem Packages

- **Sharphy.Cli**: Global .NET CLI executable and self-contained MCP server runner (recommended for all end users).
- **Sharphy.Server**: Core MCP protocol dispatchers and JSON-RPC 2.0 Stdio/SSE handlers.
- **Sharphy.Core**: Low-level diagnostic, decompilation, and metadata mutation engines.

---

## Links & Community

- **Repository**: [https://github.com/baharmaulidiii/sharphy](https://github.com/baharmaulidiii/sharphy)
- **Documentation**: [https://baharmaulidiii.github.io/sharphy](https://baharmaulidiii.github.io/sharphy)
- **License**: Apache-2.0 (Copyright 2026 Moh. Baharudin Yuda Maulidi. All rights reserved.)
