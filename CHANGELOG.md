# Catatan Perubahan (Changelog)

Seluruh perubahan penting pada proyek **Sharphy** didokumentasikan secara kronologis dalam berkas ini.

Format dokumen ini mengacu pada standar **[Keep a Changelog](https://keepachangelog.com/id/1.1.0/)**, dan penomoran versi proyek ini tunduk pada aturan **[Semantic Versioning (SemVer)](https://semver.org/lang/id/)**.

---

## [Belum Dirilis] - Unreleased

### Ditambahkan (Added)
- **Standar Komunitas GitHub & Pedoman Perilaku (`CODE_OF_CONDUCT.md`):** Pengadopsian Contributor Covenant v2.1 resmi dalam format dwibahasa (Bahasa Indonesia dan Bahasa Inggris).
- **Templat Isu GitHub Interaktif (`.github/ISSUE_TEMPLATE/`):** Formulir GitHub Issue Forms untuk pelaporan bug (`bug_report.yml`), usulan fitur (`feature_request.yml`), dan konfigurasi rujukan keamanan (`config.yml`).
- **Templat Pull Request Terstandarisasi (`.github/pull_request_template.md`):** Templat pengajuan PR dengan daftar periksa kepatuhan arsitektur (multi-targeting, kebersihan memori unmanaged ClrMD, sandboxing, kemurnian stdio MCP).
- **Pernyataan Aksesibilitas (`ACCESSIBILITY.md`):** Komitmen kepatuhan WCAG 2.1 AA dan desain CLI ramah pembaca layar (*screen reader*).

### Direncanakan (Planned)
- Implementasi penuh engine dekompilasi C# modern memanfaatkan `ICSharpCode.Decompiler` (ILSpy) pada tool `sharphy_decompile_method`.
- Integrasi pustaka `Microsoft.Diagnostics.Runtime` (ClrMD) untuk audit managed heap dan analisis crash dump pada tool `sharphy_analyze_dump`, `sharphy_inspect_heap`, dan `sharphy_deadlock_detector`.
- Integrasi engine `Mono.Cecil` untuk re-emit biner pada tool `sharphy_patch_il` dan deobfuscation pada `sharphy_clean_obfuscation`.
- Pembuatan generator interop performa tinggi menggunakan atribut `[UnsafeAccessor]` (.NET 8+) pada tool `sharphy_generate_wrapper`.

---

## [0.1.1-preview] - 2026-10-24

### Ditambahkan (Added)
- **Pedoman Manajemen Perilisan & Deployment (`.agents/release-guidelines.md`):** Penetapan kebijakan resmi penomoran versi SemVer 2.0.0, matriks deteksi perubahan tri-pilar (Core/Server, Docs, Metadata), dan SOP eksekusi rilis serta deployment portal dokumentasi.
- **Protokol Deteksi Rilis untuk AI Agent:** Aturan baku pada `AGENTS.md` dan `.agents/rules.md` yang mewajibkan seluruh AI Coding Agent melakukan audit dampak perubahan dan menyajikan laporan evaluasi rilis standar di akhir setiap siklus pengerjaan.
- **Penyelarasan Versi CLI Runner:** Pembaruan string versi `Sharphy.Cli --version` menjadi `0.1.1-preview`.

---

## [0.1.0-preview] - 2026-10-24

### Ditambahkan (Added)
- **Fondasi Server Protokol MCP:** Implementasi server native Model Context Protocol (MCP) berbasis JSON-RPC 2.0 melalui kanal Stdio murni pada proyek `Sharphy.Server`.
- **Katalog 10 MCP Tools Forensik:** Registrasi katalog terpusat pada `ToolRegistry` yang mencakup 10 instrumen diagnostik runtime, dekompilasi, mutasi CIL, dan forensik memori.
- **Utilitas Baris Perintah Global (`Sharphy.Cli`):** Perintah `sharphy mcp start` untuk menjalankan server host MCP secara instan, dilengkapi dukungan perintah `--version` dan `--help`.
- **Dukungan Multi-Targeting Native:** Konfigurasi multi-targeting lintas kerangka kerja modern `.NET 8.0 (LTS)`, `.NET 9.0 (STS)`, dan `.NET 10.0 (Latest)`.
- **Suite Pengujian Unit MCP:** Verifikasi otomatis handshake protokol `initialize`, penemuan katalog 10 tools (`tools/list`), dan dispatch pemanggilan tool (`tools/call`) berbasis `xunit.v3`.
- **Dokumentasi Human-Facing Baku & Eksplisit:** Berkas `README.md`, `CONTRIBUTING.md`, `SECURITY.md`, `DISCLAIMER.md`, `THIRD_PARTY_NOTICE.md`, dan panduan promosi `.agents/promotion/launch-kit.md` dengan Bahasa Indonesia formal sebagai teks utama dan terjemahan Bahasa Inggris terpadu.

### Keamanan (Security)
- Penerapan aturan kemurnian kanal Stdio (`stdout` khusus untuk payload JSON-RPC 2.0, sedangkan seluruh log diagnostik internal dialihkan ke `stderr`).
- Kebijakan isolasi eksekusi refleksi dinamis wajib menggunakan `AssemblyLoadContext` kolektibel (`isCollectible: true`) untuk mencegah kebocoran memori atau penguncian berkas.
- Penegasan larangan mutlak eksekusi `git commit` dan `git push` bagi AI Coding Agent.

---

<details>
<summary><b>🇬🇧 English Translation (Click to expand)</b></summary>

# Changelog

All notable changes to the **Sharphy** project will be documented in this file.

The format is based on **[Keep a Changelog](https://keepachangelog.com/en/1.1.0/)**, and this project adheres to **[Semantic Versioning](https://semver.org/spec/v2.0.0.html)**.

---

## [Unreleased]

### Added
- **GitHub Community Standards & Code of Conduct (`CODE_OF_CONDUCT.md`):** Adopted Contributor Covenant v2.1 in unified bilingual format.
- **Interactive GitHub Issue Forms (`.github/ISSUE_TEMPLATE/`):** YAML Issue Forms for structured bug reports (`bug_report.yml`), feature requests (`feature_request.yml`), and contact routing (`config.yml`).
- **Standardized Pull Request Template (`.github/pull_request_template.md`):** PR template featuring engineering & CLR architecture compliance checklists (multi-targeting, ClrMD disposal, sandbox isolation, stdio protocol purity).
- **Accessibility Statement (`ACCESSIBILITY.md`):** Formal commitment to WCAG 2.1 AA and screen-reader-friendly CLI design.

### Planned
- Full C# decompilation engine integration using `ICSharpCode.Decompiler` (ILSpy) for `sharphy_decompile_method`.
- In-depth runtime forensics via `Microsoft.Diagnostics.Runtime` (ClrMD) for `sharphy_analyze_dump`, `sharphy_inspect_heap`, and `sharphy_deadlock_detector`.
- Bytecode manipulation and binary re-emitting via `Mono.Cecil` for `sharphy_patch_il` and `sharphy_clean_obfuscation`.
- Zero-overhead interop wrapper generation utilizing `[UnsafeAccessor]` (.NET 8+) for `sharphy_generate_wrapper`.

---

## [0.1.1-preview] - 2026-10-24

### Added
- **Release Management & Deployment Guidelines (`.agents/release-guidelines.md`):** Formal SemVer 2.0.0 policies, tri-pillar change impact detection matrix, and step-by-step SOPs for NuGet packages and GitHub Pages deployment.
- **AI Agent Lifecycle Protocols:** Operational mandates in `AGENTS.md` and `.agents/rules.md` requiring AI coding agents to perform automated impact assessments and generate standardized concluding reports.
- **CLI Runner Version Synchronization:** Aligned `Sharphy.Cli --version` response string to `0.1.1-preview`.

---

## [0.1.0-preview] - 2026-10-24

### Added
- **MCP Server Protocol Host:** Native Model Context Protocol (MCP) implementation over pure Stdio JSON-RPC 2.0 in `Sharphy.Server`.
- **10 Diagnostic MCP Tools Catalog:** Centralized dispatcher and metadata definitions in `ToolRegistry` covering dump forensics, heap inspection, decompilation, and CIL mutation.
- **Global CLI Tool (`Sharphy.Cli`):** Dedicated `sharphy mcp start` launcher command, along with `--version` and `--help` CLI options.
- **Native Multi-Targeting:** Multi-framework targeting support across `.NET 8.0 (LTS)`, `.NET 9.0 (STS)`, and `.NET 10.0 (Latest)`.
- **Protocol Unit Test Suite:** Automated protocol handshake, tool discovery, and execution verification powered by `xunit.v3`.
- **Authoritative Technical Documentation:** Rigid documentation suite (`README.md`, `CONTRIBUTING.md`, `SECURITY.md`, `DISCLAIMER.md`, `THIRD_PARTY_NOTICE.md`, and launch kit) with Indonesian primary prose and collapsible English translations.

### Security
- Stdio transport isolation: `stdout` reserved exclusively for JSON-RPC 2.0 frames; diagnostics routed to `stderr`.
- Sandboxing directive requiring collectible `AssemblyLoadContext` (`isCollectible: true`) for dynamic evaluations.
- Absolute prohibition against automated `git commit` and `git push` by AI coding agents.

</details>

