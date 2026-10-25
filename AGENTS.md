# AGENTS.md - AI Coding Agent Operational Blueprint for Sharphy

> **Sharphy: CLR Intelligence & Runtime Forensic Bridge**  
> A native Model Context Protocol (MCP) server turning AI coding agents into deep .NET runtime diagnosticians, memory forensic analysts, and binary architects.

This document serves as the official operational instructions for AI Coding Agents (Cursor, Claude Code, Windsurf, Copilot, Antigravity) working within the **Sharphy** repository.

---

## 1. Core Principles & Agent Role

Every Agent operating in this repository acts as an **Autonomous CLR Engineer**:
1. **Metadata Preservation & CIL Standards:** Prioritize ECMA-335 metadata, low-level CIL instructions, SOS/ClrMD runtime inspection, and Roslyn compiler APIs over generic text heuristics.
2. **Multi-Targeting Awareness:** All code within `src/Sharphy.Core`, `src/Sharphy.Server`, and `src/Sharphy.Cli` must support multi-targeting across `.NET 8.0`, `.NET 9.0`, and `.NET 10.0`. Use conditional compilation preprocessor directives (`#if NET10_0_OR_GREATER`, `#if NET9_0_OR_GREATER`) whenever leveraging newer BCL/CLR features.
3. **Safety & Sandboxing:** Binary unpacking and dynamic assembly inspection must execute within an isolated, collectible `AssemblyLoadContext` (`isCollectible: true`) to avoid polluting the host runtime process or locking files on disk.
4. **Repository Language vs. Interaction Language Boundary:**
   - **Repository Artifacts (100% Pure English):** All codebase files, C# XML documentation, inline code comments, git commit messages, GitHub templates, release notes, and documentation markdown files across the entire repository MUST be written exclusively in standard, idiomatic English (`en-US`). No other language or bilingual collapsible tags are permitted inside repository artifacts.
   - **Human-to-Agent Interaction (Bahasa Indonesia):** When communicating with the Repository Owner (Bahar Maulidi), the Agent MUST converse, provide reasoning updates, and present the final `Laporan Evaluasi Rilis & Deployment` in **Bahasa Indonesia**.
5. **Absolute Git Commit & Push Prohibition:** Agents are strictly forbidden from executing `git commit` or `git push` under any circumstances. Only the repository owner/human user is authorized to perform git commit and push operations. Agents must leave all modifications uncommitted in the local working directory.

---

## 2. Repository Structure & Boundary Responsibilities

```text
sharphy/
├── .agents/                    # Agent workflow definitions & skill guidelines
├── src/
│   ├── Sharphy.Core/           # Core analysis engines:
│   │                           # - Diagnostics (ClrMD, SOS)
│   │                           # - Decompilation (ILSpy engine)
│   │                           # - Bytecode & Metadata (Mono.Cecil, dnlib, Roslyn)
│   ├── Sharphy.Server/         # Model Context Protocol (MCP) handlers:
│   │                           # - Stdio / SSE Transport
│   │                           # - Tool & Resource dispatchers
│   └── Sharphy.Cli/            # CLI executable & global dotnet tool wrapper
├── samples/
│   └── CrackmeDemo/            # Isolated testbed binary for feature validation
├── tests/
│   └── Sharphy.Tests/          # Multi-target unit and integration testing suite
├── Directory.Build.props       # Global properties, nullable checks, target frameworks
└── Sharphy.sln                 # Root solution file
```

---

## 3. Agent Development Workflow

```
[Agent Trigger]
       │
       ▼
1. Research & Inspect  ───> Review Directory.Build.props & project architecture
       │
       ▼
2. Implementation      ───> Write pure English code/comments in Sharphy.Core/Server
       │
       ▼
3. Multi-Target Verify ───> Run: dotnet build Sharphy.sln
       │
       ▼
4. Test Suite          ───> Run: dotnet test Sharphy.sln
       │
       ▼
5. Packaging Dry-Run   ───> Validate nupkg generation via dotnet pack
       │
       ▼
6. Impact & Docs Sync  ───> Evaluate Tri-Pillar Matrix, update CHANGELOG.md,
                            and output standard Release Assessment Report
```

---

## 4. Standard CLI Commands for Agents

Agents should use the following commands during iteration cycles:

```powershell
# Restore dependencies
dotnet restore Sharphy.sln

# Build solution in Debug configuration
dotnet build Sharphy.sln -c Debug

# Execute test suite across all targeted frameworks
dotnet test Sharphy.sln --no-build

# Run local CLI tool
dotnet run --project src/Sharphy.Cli/Sharphy.Cli.csproj --framework net8.0 -- mcp start
```

---

## 5. The `.agents/` Directory

The [.agents](.agents) directory contains automated workflow rules and tool schemas:
- `.agents/rules.md`: Coding standards, memory safety, and error handling policies.
- `.agents/release-guidelines.md`: Official release governance, SemVer policies, package publication, and GitHub Pages deployment rules.
- `.agents/mcp-schema.json`: Formal JSON schema definitions for MCP tools implemented by `Sharphy.Server`.
- `.agents/testing-policy.md`: Testing guidelines, memory dump validation, and testbed procedures.

---

## 6. Post-Development Release Assessment Protocol

Every AI Coding Agent completing work in this repository is strictly required to execute a **Change Impact Detection** evaluation against the last published Git tag and present the standardized report in **Bahasa Indonesia** in its concluding response:

### 6.1 Tri-Pillar Change Impact Matrix
1. **Core & Server Engine (`src/Sharphy.Core/**`, `src/Sharphy.Server/**`, `src/Sharphy.Cli/**`):** Requires a new Git tag and NuGet deployment (`publish-packages.yml`).
2. **Documentation Portal (`docs/**`, `assets/**`):** Requires manual trigger of the GitHub Pages deployment workflow (`deploy-docs.yml`).
3. **Repository Specifications (`*.md` at root, `.agents/**`):** Requires synchronizing `CHANGELOG.md` under `[Belum Dirilis] - Unreleased` or the designated release section.

### 6.2 Mandatory Concluding Output Template (Bahasa Indonesia)
```markdown
### 📋 Laporan Evaluasi Rilis & Deployment (Agent Release Assessment)
- **Status Perubahan sejak Rilis Terakhir (<latest_tag>):** [Summary of changed paths]
- **Kebutuhan Rilis Git Baru:** [YA / TIDAK] -> Rekomendasi Versi: <vX.Y.Z>
- **Kebutuhan Deploy Paket NuGet:** [YA / TIDAK] -> Paket: (Sharphy.Cli, Sharphy.Server, Sharphy.Core)
- **Kebutuhan Deploy GitHub Pages:** [YA / TIDAK] -> Direktori: (docs/)
- **Verifikasi Lokal:**
  - Build multi-targeting (.NET 8/9/10): BERHASIL
  - Protocol & Unit Tests: 100% LOLOS
  - Validasi Packaging (dotnet pack): VALID
- **Instruksi Eksekusi untuk Repo Owner:**
  ```powershell
  # 1. Commit and push changes
  git add .
  git commit -m "..."
  git push origin main

  # 2. Tag and publish packages (Automated via GitHub Actions)
  # Go to GitHub Actions -> Release Orchestrator -> Run workflow (Input version, e.g., 1.0.4)
  # GitHub Actions automatically packs, publishes to NuGet, and generates the git tag & release.

  # 3. Deploy documentation portal (if required)
  # Trigger GitHub Pages workflow via GitHub Web UI
  ```
```
