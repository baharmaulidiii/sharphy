# Agent Coding Rules & Standards for Sharphy

This document outlines strict coding standards and runtime rules that every AI Coding Agent must follow when modifying or contributing code to this repository.

---

## 1. C# & Framework Standards

1. **C# Language Version:** Always target `latest` C# language features supported by the active compiler toolset.
2. **Nullable Reference Types:** Nullable context is strictly enabled (`<Nullable>enable</Nullable>`). Avoid null-forgiving operators (`null!`) unless interacting with legacy COM or unmanaged P/Invoke signatures where explicitly justified.
3. **Multi-Targeting Guards:**
   - When leveraging new APIs available only in .NET 9 or .NET 10 (e.g., expanded `UnsafeAccessor` signatures or new runtime telemetry), apply conditional compilation guards:
     ```csharp
     #if NET10_0_OR_GREATER
     // .NET 10 specific optimization
     #elif NET9_0_OR_GREATER
     // .NET 9 specific implementation
     #else
     // .NET 8 LTS fallback
     #endif
     ```
4. **Performance & Memory Allocations:**
   - Prefer `ReadOnlySpan<byte>`, `Memory<T>`, and `ref struct` types when parsing raw binary metadata to minimize Garbage Collector pressure.
   - Prevent native and unmanaged memory leaks during dump inspections: always wrap `DataTarget` and `ClrRuntime` instances in `using` declarations or explicitly dispose of them.

---

## 2. MCP Server Protocol Standards (`Sharphy.Server`)

1. **Protocol Compliance:** Adhere strictly to the official Model Context Protocol (JSON-RPC 2.0) specifications.
2. **Resilient Error Handling:** An MCP tool must never crash the server process if a target binary is malformed, corrupted, or heavily obfuscated. Catch exceptions cleanly and return structured diagnostic error messages within the MCP tool response.
3. **Transport Channel Purity:** When utilizing `stdio` transport, the standard output stream (`stdout`) is reserved exclusively for JSON-RPC messages. All internal diagnostic logs, warnings, and traces must be written to standard error (`stderr` via `Console.Error.WriteLine`).

---

## 3. Isolated Assembly Loading & Sandboxing

1. When analyzing, inspecting, or testing external assemblies (`samples/CrackmeDemo` or third-party targets), execute reflection within a collectible `AssemblyLoadContext`:
   ```csharp
   var alc = new AssemblyLoadContext("SharphyIsolatedInspectContext", isCollectible: true);
   ```
2. Unload the context once inspection completes to release file locks on disk and prevent memory leaks.
