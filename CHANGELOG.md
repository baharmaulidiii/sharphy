# Changelog

All notable changes to the **Sharphy** project are documented in this file.

The format is based on **[Keep a Changelog](https://keepachangelog.com/en/1.1.0/)**, and this project adheres to **[Semantic Versioning (SemVer)](https://semver.org/spec/v2.0.0.html)**.

---

## [Unreleased]

### Added
- **Glama.ai Registry Metadata (`glama.json`):** Added official maintainer metadata schema declaring repository maintainer provenance for Glama MCP Registry verification.
- **Glama.ai MCP Registry Integration:** Added official Glama MCP server listing badge and metadata for discovery in the open-source MCP ecosystem.
- **Containerization Support (`Dockerfile`):** Implemented multi-stage container build targeting `.NET 9.0` with a chiseled minimal runtime for containerized deployments and automated checks.

---

## [1.0.4] - 2026-10-25

### Added
- **5-Pillar Legal Architecture Finalization:** Migrated primary open-source licensing to **Apache License, Version 2.0** providing express bilateral patent grants and patent retaliation protections for sole creator and copyright owner **Moh. Baharudin Yuda Maulidi**.
- **Official Trademark Policy (`TRADEMARK.md`):** Formalized guidelines protecting the "Sharphy" brand name, package namespaces, and visual vector logo (`assets/sharphy.svg`) against unauthorized derivative distributions and malicious impersonations.
- **Hardened Legal Disclaimer & AI Safety Shield (`DISCLAIMER.md`):** Established complete liability immunity for dual-use technology, explicit waivers for autonomous AI agent operations via Model Context Protocol (MCP), and user indemnification obligations.
- **Developer Certificate of Origin (`CONTRIBUTING.md`):** Integrated DCO 1.1 requirements with mandatory commit sign-offs (`git commit -s`) to guarantee provenance and license compliance of external contributions.

### Changed
- **End-to-End English Globalization:** Standardized 100% pure English across all repository documentation, Next.js web portal, C# engine XML doc comments, inline code comments, and GitHub issue/PR templates, eliminating legacy bilingual `<details>` collapses.
- **Documentation Web Portal Refresh (`docs/`):** Streamlined Next.js documentation portal to pure English (`en_US`), removed legacy language toggle switches, updated navigation bar version badge to `v1.0.4`, and added direct links to `TRADEMARK.md` and `DISCLAIMER.md`.

---

## [1.0.3] - 2026-10-25

### Added
- **Real Binary Diffing Engine (`BinaryDiffEngine`):** Real ECMA-335 metadata comparison between compiled assemblies to detect added/removed types, breaking API changes, and CIL bytecode size divergence.
- **Obfuscation & Control Flow Engine (`ObfuscationAnalyzerEngine`):** Demangling of Control Flow Graph (CFG) switch flattening, dead code / excessive NOP trimming, and dynamic proxy delegate analysis (`calli` / `MethodBase.Invoke`).
- **Complete MCP Catalog Integration:** Connected `sharphy_diff_assemblies` and `sharphy_clean_obfuscation` to production engines, replacing stub implementations.
- **Multi-Target Real Engine Tests:** Automated verification in `RealEngineTests` covering `BinaryDiffEngine` and `ObfuscationAnalyzerEngine`.

---

## [1.0.2] - 2026-10-25

### Added
- **Dedicated NuGet Gallery Readme (`PACKAGE_README.md`):** Created a standalone, concise English markdown file optimized for clean rendering on NuGet.org without complex HTML tags.
- **Self-Contained CLI Distribution:** Verified that `Sharphy.Cli` packages all required `Sharphy.Core` and `Sharphy.Server` engine dependencies as a single global tool.

---

## [1.0.1] - 2026-10-25

### Added
- **Visual Branding for NuGet Packages:** Configured `<PackageIcon>` in `Directory.Build.props` pointing to `assets/sharphy.png` across all packages (`Sharphy.Cli`, `Sharphy.Core`, `Sharphy.Server`).
- **Package Readme Integration:** Embedded `PACKAGE_README.md` into package metadata to eliminate NuGet missing readme warnings.

### Fixed
- **Automated NuGet Publishing via OpenID Connect (OIDC):** Replaced static API keys with trusted publishing using `NuGet/login@v1`.
- **Manual Release Orchestrator Control:** Refined `workflow_dispatch` pipeline to ensure precise, sequential package publishing and deployment.

---

## [1.0.0] - 2026-10-24

### Added
- **General Availability (v1.0.0 Stable Release):** Promoted codebase from preview to stable release line.
- **Modern C# Decompiler Engine (`DecompilerEngine`):** Integrated `ICSharpCode.Decompiler` (ILSpy) for method decompilation via `sharphy_decompile_method`.
- **Crash Dump & Memory Forensics Engine (`DumpForensicsEngine`):** Integrated Microsoft ClrMD for `sharphy_analyze_dump`, `sharphy_inspect_heap`, and `sharphy_deadlock_detector`.
- **ECMA-335 Metadata & CIL Patcher (`MetadataEngine`, `IlPatcherEngine`):** Integrated `Mono.Cecil` for low-level metadata inspection and bytecode modification.
- **Collectible Sandbox Engine (`SandboxEngine`):** Dynamic assembly reflection inside collectible `AssemblyLoadContext` (`isCollectible: true`).
- **Zero-Overhead Wrapper Generator (`WrapperGeneratorEngine`):** High-performance private member accessors using `.NET 8+` `[UnsafeAccessor]`.
- **Comprehensive Test Suite (`Sharphy.Tests`):** Multi-targeting unit and integration test coverage across .NET 8, 9, and 10.
