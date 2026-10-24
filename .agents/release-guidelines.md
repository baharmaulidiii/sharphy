# Pedoman Manajemen Perilisan & Deployment Sharphy

> **Dokumen Resmi Tata Kelola Rilis, Pemaketan NuGet, dan Deployment Portal Dokumentasi**  
> Standar operasional untuk Pemilik Repositori (*Human Repository Owner*) dan AI Coding Agents.

Dokumen ini mendefinisikan aturan baku siklus rilis biner, pemaketan paket NuGet, deployment portal dokumentasi web GitHub Pages, serta protokol deteksi dampak perubahan (*Change Impact Detection Protocol*) di repositori **Sharphy**.

---

## 1. Prinsip Dasar & Skema Penomoran Versi (SemVer 2.0.0)

Repositori Sharphy mengadopsi standar **[Semantic Versioning 2.0.0](https://semver.org/lang/id/)** dengan format:

$$\text{MAJOR}.\text{MINOR}.\text{PATCH}[-\text{PRERELEASE}]$$

### 1.1 Tahapan Fase Prarilis (*Preview Phase*)
Selama proyek berada pada fase `0.y.z-preview`, antarmuka publik dan spesifikasi tool MCP dapat berkembang secara dinamis:
- **`0.MINOR.0-preview`**: Penambahan kapabilitas baru yang signifikan, penambahan instrumen tool MCP baru, atau perubahan arsitektural internal.
- **`0.MINOR.PATCH-preview`**: Perbaikan bug, penguatan penanganan galat (*resilient error handling*), pembaruan dokumentasi biner, atau optimasi alokasi memori CIL.

### 1.2 Tahapan Stabil (*General Availability - v1.0.0+*)
Setelah seluruh fondasi dekompiler (`ILSpy`), diagnostik heap (`ClrMD`), dan perakit CIL (`Mono.Cecil`) terintegrasi penuh:
- **`MAJOR`**: Perubahan yang merusak kompatibilitas mundur (*breaking changes*) pada protokol JSON-RPC MCP atau kontrak API publik `Sharphy.Core`.
- **`MINOR`**: Penambahan tool MCP baru, penambahan opsi CLI baru, atau backward-compatible API additions.
- **`PATCH`**: Perbaikan bug tanpa mengubah kontrak antarmuka tool atau protokol.

---

## 2. Matriks Deteksi Dampak Perubahan (*Tri-Pillar Detection Matrix*)

Setiap kali pekerjaan pengembangan (*development task*) selesai, Agent dan Developer wajib mengevaluasi matriks berikut untuk menentukan konsekuensi rilis:

| Dimensi Perubahan | Indikator Berkas / Jalur | Memerlukan Git Tag / Release? | Memerlukan Deploy NuGet? | Memerlukan Deploy Pages? | Rekomendasi Versi |
| :--- | :--- | :---: | :---: | :---: | :---: |
| **Pillar 1: Core Engine & MCP Server** | `src/Sharphy.Core/**`<br>`src/Sharphy.Server/**` | **YA** | **YA** (`Sharphy.Core`<br>`Sharphy.Server`) | Tidak (Kecuali tabel docs diubah) | **Minor / Patch** (`0.x.y-preview`) |
| **Pillar 1b: CLI Executable** | `src/Sharphy.Cli/**` | **YA** | **YA** (`Sharphy.Cli` Global Tool) | Tidak | **Patch / Minor** |
| **Pillar 2: Web Documentation Portal** | `docs/**`<br>`assets/**` | Tidak wajib rilis biner | Tidak | **YA** (Trigger `deploy-docs.yml`) | Tanpa bump biner |
| **Pillar 3: Non-Code Project Meta** | Root `*.md`, `.agents/**`, `.github/**` | Tidak wajib | Tidak | Tidak | Sinkronisasi `CHANGELOG.md` |

---

## 3. Matriks Komponen Terdistribusi

Sharphy mendistribusikan tiga paket biner NuGet dan satu portal dokumentasi statis:

1. **`Sharphy.Cli`** (.NET Global Tool):
   - Command: `sharphy mcp start`
   - Target Konsumen: AI Agent runner (Claude Code, Cursor, Windsurf, AGY) dan insinyur reverse engineering CLI.
2. **`Sharphy.Server`** (Pustaka MCP Server Host):
   - Target Konsumen: Pengembang yang ingin mengintegrasikan server MCP Sharphy ke dalam host kustom.
3. **`Sharphy.Core`** (Pustaka Engine Diagnostik CLR):
   - Target Konsumen: Pustaka fondasi analisis runtime, dekompilasi, mutasi CIL, dan sandboxing.
4. **`docs/`** (Web Portal Next.js):
   - Host: GitHub Pages (`https://baharmaulidiii.github.io/sharphy/`)
   - Target Konsumen: Dokumentasi publik, katalog 10 MCP tools, dan konfigurasi agen.

---

## 4. Standar Pemeliharaan `CHANGELOG.md`

Pengelolaan berkas [`CHANGELOG.md`](../CHANGELOG.md) wajib mengikuti standar **[Keep a Changelog](https://keepachangelog.com/id/1.1.0/)**:
1. Setiap fitur atau perbaikan baru yang belum dirilis wajib dicatat di bawah seksi `## [Belum Dirilis] - Unreleased`.
2. Gunakan sub-kategori baku:
   - `### Ditambahkan (Added)` untuk kapabilitas/tools baru.
   - `### Diubah (Changed)` untuk modifikasi fungsionalitas yang sudah ada.
   - `### Diperbaiki (Fixed)` untuk perbaikan bug.
   - `### Keamanan (Security)` untuk penanganan kerentanan, transport isolation, atau sandboxing.
3. Saat rilis baru diterbitkan, seksi `[Belum Dirilis]` dipindahkan ke nomor versi rilis dengan tanggal ISO 8601 (contoh: `## [0.1.1-preview] - 2026-10-24`).
4. Wajib mempertahankan format dwibahasa (Bahasa Indonesia formal utama dengan terjemahan Bahasa Inggris di `<details>`).

---

## 5. Checklist Verifikasi Pra-Rilis (*Pre-Release Checklist*)

Sebelum melakukan tagging rilis baru atau mempublikasikan paket, lakukan verifikasi lokal:

```powershell
# 1. Verifikasi kompilasi bersih pada seluruh framework (net8.0, net9.0, net10.0)
dotnet build Sharphy.sln -c Release

# 2. Eksekusi pengujian unit protokol MCP
dotnet test Sharphy.sln -c Release --no-build

# 3. Verifikasi sanitasi Stdio
# Pastikan tidak ada Console.WriteLine liar di src/Sharphy.Server
# stdout murni untuk JSON-RPC 2.0; log diagnostik hanya via Console.Error.WriteLine

# 4. Uji coba pemaketan lokal (Dry-Run Packaging)
$tempDir = "./artifacts_dryrun"
New-Item -ItemType Directory -Force -Path $tempDir
dotnet pack src/Sharphy.Core/Sharphy.Core.csproj -c Release -o $tempDir /p:PackageVersion=0.1.1-preview
dotnet pack src/Sharphy.Server/Sharphy.Server.csproj -c Release -o $tempDir /p:PackageVersion=0.1.1-preview
dotnet pack src/Sharphy.Cli/Sharphy.Cli.csproj -c Release -o $tempDir /p:PackageVersion=0.1.1-preview
Get-ChildItem -Path $tempDir -Filter *.nupkg
Remove-Item -Recurse -Force $tempDir
```

---

## 6. Prosedur Operasional Standar (SOP) Eksekusi Rilis

### 6.1 Batasan Hak Akses: AI Agent vs Repo Owner
- **AI Agent:** Berwenang melakukan pemeriksaan diff, memvalidasi build/test/pack lokal, menyinkronkan `CHANGELOG.md`, dan menyiapkan laporan rilis. **DILARANG KERAS** menjalankan `git commit`, `git push`, atau membuat git tag di remote.
- **Repository Owner:** Berwenang penuh melakukan commit, push ke branch `main`, membuat `git tag`, dan memicu rilis GitHub.

### 6.2 Langkah-Langkah Eksekusi Rilis Baru (Oleh Repo Owner)
```powershell
# 1. Pastikan perubahan lokal sudah di-stage dan di-commit
git add .
git commit -m "chore(release): prepare v0.1.1-preview release"
git push origin main

# 2. Buat Tag Rilis Beranotasi
git tag -a v0.1.1-preview -m "Release v0.1.1-preview: Functional Stdio MCP Server, 10 Tool Catalog, and CLI Runner"

# 3. Dorong Tag ke Remote GitHub
git push origin v0.1.1-preview
```
*Catatan:* Mendorong tag dengan pola `v*` akan memicu workflow [`.github/workflows/publish-packages.yml`](../.github/workflows/publish-packages.yml) secara otomatis untuk mem-pack dan mengunggah ketiga paket ke GitHub Packages dan NuGet.org.

### 6.3 Langkah-Langkah Deploy Web Dokumentasi GitHub Pages (Oleh Repo Owner)
Workflow [`.github/workflows/deploy-docs.yml`](../.github/workflows/deploy-docs.yml) diatur dengan pemicu manual (`workflow_dispatch`).
1. Kunjungi menu **Actions** di repositori GitHub:  
   `https://github.com/baharmaulidiii/sharphy/actions/workflows/deploy-docs.yml`
2. Pilih tombol **Run workflow** pada branch `main`.
3. GitHub Actions akan mengompilasi aplikasi Next.js di direktori `docs/` dan mempublikasikan artefak ke GitHub Pages.

---

<details>
<summary><b>🇬🇧 English Translation: Release & Deployment Management Guidelines (Click to expand)</b></summary>

# Sharphy Release & Deployment Management Guidelines

> **Authoritative Operational Policy for Binary Releases, NuGet Packaging, and Documentation Deployments**  
> Operational standard for Repository Owners and AI Coding Agents.

---

## 1. Versioning Principles (SemVer 2.0.0)

Sharphy strictly adheres to **Semantic Versioning 2.0.0**:
- **Preview Phase (`0.y.z-preview`)**:
  - `0.MINOR.0-preview`: Significant functional additions, new MCP tool instruments, or internal architecture refactoring.
  - `0.MINOR.PATCH-preview`: Bugfixes, error handling hardening, binary metadata updates, or CIL memory optimizations.
- **Stable Phase (`v1.0.0+`)**:
  - `MAJOR`: Breaking changes to JSON-RPC MCP wire protocol or public API contracts in `Sharphy.Core`.
  - `MINOR`: New MCP tools, new CLI options, or backward-compatible feature additions.
  - `PATCH`: Backward-compatible bugfixes and internal enhancements.

---

## 2. Tri-Pillar Change Impact Detection Matrix

At the conclusion of each development cycle, evaluate the change impact:
- **Pillar 1 (`src/Sharphy.Core/**`, `src/Sharphy.Server/**`, `src/Sharphy.Cli/**`)**: Requires Git Tag + NuGet Package Publish (`publish-packages.yml`).
- **Pillar 2 (`docs/**`, `assets/**`)**: Requires GitHub Pages Deployment (`deploy-docs.yml`).
- **Pillar 3 (Root `*.md`, `.agents/**`, `.github/**`)**: Synchronize `CHANGELOG.md`; no binary bump required.

---

## 3. Distributed Components Matrix
1. **`Sharphy.Cli`**: .NET Global Tool executable (`sharphy mcp start`).
2. **`Sharphy.Server`**: MCP Server protocol library.
3. **`Sharphy.Core`**: Diagnostics, metadata, and CIL mutation library.
4. **`docs/`**: Next.js static documentation site hosted on GitHub Pages.

---

## 4. Pre-Release Verification & Execution SOP
1. Execute multi-targeting builds across .NET 8, 9, and 10.
2. Execute full xUnit test suite.
3. Verify stdio channel purity (`stdout` exclusively for JSON-RPC 2.0).
4. Run dry-run `dotnet pack` across all three projects.
5. Repository Owner executes `git tag -a vX.Y.Z -m "..."` and pushes to origin.
6. Trigger GitHub Actions workflows accordingly.

</details>
