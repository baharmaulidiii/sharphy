# Panduan Publikasi & Rilis Komunitas Sharphy (Launch Kit)

Dokumen ini menyediakan materi komunikasi resmi dan format publikasi siap pakai (*ready-to-deploy announcements*) untuk memperkenalkan **Sharphy** kepada komunitas perekayasa perangkat lunak, peneliti keamanan, dan ekosistem AI coding agent global.

---

## 1. Konfigurasi Metadata Repositori GitHub

Akses pengaturan repositori pada tautan https://github.com/baharmaulidiii/sharphy -> panel **About**:

* **Deskripsi Singkat (*Description*):**
  ```text
  CLR Intelligence & Runtime Forensic Bridge for AI Coding Agents (Native Model Context Protocol / MCP)
  ```
* **Tautan Dokumentasi (*Website*):**
  ```text
  https://baharmaulidiii.github.io/sharphy/
  ```
* **Label Topik (*Topics*):**
  `mcp` `model-context-protocol` `dotnet` `csharp` `clr` `reverse-engineering` `forensics` `memory-dump` `decompiler` `ilspy` `clrmd` `roslyn` `ai-agent` `claude-code` `cursor`

---

## 2. Draf Komunikasi Reddit (`r/dotnet`)

* **Target Komunitas:** `r/dotnet`
* **Judul Postingan (*Title*):**
  ```text
  Sharphy: Native MCP Server Turning AI Coding Agents into Autonomous CLR Runtime Engineers
  ```
* **Isi Postingan (*Body*):**
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

## 3. Draf Komunikasi Komunitas AI (`r/ClaudeAI` & `r/LocalLLaMA`)

* **Target Komunitas:** `r/ClaudeAI` / `r/LocalLLaMA`
* **Judul Postingan (*Title*):**
  ```text
  Empowering Claude Code and Cursor with Native .NET CLR Reverse Engineering & Memory Forensics (Sharphy MCP)
  ```
* **Isi Postingan (*Body*):**
  ```markdown
  Hi everyone,

  Most MCP tools focus on web browsing, documentation scraping, or database integrations. I built an MCP server focused entirely on deep, low-level systems engineering for the .NET runtime ecosystem.

  **Sharphy** gives your agent direct observability over compiled assemblies and runtime memory dumps:
  - Perform Root Cause Analysis (RCA) on fatal application crash dumps.
  - Inspect managed heap object retention and detect memory leaks.
  - Decompile method bytecode directly into idiomatic C#.
  - Safely extract tokens and decrypt logic inside memory-isolated sandboxes.

  Get started instantly via NuGet or source:
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

  Check out the repository: https://github.com/baharmaulidiii/sharphy
  ```

---

## 4. Pengajuan Hacker News (Show HN)

* **Tautan Pengajuan:** https://news.ycombinator.com/submit
* **Format Judul (*Title*):**
  ```text
  Show HN: Sharphy – CLR runtime intelligence and memory forensics bridge for AI agents
  ```
* **Tautan Sasaran (*URL*):**
  ```text
  https://github.com/baharmaulidiii/sharphy
  ```

---

## 5. Pendaftaran Kurasi Awesome MCP Servers

Ajukan Pull Request ke repositori kurasi MCP global:
* **Repositori Sasaran:** `punkpeye/awesome-mcp-servers` (https://github.com/punkpeye/awesome-mcp-servers)
* **Kategori Penempatan:** `Development & Debugging Tools`
* **Teks Entri PR:**
  ```markdown
  - [Sharphy](https://github.com/baharmaulidiii/sharphy) - Native Model Context Protocol (MCP) server providing deep CLR runtime intelligence, memory crash dump forensics, and CIL decompilation for AI coding agents.
  ```

---

<details>
<summary><b>🇬🇧 English Translation (Click to expand)</b></summary>

# Sharphy Launch & Community Promotion Kit

This document provides official promotional materials, publication templates, and community submission drafts to introduce **Sharphy** across developer forums, Reddit, Hacker News, and awesome-mcp catalogs.

*(Refer to complete templates and submission copy above).*

</details>
