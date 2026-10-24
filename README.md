<div align="center">

<img src="assets/sharphy.svg" alt="Sharphy Logo" width="120" />

# Sharphy: CLR Intelligence & Runtime Forensic Bridge

### Model Context Protocol (MCP) Server Native untuk Mentransformasi AI Coding Agent Menjadi Diagnostisi Tingkat Tinggi, Analis Forensik Memori, dan Arsitek Biner Runtime .NET.
**Pemanfaatan Semantik Metadata ECMA-335, Analisis Crash Dump Langsung Lintas Sistem Operasi, dan Rekonstruksi Bytecode CIL Secara Rigid.**

<p align="center">
  <a href="https://www.nuget.org/packages/Sharphy.Cli"><img src="https://img.shields.io/badge/nuget-v0.1.0--preview-blue?logo=nuget&logoColor=white" alt="NuGet Package" /></a>
  <a href="#"><img src="https://img.shields.io/badge/MCP-10%20tools%20catalog-8A2BE2" alt="MCP Catalog" /></a>
  <a href="https://dotnet.microsoft.com/download"><img src="https://img.shields.io/badge/.NET-8%20|%209%20|%2010-512BD4?logo=dotnet&logoColor=white" alt=".NET 8/9/10" /></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/license-MIT-green" alt="MIT License" /></a>
  <a href="CODE_OF_CONDUCT.md"><img src="https://img.shields.io/badge/Contributor%20Covenant-2.1-4baaaa.svg" alt="Contributor Covenant" /></a>
  <a href="https://github.com/baharmaulidiii/sharphy"><img src="https://img.shields.io/badge/GitHub-baharmaulidiii%2Fsharphy-181717?logo=github" alt="GitHub Repo" /></a>
</p>

<p align="center">
  <a href="#ikhtisar-arsitektur--filosofi"><b>Ikhtisar & Arsitektur</b></a> ·
  <a href="#ekosistem-paket--komposisi-peran"><b>Ekosistem Paket</b></a> ·
  <a href="#katalog-eksplisit-10-mcp-tools"><b>Katalog 10 MCP Tools</b></a> ·
  <a href="#panduan-pemasangan--integrasi-agent"><b>Integrasi Agent</b></a> ·
  <a href="#standar-komunitas-kebijakan-keamanan--hukum"><b>Kebijakan & Komunitas</b></a> ·
  <a href="CHANGELOG.md"><b>Changelog</b></a> ·
  <a href="CONTRIBUTING.md"><b>Panduan Kontribusi</b></a> ·
  <a href="CODE_OF_CONDUCT.md"><b>Kode Etik</b></a>
</p>

```bash
dotnet tool install -g Sharphy.Cli
```

</div>

> [!CAUTION]
> **Pernyataan Penyangkalan Hukum & Batasan Tanggung Jawab (Legal Disclaimer)**
> Sharphy dirancang dan didistribusikan secara eksklusif untuk tujuan riset keamanan siber beriktikad baik (*good-faith security research*), audit diagnostik kegagalan produksi, analisis kerentanan, investigasi insiden, serta rekayasa balik perangkat lunak yang sah menurut hukum. Pengguna memikul tanggung jawab perdata dan pidana secara mandiri atas kepatuhan terhadap yurisdiksi hukum lokal, Perjanjian Lisensi Pengguna Akhir (EULA), dan hak kekayaan intelektual (HAKI) pihak ketiga. Segala bentuk penyalahgunaan berada di luar tanggung jawab pengembang. Rujukan lengkap: [DISCLAIMER.md](DISCLAIMER.md).

