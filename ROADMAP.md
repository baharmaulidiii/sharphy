# Technical Roadmap for Sharphy

This document outlines the strategic engineering roadmap and upcoming milestones for the **Sharphy** CLR Intelligence & Runtime Forensic Bridge ecosystem.

---

## 1. Release Milestone History

### Milestone 1.1 (Active CLR Diagnostics & Advanced Bytecode Forensics) - COMPLETED
- [x] SOS-style ThreadPool starvation diagnostics detecting sync-over-async blockages (`sharphy_analyze_dump`).
- [x] Automated heap fragmentation analysis and LOH `HandleKind.Pinned` GC compaction risk telemetry (`sharphy_inspect_heap`).
- [x] Unified line-by-line CIL instruction diffing (`---`, `+++`, `@@`, `+`, `-`) for AI coding agents (`sharphy_diff_assemblies`).
- [x] Pattern-matching string decryptor heuristic resolving obfuscated runtime loops (`sharphy_clean_obfuscation`).
- [x] Expansion of xUnit v3 automated testing suite to 14 verified multi-targeting tests across .NET 8.0 and .NET 10.0.

### Milestone 1.0 (General Availability & Core Engine Stabilization) - COMPLETED
- [x] Complete transition from preview (`0.1.x`) to General Availability (`1.0.0`).
- [x] Integrate `ICSharpCode.Decompiler` (ILSpy) for high-fidelity CIL method decompilation.
- [x] Integrate Microsoft ClrMD for memory crash dump forensics, heap inspections, and Monitor lock cycle detection.
- [x] Integrate `Mono.Cecil` for ECMA-335 metadata parsing and bytecode patching.
- [x] Implement collectible `AssemblyLoadContext` (`isCollectible: true`) sandboxing.
- [x] Standardize 10-tool Model Context Protocol (MCP) server catalog over Stdio JSON-RPC 2.0.
- [x] Deploy unified Next.js documentation portal on GitHub Pages.
- [x] Formalize 5-Pillar Legal Architecture: Apache License 2.0, sole copyright protection under Bahar Maulidi, Trademark Policy, DCO 1.1, and hardened AI safety disclaimers.

---

## 2. Near-Term Milestones (v1.2.x)

### Advanced Memory Telemetry & Native Heaps
- [ ] Native P/Invoke heap allocation profiling alongside managed GC tracking.
- [ ] Cross-platform SOS extension command bridge for native LLDB/WinDbg command passes.

### Live Telemetry & Transport Expansion
- [ ] Bidirectional Server-Sent Events (SSE) and HTTP stream transport for remote container diagnostics.
- [ ] EventPipe IPC streaming for live .NET processes on Linux and Windows.

---

## 3. Long-Term Architectural Horizon (v2.0.x)

### Remote Runtime Live Profiling
- [ ] EventPipe and DiagnosticsClient IPC streaming for live .NET processes on Linux and Windows.
- [ ] Dynamic tracing of JIT compilation events, assembly loads, and first-chance exception telemetry.
- [ ] Native AOT (Ahead-of-Time) metadata and reflection mapping analyzer.

### Multi-Agent Autonomous Swarms
- [ ] Bidirectional Server-Sent Events (SSE) transport for remote cloud container agent swarms.
- [ ] Interactive stateful debugging sessions with breakpoint hooks for LLM diagnostic loops.
