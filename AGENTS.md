# AGENT.md - AI Coding Agent Operational Blueprint for Sharphy

> **Sharphy: CLR Intelligence & Runtime Forensic Bridge**  
> Server Model Context Protocol (MCP) native untuk mentransformasi AI coding agent menjadi diagnostisi mendalam, analis forensik memori, dan arsitek biner .NET.

Dokumen ini adalah instruksi operasional resmi bagi AI Coding Agent (Cursor, Claude Code, Windsurf, Copilot, Antigravity) yang bekerja dalam repositori **Sharphy**.

---

## 1. Peran & Prinsip Inti Agent

Setiap Agent yang beroperasi di repo ini harus bertindak sebagai **Autonomous CLR Engineer**:
1. **Preservasi Metadata & Standar CIL:** Utamakan pemanfaatan ECMA-335 metadata, IL low-level, SOS/ClrMD, dan Roslyn compiler API dibanding pendekatan teks heuristik biasa.
2. **Multi-Targeting Awareness:** Seluruh kode di `src/Sharphy.Core`, `src/Sharphy.Server`, dan `src/Sharphy.Cli` wajib kompatibel dengan multi-target `.NET 8.0`, `.NET 9.0`, dan `.NET 10.0`. Gunakan preprocessor directives (`#if NET10_0_OR_GREATER`, `#if NET9_0_OR_GREATER`) jika memanfaatkan fitur BCL/CLR terbaru.
3. **Safety & Sandboxing:** Modul unpacking atau eksekusi dinamis biner harus berjalan dalam `AssemblyLoadContext` terisolasi dan kolektibel (`isCollectible: true`) agar tidak mencemari proses induk.
4. **Bilingual Documentation:** Pertahankan konvensi dokumentasi bilingual (`.md` berbahasa Indonesia sebagai default, `.en.md` sebagai terjemahan bahasa Inggris).

---

## 2. Struktur Repositori & Boundary Tanggung Jawab

```text
sharphy/
├── .agents/                    # Agent workflow definitions & skill guidelines
├── src/
│   ├── Sharphy.Core/           # Mesin analisis inti:
│   │                           # - Diagnostics (ClrMD, SOS)
│   │                           # - Decompilation (ILSpy engine)
│   │                           # - Bytecode & Metadata (Mono.Cecil, dnlib, Roslyn)
│   ├── Sharphy.Server/         # Protokol MCP (Model Context Protocol):
│   │                           # - Stdio / SSE Transport
│   │                           # - Tool & Resource dispatchers
│   └── Sharphy.Cli/            # CLI executable & Global dotnet tool wrapper
├── samples/
│   └── CrackmeDemo/            # Testbed biner terisolasi untuk validasi fitur
├── tests/
│   └── Sharphy.Tests/          # Unit & integration testing multi-target
├── Directory.Build.props       # Global properties, nullable check, warnings, target frameworks
└── Sharphy.sln                 # Root solution
```

---

## 3. Protokol Alur Kerja Pengembangan (Workflow)

```
[Agent Trigger]
       │
       ▼
1. Research & Inspect  ───> Cek Directory.Build.props & arsitektur proyek
       │
       ▼
2. Implementation      ───> Tulis kode di Sharphy.Core/Server dengan isolated context
       │
       ▼
3. Multi-Target Verify ───> Jalankan: dotnet build Sharphy.sln
       │
       ▼
4. Test Suite          ───> Jalankan: dotnet test Sharphy.sln
       │
       ▼
5. Docs & Clean Git    ───> Pastikan README / docs tetap sinkron bilingual
```

---

## 4. Standar Perintah CLI untuk Agent

Agen harus menggunakan perintah berikut selama siklus iterasi:

```powershell
# Restore paket
dotnet restore Sharphy.sln

# Build solution
dotnet build Sharphy.sln -c Debug

# Jalankan test lintas framework
dotnet test Sharphy.sln --no-build

# Jalankan CLI secara lokal
dotnet run --project src/Sharphy.Cli/Sharphy.Cli.csproj -- mcp start
```

---

## 5. Direktori `.agents/`

Folder [.agents](file:///d:/Programming/GitHub/sharphy/.agents) memuat aturan dan spesifikasi workflow otomatis:
- `.agents/rules.md`: Aturan penulisan kode, penanganan error, dan keamanan memory dump.
- `.agents/mcp-schema.json`: Schema definisi MCP tools yang diimplementasikan oleh `Sharphy.Server`.
- `.agents/testing-policy.md`: Kebijakan pengujian, validasi memory dump, dan biner sample.
