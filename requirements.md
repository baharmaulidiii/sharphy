**Sharphy** memiliki *market positioning* yang sangat solid karena ekosistem .NET memiliki metadata paling kaya (ECMA-335, CIL, Roslyn, SOS/CLRMD) namun belum ada agentic MCP tool yang mengeksploitasinya secara *first-class*.

Pilihan arsitektur dan kapabilitas inti untuk membangun **Sharphy** sebagai framework MCP di ekosistem .NET:

---

### Value Proposition Utama

Menjadikan AI Coding Agent bukan hanya sekadar pembaca teks kode, melainkan **Autonomous CLR Engineer** yang mampu membaca assembly (`.dll`/`.exe`), menganalisis memory dump (`.dmp`), merekonstruksi source code via Roslyn/ILSpy, dan mengoptimalkan performa low-level CIL secara runtime maupun static.

---

### 4 Fitur Pilar untuk MCP Tools

**1. Decompilation & Static Code Reconstruction**

* **Engine:** `ICSharpCode.Decompiler` (ILSpy engine) + `Mono.Cecil` / `System.Reflection.Metadata`.
* **MCP Tools:**
* `sharphy_decompile_method`: Mengekstrak method CIL tertentu langsung menjadi representasi C# bersih beserta atribut internal/compiler-generated.
* `sharphy_diff_assemblies`: Membandingkan dua assembly `.dll` (misal update patch versi) untuk melihat *breaking API changes* atau penambahan logika tersembunyi.
* `sharphy_inspect_metadata`: Dump type hierarchy, P/Invoke signatures, generic parameters, dan embedded resources.



**2. Deep CLR Diagnostics & Crash Dump Forensic**

* **Engine:** `Microsoft.Diagnostics.Runtime` (ClrMD).
* **MCP Tools:**
* `sharphy_analyze_dump`: Agent membaca Windows/Linux crash dump (`.dmp`) dan langsung menarik Root Cause Analysis (RCA).
* `sharphy_inspect_heap`: Menemukan memory leak, duplicate strings, pinvoke handles yang tidak terlepas, dan Large Object Heap (LOH) fragmentation.
* `sharphy_deadlock_detector`: Membaca lock chain antar Thread dan monitor lock yang sedang *hang*.



**3. Anti-Obfuscation & Integrity Unpacking**

* **Engine:** `dnlib` / `AsmResolver`.
* **MCP Tools:**
* `sharphy_clean_obfuscation`: Membantu agen membersihkan teknik obfuscation standar (ConfuserEx, string encryption routines, CFG flattening, proxy calls).
* `sharphy_extract_tokens`: Menelusuri string decryption wrapper dan memanggil decryptor secara isolated assembly loading (`AssemblyLoadContext`).



**4. Roslyn-Powered Live Mutation & Patching**

* **Engine:** `Microsoft.CodeAnalysis.CSharp` (Roslyn) + `0xd4d/dnlib`.
* **MCP Tools:**
* `sharphy_patch_il`: Mengganti instruksi CIL (misal `nop`-ing validation checks atau mengubah return value) dan mere-compile biner tanpa butuh source code asli.
* `sharphy_generate_wrapper`: Secara otomatis meng-generate source code C# strongly-typed wrapper untuk memanggil internal/private API target via high-performance reflection (`UnsafeAccessor` / Source Generators).



---

### Blueprint Struktur Repo

```text
sharphy/
├── src/
│   ├── Sharphy.Core/           # Logic bridge (ILSpy, ClrMD, Roslyn, Cecil)
│   ├── Sharphy.Server/         # MCP Server (Stdio & SSE protocol handlers)
│   └── Sharphy.Cli/            # Global CLI tool (dotnet tool install -g sharphy)
├── samples/
│   └── CrackmeDemo/            # Target testing binary
├── tests/
└── README.md

```

---

### Draf Konsep README (Hero Section)

> **Sharphy: The CLR-Native Reverse Engineering & Diagnostics Bridge for AI Agents.**
> Sharphy is an open-source Model Context Protocol (MCP) server that empowers LLM agents (Claude Code, Cursor, Copilot) to inspect, decompile, patch, and diagnose .NET binaries and runtime environments directly from plain natural language.

---