> [!IMPORTANT]
> Unduh dan jalankan Sharphy hanya dari kanal distribusi resmi: repositori NuGet terverifikasi (`Sharphy.Cli`) atau melalui kompilasi kode sumber langsung dari repositori resmi di [github.com/baharmaulidiii/sharphy](https://github.com/baharmaulidiii/sharphy).

---

## Ikhtisar Arsitektur & Filosofi

Sebagian besar AI coding agent (*Claude Code*, *Cursor*, *Windsurf*, *Copilot*) memiliki keterbatasan fundamental: mereka hanya beroperasi pada tataran heuristik teks mentah di editor, tanpa akses observabilitas terhadap struktur runtime internal Common Language Runtime (CLR). Saat menghadapi kerusakan sistem berupa crash memori, deadlock antar-thread, anomali Large Object Heap (LOH), atau kebutuhan audit biner pihak ketiga tanpa kode sumber, AI agent konvensional mengalami kebutaan diagnostik (*runtime blindness*).

**Sharphy** bertindak sebagai jembatan forensik runtime deterministik berbasis standar **Model Context Protocol (MCP)**. Menggunakan kanal komunikasi JSON-RPC 2.0 melalui protokol Stdio yang terisolasi secara murni, Sharphy mengekspos mesin diagnostik tingkat rendah Microsoft ke antarmuka pemikiran semantik LLM tingkat tinggi.

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
│   │  Kanal Stdio Purni (stdout: Payload / stderr: Log)  │   │
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
│                  Target Analisis Runtime                    │
│  • File Crash Dump Windows / Linux (.dmp / ELF dump)        │
│  • Rakitan Biner Terkompilasi (.dll / .exe ECMA-335)        │
│  • Proses Runtime Aktif & Isolasi AssemblyLoadContext       │
└─────────────────────────────────────────────────────────────┘
```

---

## Ekosistem Paket & Komposisi Peran

Solusi Sharphy disusun ke dalam 3 proyek modular dengan batas tanggung jawab yang tegas:

| Paket / Modul | Bentuk Distribusi | Target Konsumen | Peran Teknis & Batasan Arsitektural |
| :--- | :--- | :--- | :--- |
| **`Sharphy.Cli`** | `.NET Global Tool` | Pengguna Akhir & Terminal Agent | Titik masuk eksekusi perintah baris (*command-line entry point*). Bertindak sebagai launcher instan untuk menjalankan server host MCP (`sharphy mcp start`) atau utilitas inspeksi kilat tanpa konfigurasi manual. |
| **`Sharphy.Server`** | `Class Library / Host` | Agent Frameworks & Runtime Hosts | Mengimplementasikan spesifikasi resmi Model Context Protocol (MCP) berbasis JSON-RPC 2.0. Memisahkan aliran data: `stdout` didedikasikan 100% untuk frame JSON-RPC, sementara seluruh log diagnostik internal diarahkan ke `stderr`. |
| **`Sharphy.Core`** | `Class Library Engine` | Arsitek & Tool Builders .NET | Engine analitis tanpa dependensi antarmuka MCP. Mengabstraksi pustaka diagnostik sistem tingkat rendah: `Microsoft.Diagnostics.Runtime` (ClrMD), `ICSharpCode.Decompiler` (ILSpy), `Mono.Cecil`, dan `Microsoft.CodeAnalysis.CSharp` (Roslyn). |

---

## Katalog Eksplisit 10 MCP Tools

Sharphy menyediakan 10 instrumen analisis deterministik yang dipetakan ke dalam 4 pilar forensik utama:

### Pilar 1: Diagnostik Runtime & Forensik Memori (ClrMD Engine)

#### 1. `sharphy_analyze_dump`
* **Tujuan Operasional:** Menjalankan *Root Cause Analysis* (RCA) otomatis terhadap file crash dump memori Windows (`.dmp`) dan Linux CoreCLR ELF dumps.
* **Parameter Masukan:**
  * `dumpPath` (*string, wajib*): Path absolut menuju berkas crash dump.
* **Mekanisme Internal:** Membuka `DataTarget`, menginisialisasi `ClrRuntime`, mengekstraksi thread penyebab crash (*crashing thread*), mengurai struktur `ClrException` beserta rantai `InnerException`, men-decode kode status HRESULT, serta memetakan *mixed-mode call stack* (gabungan managed dan unmanaged frame).
* **Nilai Forensik:** Menyajikan ringkasan deterministik akar kegagalan fatal aplikasi secara instan kepada agen AI tanpa perlu membuka debugger WinDbg atau lldb secara manual.

#### 2. `sharphy_inspect_heap`
* **Tujuan Operasional:** Mengaudit alokasi memori *managed heap*, melacak retensi objek penyebab *memory leak*, mendeteksi pemborosan duplikasi string, serta mengidentifikasi fragmentasi pada Large Object Heap (LOH) dan Pinned Object Heap (POH).
* **Parameter Masukan:**
  * `dumpPath` (*string, wajib*): Path absolut menuju berkas crash dump.
  * `topObjects` (*integer, opsional, default: 20*): Batas jumlah tipe objek teratas yang diurutkan berdasarkan akumulasi konsumsi memori.
* **Mekanisme Internal:** Melakukan traversal penuh terhadap tabel segmen GC, menghitung metrik agregat per `ClrType`, menganalisis penumpukan objek berumur panjang pada Generasi 2, dan memeriksa rantai GC Roots yang menahan objek dari proses pembersihan.
* **Nilai Forensik:** AI agent memperoleh visibilitas komprehensif mengenai struktur objek yang memonopoli memori dan akar penyebab kenaikan konsumsi RAM tak terkendali.

#### 3. `sharphy_deadlock_detector`
* **Tujuan Operasional:** Mendeteksi kondisi saling kunci antar-thread (*inter-thread deadlock*) dan kontensi ekstrem pada monitor sinkronisasi.
* **Parameter Masukan:**
  * `dumpPath` (*string, wajib*): Path absolut menuju berkas crash dump.
* **Mekanisme Internal:** Memindai seluruh struktur tabel `SyncBlock` pada CLR, mengevaluasi properti kepemilikan kunci monitor (`Monitor.Enter`), membangun representasi *Wait-For Graph* (WFG) terarah, serta menjalankan algoritma pendeteksi siklus tertutup (*cycle-finding algorithm*).
* **Nilai Forensik:** Mengidentifikasi secara spesifik Thread ID mana yang saling mengunci sumber daya dan baris instruksi penyebab pembekuan (*freezing*) proses aplikasi.

---

### Pilar 2: Rekonstruksi Logika & Analisis Metadata (ILSpy & Cecil Engines)

#### 4. `sharphy_decompile_method`
* **Tujuan Operasional:** Merekonstruksi instruksi bytecode CIL dari suatu metode biner menjadi kode sumber C# modern yang bersih, terstruktur, dan mudah dipahami.
* **Parameter Masukan:**
  * `assemblyPath` (*string, wajib*): Path absolut menuju berkas rakitan biner (`.dll` atau `.exe`).
  * `typeName` (*string, wajib*): Nama tipe atau kelas lengkap disertai namespace (contoh: `Contoso.Security.LicenseValidator`).
  * `methodName` (*string, wajib*): Nama metode target yang ingin didekompilasi.
* **Mekanisme Internal:** Memanfaatkan mesin dekompilasi ILSpy (`ICSharpCode.Decompiler`) untuk memulihkan konstruksi sintaksis tingkat tinggi, termasuk pemulihan mesin *async/await state machine*, *pattern matching*, *local functions*, ekspresi lambda, serta atribut kompilasi.
* **Nilai Forensik:** AI agent dapat membaca dan menalar implementasi algoritma biner internal tanpa ketersediaan repositori kode sumber asli.

#### 5. `sharphy_diff_assemblies`
* **Tujuan Operasional:** Mengomparasikan dua rilis atau versi biner rakitan untuk mendeteksi perubahan API yang merusak (*breaking changes*), alterasi visibilitas tipe, dan mutasi logika internal.
* **Parameter Masukan:**
  * `oldAssemblyPath` (*string, wajib*): Path rakitan versi baseline/lama.
  * `newAssemblyPath` (*string, wajib*): Path rakitan versi teranyar.
* **Mekanisme Internal:** Mengurai definisi metadata ECMA-335 pada kedua berkas biner, membandingkan signature publik type/method/event/property, serta mendeteksi divergensi struktur IL body pada metode dengan signature identik.
* **Nilai Forensik:** Memastikan kompatibilitas biner lintas versi serta mendeteksi modifikasi tak terduga pada dependensi pihak ketiga.

#### 6. `sharphy_inspect_metadata`
* **Tujuan Operasional:** Mengekstraksi struktur arsitektur metadata ECMA-335 secara rigid dan menyeluruh dari suatu rakitan biner.
* **Parameter Masukan:**
  * `assemblyPath` (*string, wajib*): Path absolut menuju berkas rakitan biner target.
* **Mekanisme Internal:** Membaca header CLR, mengekstrak hierarki pewarisan tipe (`TypeDef`, `TypeRef`), definisi argumen generik, daftar atribut kustom, impor fungsi native platform P/Invoke (`DllImport`), serta manifest sumber daya tersemat (*embedded resources*).
* **Nilai Forensik:** Memberikan gambaran arsitektur biner secara komprehensif bagi audit keamanan antarmuka dan integritas dependensi native.

---

### Pilar 3: Integritas Biner & Sandbox Deobfuscation (ALC & Cecil)

#### 7. `sharphy_clean_obfuscation`
* **Tujuan Operasional:** Menganalisis, memetakan, dan merapikan teknik pengaburan kode (*obfuscation*) yang umum ditemukan pada biner .NET.
* **Parameter Masukan:**
  * `assemblyPath` (*string, wajib*): Path rakitan ter-obfuscate.
  * `outputPath` (*string, wajib*): Path penyimpanan untuk biner hasil rekonstruksi.
* **Mekanisme Internal:** Menganalisis topologi Control Flow Graph (CFG) untuk meratakan percabangan dispatcher buatan (*CFG flattening removal*), mengidentifikasi pola proxy delegates, serta mendeteksi stub enkripsi string.
* **Nilai Forensik:** Memulihkan keterbacaan kode biner yang diproteksi secara agresif sehingga dapat diaudit dengan aman.

#### 8. `sharphy_extract_tokens`
* **Tujuan Operasional:** Mengekstraksi token dekripsi atau mengeksekusi rutin unpacking dinamis di dalam lingkungan terisolasi yang aman.
* **Parameter Masukan:**
  * `assemblyPath` (*string, wajib*): Path menuju berkas biner target.
* **Mekanisme Internal:** Memuat rakitan ke dalam `AssemblyLoadContext` kolektibel khusus (`isCollectible: true`). Seluruh refleksi dan evaluasi token berjalan dalam sandbox memori tertutup, yang kemudian di-unload secara penuh dari memori proses host untuk mencegah kebocoran referensi atau penguncian file biner di disk.
* **Nilai Forensik:** Menjamin keamanan proses host Sharphy dan agent saat menganalisis biner yang tidak tepercaya (*untrusted binaries*).

---

### Pilar 4: Mutasi Bytecode & Interop Akselerasi (Cecil & UnsafeAccessor)

#### 9. `sharphy_patch_il`
* **Tujuan Operasional:** Memodifikasi, mengganti, atau menyuntikkan instruksi bytecode CIL pada metode tertentu dan memancarkan (*re-emit*) berkas biner terpatch yang valid.
* **Parameter Masukan:**
  * `assemblyPath` (*string, wajib*): Path rakitan biner asli.
  * `typeName` (*string, wajib*): Nama tipe kelas target.
  * `methodName` (*string, wajib*): Nama metode yang akan dimutasi.
  * `instructionOffset` (*integer, wajib*): Offset IL spesifik instruksi yang ingin diubah.
  * `newOpCode` (*string, wajib*): Nama OpCode CIL pengganti (misal: `Nop`, `Ldc_I4_1`, `Ret`).
  * `outputPath` (*string, wajib*): Path keluaran untuk rakitan biner baru.
* **Mekanisme Internal:** Memuat biner via `Mono.Cecil`, menavigasi `MethodBody.Instructions`, menghitung ulang penanda lompatan instruksi cabang (*branch targets* dan *exception handlers*), menyuntikkan opcode target, dan menyimpan ulang rakitan dengan integritas PE checksum yang valid.
* **Nilai Forensik:** Memungkinkan patching diagnostik cepat (misal: bypass pengujian integritas sementara untuk mengisolasi galat biner pihak ketiga).

#### 10. `sharphy_generate_wrapper`
* **Tujuan Operasional:** Menghasilkan kode pembungkus C# biner yang sangat cepat (*zero-overhead*) untuk mengakses status internal atau private anggota kelas tanpa refleksi tradisional.
* **Parameter Masukan:**
  * `assemblyPath` (*string, wajib*): Path rakitan target.
  * `typeName` (*string, wajib*): Nama tipe target yang ingin dibungkus.
  * `outputPath` (*string, wajib*): Path berkas kode sumber C# (`.cs`) yang dihasilkan.
* **Mekanisme Internal:** Memindai seluruh anggota private/internal tipe target dan mensintesis definisi atribut `[UnsafeAccessor]` (.NET 8+) atau kelas interop Roslyn strongly-typed.
* **Nilai Forensik:** Memungkinkan interop performa tinggi dengan komponen pustaka tertutup tanpa terkena penalti performa reflection.

---

## Panduan Pemasangan & Integrasi Agent

### 1. Pemasangan Global CLI Tool
Pasang Sharphy secara global pada mesin pengembang Anda:
```bash
dotnet tool install -g Sharphy.Cli
```
Verifikasi status instalasi dan versi framework runtime aktif:
```bash
sharphy --version
# Output: Sharphy CLI v0.1.0-preview [.NET 8.0 (LTS)]
```

### 2. Integrasi ke Claude Code
Jalankan pendaftaran server MCP ke Claude Code CLI:
```bash
claude mcp add sharphy -- sharphy mcp start
```

### 3. Integrasi ke Cursor / Windsurf
Buka berkas konfigurasi MCP (`claude_desktop_config.json` atau pengaturan MCP Cursor) dan tambahkan entri berikut:
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

Bagi pengembang yang ingin menjalankan langsung dari *source tree* lokal:
```json
{
  "mcpServers": {
    "sharphy": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "D:/Programming/GitHub/sharphy/src/Sharphy.Cli/Sharphy.Cli.csproj",
        "--framework",
        "net8.0",
        "--",
        "mcp",
        "start"
      ]
    }
  }
}
```

---

## Matriks Multi-Targeting

Sharphy dibangun dengan arsitektur multi-targeting native tanpa emulasi:
* **.NET 8.0 (LTS):** Kompatibilitas jangka panjang untuk lingkungan perusahaan dan infrastruktur server stabil.
* **.NET 9.0 (STS):** Optimasi runtime terkini dan penyempurnaan alokasi memori GC.
* **.NET 10.0 (Latest):** Fitur runtime eksperimental terdepan dan pemanfaatan compiler API termodern.

---

## Standar Komunitas, Kebijakan Keamanan & Hukum

- Tata kelola kontribusi teknis diuraikan di [CONTRIBUTING.md](CONTRIBUTING.md).
- Pedoman perilaku komunitas dan kode etik diatur dalam [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md).
- Model mitigasi ancaman siber dan kanal pelaporan insiden tersedia di [SECURITY.md](SECURITY.md).
- Komitmen aksesibilitas (WCAG 2.1 AA & pembaca layar) termaktub dalam [ACCESSIBILITY.md](ACCESSIBILITY.md).
- Formulir pelaporan isu dan pengajuan kontribusi kode tersedia di [.github/ISSUE_TEMPLATE](.github/ISSUE_TEMPLATE) dan [.github/pull_request_template.md](.github/pull_request_template.md).
- Batasan legalitas dan pelepasan tanggung jawab diatur dalam [DISCLAIMER.md](DISCLAIMER.md).
- Hak cipta pustaka dependensi tertera di [THIRD_PARTY_NOTICE.md](THIRD_PARTY_NOTICE.md).

## Lisensi

Proyek Sharphy didistribusikan di bawah naungan [Lisensi MIT](LICENSE).

---

<details>
<summary><b>🇬🇧 English Translation (Click to expand)</b></summary>

<div align="center">

# Sharphy: CLR Intelligence & Runtime Forensic Bridge

### Native Model Context Protocol (MCP) Server Empowering AI Coding Agents as Advanced .NET Runtime Diagnosticians, Memory Forensics Analysts, and Binary Architects.
**Harnessing ECMA-335 Metadata Semantics, Cross-Platform Memory Crash Dump Telemetry, and Rigid CIL Bytecode Navigation.**

</div>

## Architecture Overview & Philosophy

Mainstream AI coding agents operate purely on raw text editor heuristics, lacking visibility into CLR runtime internals. When encountering memory crash dumps, synchronization deadlocks, or closed-source third-party binaries, agents suffer from runtime blindness.

**Sharphy** serves as a deterministic runtime forensic bridge implementing the Model Context Protocol (MCP) standard. Over a pure Stdio transport channel, Sharphy bridges low-level CLR diagnostics to high-level semantic AI reasoning.

## Package Ecosystem

| Package | Type | Target Audience | Value Proposition |
| :--- | :--- | :--- | :--- |
| **`Sharphy.Cli`** | `.NET Global Tool` | End Users & Agents | Global CLI launcher (`sharphy mcp start`) providing zero-configuration operational startup. |
| **`Sharphy.Server`** | `Class Library / Host` | Hosts & Frameworks | Pure JSON-RPC 2.0 MCP server host mapping `Sharphy.Core` engines to agent tool catalogs over Stdio. |
| **`Sharphy.Core`** | `Class Library` | CLR Engineers | Core engine wrapping ClrMD, ILSpy decompiler, Mono.Cecil, and Roslyn without MCP dependencies. |

## 10 MCP Tools Catalog Summary

1. `sharphy_analyze_dump`: Automated Root Cause Analysis (RCA) on Windows and Linux CoreCLR crash dumps.
2. `sharphy_inspect_heap`: Deep managed heap inspection, GC generation audits, memory leak detection, and LOH analysis.
3. `sharphy_deadlock_detector`: Traces Monitor locks and SyncBlock synchronization cycles across threads.
4. `sharphy_decompile_method`: Reconstructs CIL bytecode into idiomatic modern C# source code via ILSpy.
5. `sharphy_diff_assemblies`: Compares binary assemblies to identify breaking API changes and bytecode divergence.
6. `sharphy_inspect_metadata`: Comprehensive ECMA-335 metadata hierarchy, P/Invoke imports, and resource dumper.
7. `sharphy_clean_obfuscation`: Demangles Control Flow Graph (CFG) flattening and resolves proxy delegates.
8. `sharphy_extract_tokens`: Safe isolated execution and unpacking within collectible `AssemblyLoadContext` sandboxes.
9. `sharphy_patch_il`: Surgical CIL opcode replacement and binary re-emitting.
10. `sharphy_generate_wrapper`: Automated synthesis of zero-overhead C# wrappers using `[UnsafeAccessor]` or Roslyn.

## Community Standards, Security & Legal Policies

- **Contributing:** Engineering guidelines and workflows in [CONTRIBUTING.md](CONTRIBUTING.md).
- **Code of Conduct:** Community expectations and enforcement in [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md).
- **Security Policy:** Threat model and reporting procedure in [SECURITY.md](SECURITY.md).
- **Accessibility:** Inclusivity commitments and barrier reporting in [ACCESSIBILITY.md](ACCESSIBILITY.md).
- **Issue Forms & PR Template:** Structured collaboration via [.github/ISSUE_TEMPLATE](.github/ISSUE_TEMPLATE) and [.github/pull_request_template.md](.github/pull_request_template.md).
- **Legal Disclaimer & Third-Party:** Liability boundaries in [DISCLAIMER.md](DISCLAIMER.md) and license notices in [THIRD_PARTY_NOTICE.md](THIRD_PARTY_NOTICE.md).
- **License:** Distributed under the [MIT License](LICENSE).

</details>
