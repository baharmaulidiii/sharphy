# Sharphy Community Launch & Announcement Kit

This document provides ready-to-deploy announcements, discussion drafts, and copy for introducing **Sharphy** to software engineering communities, cybersecurity researchers, and AI coding agent ecosystems.

---

## 1. GitHub Repository Metadata

Access repository settings at https://github.com/baharmaulidiii/sharphy -> **About** section:

* **Description:**
  ```text
  CLR Intelligence & Runtime Forensic Bridge for AI Coding Agents (Native Model Context Protocol / MCP)
  ```
* **Website:**
  ```text
  https://baharmaulidiii.github.io/sharphy/
  ```
* **Topics:**
  `mcp` `model-context-protocol` `dotnet` `csharp` `clr` `reverse-engineering` `forensics` `memory-dump` `decompiler` `ilspy` `clrmd` `roslyn` `ai-agent` `claude-code` `cursor`

---

## 2. Reddit Draft (`r/dotnet`)

* **Target Community:** `r/dotnet`
* **Title:**
  ```text
  Sharphy: Native MCP Server Turning AI Coding Agents into Autonomous CLR Runtime Engineers
  ```
* **Post Body:**
  ```markdown
  Hello .NET Community! 👋

  While utilizing AI coding agents (Claude Code, Cursor, Windsurf, Copilot), a major engineering hurdle emerges: agents operate solely on source text heuristics and remain blind to compiled CLR runtime internals. They cannot diagnose crash dumps, inspect managed heaps, or evaluate bytecode without tedious external manual steps.

  To bridge this gap deterministically, I developed **Sharphy**: an open-source, native Model Context Protocol (MCP) server designed to connect AI coding agents directly to .NET runtime internals.

  ### ⚡ Key Capabilities:
  1. **Memory Dump Forensics:** Automated Root Cause Analysis (RCA) on Windows and Linux CoreCLR `.dmp` files via Microsoft ClrMD.
  2. **Heap & Deadlock Diagnostics:** Traversal of GC segments, LOH fragmentation analysis, and thread synchronization Wait-For Graphs.
  3. **High-Fidelity Decompilation:** Reconstructing clean, modern C# methods from compiled assemblies via the ILSpy decompiler engine.
  4. **Bytecode Mutation & Sandboxing:** Surgical CIL instruction patching and isolated unpacking inside collectible `AssemblyLoadContext` sandboxes.
  5. **Pure Stdio Transport:** Standard JSON-RPC 2.0 communication with zero overhead.

  📦 **GitHub:** https://github.com/baharmaulidiii/sharphy  
  🌐 **Documentation:** https://baharmaulidiii.github.io/sharphy/

  Looking forward to your technical feedback, architectural critiques, and thoughts!
  ```

---

## 3. Hacker News Draft (`Show HN`)

* **Title:**
  ```text
  Show HN: Sharphy – Native MCP Server Giving AI Agents Deep .NET CLR Diagnostics
  ```
* **Body:**
  ```markdown
  Hi HN,

  I built Sharphy, a native Model Context Protocol (MCP) server for the .NET runtime.

  Most AI coding assistants only read source files. Sharphy gives LLMs direct observability into the Common Language Runtime (CLR):
  - Analyzing Windows/Linux crash dumps with ClrMD
  - Traversing managed GC heaps and detecting deadlocks
  - Decompiling CIL into modern C# using the ILSpy engine
  - Safe dynamic assembly sandboxing via collectible AssemblyLoadContexts

  Source: https://github.com/baharmaulidiii/sharphy
  License: Apache-2.0
  Docs: https://baharmaulidiii.github.io/sharphy/

  Feedback welcome!
  ```
