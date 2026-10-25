# Release Governance, Packaging & Deployment Guidelines for Sharphy

> **Official Release Governance & Artifact Publication Protocols**  
> Operational standard for Repository Owner and automated AI Coding Agents.

This document defines official rules for version lifecycles, NuGet package publishing, semantic versioning (SemVer), and GitHub Pages documentation deployments.

---

## 1. Versioning Strategy (SemVer)

Sharphy strictly adheres to **[Semantic Versioning 2.0.0](https://semver.org/)**:
```text
MAJOR.MINOR.PATCH
```

### 1.1 Stable Production Releases (`1.y.z`)
- **`MAJOR`**: Breaking architectural changes, protocol alterations, or tool schema removals.
- **`MINOR`**: Adding new MCP tools, expanding diagnostic engines, or substantial capabilities with backward compatibility.
- **`PATCH`**: Bug fixes, performance optimizations, or documentation/legal enhancements without tool interface changes.

---

## 2. Tri-Pillar Change Impact Detection Matrix

Every development cycle must evaluate affected files against the following impact matrix:

| Change Dimension | File Indicators / Paths | Triggered Action | Publication Target |
| :--- | :--- | :--- | :--- |
| **Pillar 1: Core Engines & MCP Server** | `src/Sharphy.Core/**`<br>`src/Sharphy.Server/**`<br>`src/Sharphy.Cli/**`<br>`Directory.Build.props` | Requires new Git Tag (`vX.Y.Z`) and NuGet publication | NuGet.org (`Sharphy.Cli`, `Sharphy.Server`, `Sharphy.Core`) |
| **Pillar 2: Web Documentation Portal** | `docs/**`<br>`assets/**` | Requires GitHub Pages deployment trigger | GitHub Pages (`baharmaulidiii.github.io/sharphy`) |
| **Pillar 3: Project Specifications** | `*.md` at root<br>`.agents/**`<br>`.github/**` | Internal documentation synchronization | Commit and push to `main` branch |

---

## 3. Package Ecosystem Composition

Sharphy distributes three NuGet packages generated from the root solution:
1. **`Sharphy.Cli` (Primary Global Tool):**
   - Executable wrapper (`PackAsTool=true`, command: `sharphy`).
   - Consumer: AI Agent runners (Claude Code, Cursor, Windsurf) and terminal users.
2. **`Sharphy.Server` (Class Library):**
   - Protocol host implementing JSON-RPC 2.0 over Stdio and SSE.
   - Consumer: Developers embedding Sharphy into custom host runtimes.
3. **`Sharphy.Core` (Class Library):**
   - Analytical engine wrapping ClrMD, ILSpy, Mono.Cecil, and Roslyn.
   - Consumer: Standalone CLR diagnostic and decompilation tools.

---

## 4. Changelog Governance (`CHANGELOG.md`)

1. Every unreleased modification must be recorded under `## [Unreleased]`.
2. Categorize items into:
   - `### Added` for new capabilities or tools.
   - `### Changed` for modifications to existing behavior.
   - `### Fixed` for bug fixes.
   - `### Security` for vulnerability patches and threat modeling updates.
3. Upon publishing a new release, convert the unreleased section to `## [X.Y.Z] - YYYY-MM-DD`.

---

## 5. Pre-Release Verification Checklist

Prior to tagging or publishing a release, verify:
```powershell
# 1. Clean multi-targeting compilation across .NET 8, 9, 10
dotnet build Sharphy.sln -c Debug

# 2. Verify channel purity: ensure no console writes to stdout
# Only stderr is permitted for internal logging in Sharphy.Server

# 3. Comprehensive unit test pass
dotnet test Sharphy.sln

# 4. Packaging dry-run validation
dotnet pack Sharphy.sln -c Release
```

---

## 6. Release Execution Protocols

- **AI Agents:** Permitted to execute local verification, build checks, and packaging dry-runs. **Strictly forbidden from running git commit, git push, or creating remote git tags.**
- **Repository Owner:** Authorized to commit changes and trigger the automated **Release Orchestrator** workflow via GitHub Actions (`workflow_dispatch`).
