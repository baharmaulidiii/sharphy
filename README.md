<div align="center">

<p align="center">
  <b>Bahasa Indonesia</b> · <a href="README.en.md">English</a>
</p>

# Sharphy: Reverse Engineer .NET

### Satu MCP untuk reverse engineering seluruh assembly .NET, memory runtime, dan CIL bytecode.
**Inspeksi tipe atau method yang Anda inginkan. Pahami cara kerjanya hingga ke tingkat instruksi CIL & native runtime.**

<p align="center">
  <a href="https://www.nuget.org/packages/Sharphy.Cli"><img src="https://img.shields.io/badge/nuget-v0.1.0--preview-blue?logo=nuget&logoColor=white" alt="NuGet Package" /></a>
  <a href="#"><img src="https://img.shields.io/badge/MCP-tool%20catalog-8A2BE2" alt="MCP Catalog" /></a>
  <a href="https://dotnet.microsoft.com/download"><img src="https://img.shields.io/badge/.NET-8%20|%209%20|%2010-512BD4?logo=dotnet&logoColor=white" alt=".NET 8/9/10" /></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/license-MIT-green" alt="MIT License" /></a>
  <a href="https://github.com/baharmaulidiii/sharphy"><img src="https://img.shields.io/badge/GitHub-baharmaulidiii%2Fsharphy-181717?logo=github" alt="GitHub Repo" /></a>
</p>

<p align="center">
  <a href="#quick-start"><b>Mulai Cepat</b></a> ·
  <a href="#cara-kerja-sharphy"><b>Cara Kerja Sharphy</b></a> ·
  <a href="#apa-yang-dapat-dianalisis"><b>Kemampuan Analisis</b></a> ·
  <a href="#katalog-mcp-tools"><b>Katalog MCP Tools</b></a> ·
  <a href="#faq"><b>FAQ</b></a> ·
  <a href="CONTRIBUTING.md"><b>Kontribusi</b></a>
</p>

```bash
dotnet tool install -g Sharphy.Cli
```

</div>

> [!CAUTION]
> **Pernyataan Penyangkalan & Aspek Legal (Disclaimer)**
> Sharphy dirancang semata-mata untuk tujuan edukasi, riset keamanan, audit kerentanan, debugging, dan reverse engineering resmi yang sah. Pengguna bertanggung jawab penuh untuk memastikan kepatuhan terhadap hukum lokal, perjanjian lisensi pengguna akhir (EULA), serta hak kekayaan intelektual perangkat lunak yang dianalisis. Pembuat dan kontributor tidak bertanggung jawab atas penyalahgunaan. Lihat [DISCLAIMER.md](DISCLAIMER.md) untuk ketentuan lengkap.

