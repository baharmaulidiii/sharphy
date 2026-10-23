# Sharphy Launch & Community Promotion Kit

Dokumen ini berisi materi siap pakai (copy-paste) untuk mempublikasikan dan memviralkan **Sharphy** ke berbagai komunitas developer global.

---

## 1. Pengaturan Repository GitHub (About Section)

Buka: https://github.com/baharmaulidiii/sharphy -> Klik tombol ⚙️ di samping panel **About**:

- **Description:**
  ```text
  CLR Intelligence & Runtime Forensic Bridge for AI Coding Agents (Model Context Protocol / MCP)
  ```
- **Website:**
  ```text
  https://baharmaulidiii.github.io/sharphy/
  ```
- **Topics (Ketik satu per satu lalu Enter):**
  `mcp` `model-context-protocol` `dotnet` `csharp` `clr` `reverse-engineering` `forensics` `memory-dump` `decompiler` `ilspy` `clrmd` `roslyn` `ai-agent` `claude-code` `cursor`

---

## 2. Reddit Post (`r/dotnet`)

- **Subreddit:** `r/dotnet`
- **Post Title:**
  ```text
  Show r/dotnet: Sharphy – Turn AI coding agents into CLR engineers (MCP server for deep runtime diagnostics, crash dumps, and CIL decompilation)
  ```
- **Post Content:**
  ```markdown
  Hey r/dotnet! 👋

  While working with LLM coding agents (Cursor, Claude Code, Copilot), I noticed they are largely blind to compiled .NET internals—they only see raw text in the editor, but cannot inspect memory dumps, decompile external assemblies, or diagnose CLR runtime issues directly.

  To solve this, I built **Sharphy**: an open-source native Model Context Protocol (MCP) server designed to bridge AI coding agents directly with the CLR:

  ⚡ **Key Capabilities:**
  - **Memory Dump Forensics:** Ingest Windows & Linux CoreCLR `.dmp` files via ClrMD, detect thread deadlocks, analyze managed heaps, and trace LOH fragmentation.
  - **CIL Decompilation & Metadata:** Reconstruct clean modern C# methods and inspect ECMA-335 metadata using the ILSpy engine.
  - **Isolated Unpacking:** Run decryption routines in collectible `AssemblyLoadContext` sandboxes.
  - **CIL Instruction Mutation:** Safely patch opcodes and generate high-throughput wrappers via `UnsafeAccessor`.
  - **Multi-Targeting:** Native support across .NET 8, 9, and 10.

  📦 GitHub: https://github.com/baharmaulidiii/sharphy  
  🌐 Docs: https://baharmaulidiii.github.io/sharphy/

  Would love to hear feedback and thoughts from the .NET community!
  ```

---

## 3. Reddit Post (`r/ClaudeAI` & `r/LocalLLaMA`)

- **Subreddit:** `r/ClaudeAI` / `r/LocalLLaMA`
- **Post Title:**
  ```text
  I built an MCP server that gives Claude and Cursor native CLR / .NET reverse engineering and crash dump forensic powers
  ```
- **Post Content:**
  ```markdown
  Hi all,

  Most MCP tools focus on web search or database queries. I wanted to give AI coding agents deep low-level systems engineering abilities, specifically for the .NET CLR ecosystem.

  Meet **Sharphy**:
  It turns your Claude Code, Cursor, or Windsurf agent into an autonomous CLR engineer that can:
  - Read `.dmp` crash dumps and perform automated Root Cause Analysis.
  - Decompile method bytecode directly from `.dll` binaries.
  - Trace thread lock deadlocks and memory leaks.
  - Inspect ECMA-335 metadata hierarchies.

  You can install it as a global dotnet tool or run it directly:
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

  Check out the repo here: https://github.com/baharmaulidiii/sharphy  
  Feedback and stars are warmly welcome!
  ```

---

## 4. Hacker News Submission (Show HN)

- **URL:** https://news.ycombinator.com/submit
- **Title:**
  ```text
  Show HN: Sharphy – CLR intelligence and memory forensics bridge for AI agents
  ```
- **URL / Text:**
  ```text
  https://github.com/baharmaulidiii/sharphy
  ```

---

## 5. Submission Pull Request ke "Awesome MCP Servers"

Daftarkan Sharphy ke repositori kurasi MCP terbesar agar ribuan developer menemukannya:

1. **punkpeye/awesome-mcp-servers** (https://github.com/punkpeye/awesome-mcp-servers)
   - Edit section `Development & Debugging Tools`:
   ```markdown
   - [Sharphy](https://github.com/baharmaulidiii/sharphy) - CLR intelligence, runtime memory forensics, and CIL decompilation bridge for .NET binaries.
   ```
2. **appcypher/awesome-mcp-servers** (https://github.com/appcypher/awesome-mcp-servers)
   - Tambahkan entri di kategori `Developer Tools` / `Diagnostics`.

---

## 6. X (Twitter) & LinkedIn Launch Thread

```text
🚀 Excited to launch Sharphy: A native Model Context Protocol (MCP) server turning AI coding agents into deep .NET CLR diagnosticians and reverse engineers!

🔍 What it does:
• Crash dump (.dmp) forensics via ClrMD
• CIL method decompilation via ILSpy
• Heap leak & deadlock detection
• Multi-target (.NET 8/9/10)

Try it out: https://github.com/baharmaulidiii/sharphy
Docs: https://baharmaulidiii.github.io/sharphy/

#dotnet #csharp #mcp #ai #claude #cursor #opensource
```
