# Contributing Guidelines for Sharphy

This document outlines mandatory engineering standards, low-level architecture policies, and contribution guidelines for human engineers and AI coding agents operating on the **Sharphy** repository.

---

## 1. Core Engineering Principles

As a specialized bridge handling internal virtual machine memory forensics and CIL bytecode manipulation, every contribution must satisfy four fundamental engineering pillars:

### 1.1 Unmanaged Memory Discipline & Leak Prevention
- Low-level diagnostic components (such as Microsoft ClrMD `DataTarget`, `ClrRuntime`, and `ClrHeap`) allocate unmanaged native resources outside regular GC tracking.
- Every ClrMD instance **must** be enclosed in a `using` declaration or deterministically released via `.Dispose()`.
- Use `ReadOnlySpan<byte>`, `ReadOnlyMemory<byte>`, and `ref struct` types when parsing binary PE/COFF metadata headers to maintain zero-allocation runtime performance.

### 1.2 Binary Isolation & Sandboxed Execution
- Never load untrusted third-party assemblies directly into the host process default execution context (`AssemblyLoadContext.Default`).
- All dynamic inspection and token extraction must execute within an isolated collectible `AssemblyLoadContext`:
  ```csharp
  var isolatedContext = new AssemblyLoadContext("SharphySandbox", isCollectible: true);
  try
  {
      // Execute isolated reflection or evaluation here
  }
  finally
  {
      isolatedContext.Unload();
  }
  ```
- Ensure context unloading completes cleanly to eliminate persistent disk file locks and host memory pollution.

### 1.3 Multi-Targeting Architecture (.NET 8, 9, 10)
- All codebase modifications in `Sharphy.Core`, `Sharphy.Server`, and `Sharphy.Cli` must compile with 0 warnings and 0 errors across `.NET 8.0 (LTS)`, `.NET 9.0 (STS)`, and `.NET 10.0 (Latest)`.
- Apply conditional compilation preprocessor guards when leveraging newer BCL or runtime features:
  ```csharp
  #if NET10_0_OR_GREATER
  // .NET 10 high-performance optimizations
  #elif NET9_0_OR_GREATER
  // .NET 9 specific implementations
  #else
  // .NET 8 LTS baseline fallback
  #endif
  ```

### 1.4 Transport Channel Purity (Model Context Protocol)
- For the `Sharphy.Server` project, the standard output stream (`Console.Out`) is dedicated exclusively to transmitting JSON-RPC 2.0 payloads.
- Never write diagnostic logs, print statements, or unhandled exceptions to `stdout`. All instrumentation, telemetry, and debugging output must be routed to `stderr` (`Console.Error.WriteLine`).

---

## 2. Local Development Environment

### Prerequisites
1. [.NET SDK 8.0+](https://dotnet.microsoft.com/download) (installing SDK 8.0 and 10.0 is recommended to test the full multi-targeting matrix).
2. Git CLI version 2.40 or newer.

### Standard Commands
```powershell
# Restore package dependencies
dotnet restore Sharphy.sln

# Build solution in Debug configuration
dotnet build Sharphy.sln -c Debug

# Run the full unit testing suite
dotnet test Sharphy.sln

# Run the local MCP server via CLI
dotnet run --project src/Sharphy.Cli/Sharphy.Cli.csproj --framework net8.0 -- mcp start
```

---

## 3. Pull Request & Contribution Workflow

1. **Dedicated Branching:**
   - Create branches off `main` with descriptive prefixes:
     * `feat/feature-name` for new MCP tools or diagnostic engines.
     * `fix/bug-description` for bug fixes and anomaly resolutions.
     * `perf/allocation-optimization` for memory or latency improvements.
2. **Test Coverage:**
   - Every new engine or tool must include unit tests in `tests/Sharphy.Tests` based on `xunit.v3`.
   - Ensure builds yield **0 Warnings** and **0 Errors**.
3. **Agent Operational Boundaries:**
   - AI Coding Agents operating in this repository are strictly forbidden from executing `git commit` or `git push`.
   - Only the repository owner/human user is authorized to review and commit changes to Git history.

---

## 4. Developer Certificate of Origin (DCO 1.1)

To provide certainty of project ownership and defend intellectual property integrity, this repository enforces the **Developer Certificate of Origin (DCO) Version 1.1** (the industry standard utilized by the Linux Kernel and CNCF).

By contributing code, documentation, or pull requests to this project, you certify that:

```text
Developer Certificate of Origin
Version 1.1

By making a contribution to this project, I certify that:

(a) The contribution was created in whole or in part by me and I
    have the right to submit it under the open source license
    indicated in the file; or

(b) The contribution is based upon previous work that, to the best
    of my knowledge, is covered under an appropriate open source
    license and I have the right under that license to submit that
    work with modifications, whether created in whole or in part
    by me, under the same open source license (unless I am
    permitted to submit under a different license), as indicated
    in the file; or

(c) The contribution was provided directly to me by some other
    person who certified (a), (b) or (c) and I have not modified
    it.

(d) I understand and agree that this project and the contribution
    are public and that a record of the contribution (including all
    personal information I submit with it, including my sign-off) is
    maintained indefinitely and may be redistributed consistent with
    this project or the open source license(s) involved.
```

### Commit Sign-Off Requirement
All human contributors must append a sign-off line to commit messages:
```bash
git commit -s -m "feat: add ECMA-335 token filter"
```
This produces an explicit `Signed-off-by: Full Name <email@example.com>` signature. All accepted contributions are permanently licensed under the **Apache License 2.0** administered by **Bahar Maulidi**.