> [!IMPORTANT]
> Unduh dan jalankan Sharphy hanya dari sumber resmi: NuGet (`Sharphy.Cli`), atau build langsung dari source code di [github.com/baharmaulidiii/sharphy](https://github.com/baharmaulidiii/sharphy).

---

## Apa yang Dapat Dianalisis

Sharphy mengubah AI coding agent mana pun (seperti Cursor, Claude Code, Windsurf, Copilot, Gemini) menjadi seorang **Autonomous CLR Engineer**:

* **Dekomplikasi & Rekonstruksi Kode Sumber:** Mendekompresi assembly, type, atau method spesifik menjadi kode C# modern yang bersih menggunakan ILSpy engine dan ECMA-335 metadata reader.
* **Forensik CLR Crash Dump:** Membaca memory dump Windows/Linux (`.dmp`), mendeteksi deadlock antar-thread, dan melacak memory leak pada heap.
* **Anti-Obfuscation Unpacking:** Menganalisis CFG flattening, mendekripsi string tersembunyi, dan mengekstrak token dalam `AssemblyLoadContext` yang terisolasi.
* **Mutasi & Patching Instruksi CIL:** Mengganti opcode instruksi CIL secara aman (`nop`, return value, branch target) dan mengompilasi ulang biner tanpa membutuhkan source code asli.

---

## Quick Start

### 1. Pasang Global CLI Tool

```bash
dotnet tool install -g Sharphy.Cli
```

### 2. Hubungkan ke AI Agent Anda (Claude Code / Cursor / Windsurf)

Tambahkan Sharphy ke konfigurasi MCP client Anda (`mcpServers`):

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

Atau jalankan langsung dari source code melalui `dotnet run`:

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

## Katalog MCP Tools

| Nama Tool | Kategori | Deskripsi |
| :--- | :--- | :--- |
| `sharphy_decompile_method` | Dekompiler | Mengekstrak method CIL tertentu langsung menjadi representasi C# bersih beserta atribut internal/compiler-generated. |
| `sharphy_diff_assemblies` | Dekompiler | Membandingkan dua assembly `.dll`/`.exe` untuk melihat breaking API changes atau logika tersembunyi. |
| `sharphy_inspect_metadata` | Metadata | Membaca hierarki tipe ECMA-335, signature P/Invoke, attribute, dan resource bawaan. |
| `sharphy_analyze_dump` | Diagnostik | Membaca memory dump (`.dmp`) Windows/Linux dan langsung menarik Root Cause Analysis (RCA). |
| `sharphy_inspect_heap` | Diagnostik | Memeriksa managed heap untuk menemukan memory leak, duplikasi string, dan fragmentasi Large Object Heap (LOH). |
| `sharphy_deadlock_detector`| Diagnostik | Melacak thread lock chain dan monitor sinkronisasi yang berpotensi hang. |
| `sharphy_clean_obfuscation`| Unpacker | Membantu membersihkan teknik obfuscation dasar (CFG flattening, proxy delegates, string decrypters). |
| `sharphy_extract_tokens` | Unpacker | Menjalankan rutinitas dekripsi dalam `AssemblyLoadContext` terisolasi. |
| `sharphy_patch_il` | Patching | Mengganti instruksi CIL langsung dan mere-emit biner tanpa file projek awal. |
| `sharphy_generate_wrapper` | Interop | Meng-generate strongly-typed wrapper C# berkinerja tinggi menggunakan `UnsafeAccessor`. |

---

## Arsitektur & Multi-Targeting

Sharphy dirancang untuk mendukung multi-targeting pada runtime modern .NET:
* **.NET 8.0 (LTS)**
* **.NET 9.0 (STS)**
* **.NET 10.0 (Preview / Latest)**

```text
sharphy/
├── src/
│   ├── Sharphy.Core/           # Logic bridge (ILSpy, ClrMD, Roslyn, Cecil, dnlib)
│   ├── Sharphy.Server/         # MCP Server (Stdio & SSE protocol handlers)
│   └── Sharphy.Cli/            # Global CLI tool (dotnet tool)
├── samples/
│   └── CrackmeDemo/            # Target sample binary
├── tests/
│   └── Sharphy.Tests/          # Unit tests lintas target framework
├── .github/workflows/          # CI matrix workflow (.NET 8/9/10)
└── README.md
```

---

## FAQ

<details>
<summary><b>Apakah Sharphy membutuhkan source code asli target?</b></summary>
Tidak. Sharphy bekerja langsung terhadap biner terkompilasi (<code>.dll</code>, <code>.exe</code>), memory dump (<code>.dmp</code>), atau memori runtime proses.
</details>

<details>
<summary><b>Apakah Sharphy bisa digunakan di Windows dan Linux?</b></summary>
Ya. Sharphy dibangun di atas .NET cross-platform modern (8/9/10) dan mendukung pembacaan dump serta assembly baik di Windows maupun Linux.
</details>

---

## Kontribusi & Keamanan

- Kontribusi sangat terbuka! Silakan baca panduan di [CONTRIBUTING.md](CONTRIBUTING.md).
- Untuk pelaporan kerentanan keamanan, silakan lihat [SECURITY.md](SECURITY.md).
- Penjelasan batasan hukum dan legalitas tercantum di [DISCLAIMER.md](DISCLAIMER.md).

## Lisensi

Sharphy dirilis di bawah naungan [Lisensi MIT](LICENSE).
