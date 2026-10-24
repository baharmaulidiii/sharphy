# Testing Policy for AI Agents

This document defines testing standards and verification requirements that AI Agents must fulfill prior to completing any development task.

---

## 1. Testing Scopes

1. **Unit Testing (`tests/Sharphy.Tests`):**
   - Verify isolated unit logic within `Sharphy.Core` (metadata parsing, IL manipulation helpers, heap statistics calculators).
   - Ensure `Sharphy.Server` tool dispatchers serialize and deserialize JSON-RPC requests conforming to the MCP schema.
2. **Sample Testbed (`samples/CrackmeDemo`):**
   - Target project designed to validate decompilation accuracy, memory snapshot forensic ingestion, and safe CIL instruction mutations.

---

## 2. Verification Checklist for Agents

Before concluding an operational cycle and handing over changes for the human owner to review/commit, the Agent **must** verify:
- [ ] `dotnet build Sharphy.sln` compiles cleanly with 0 errors and 0 new warnings across supported frameworks.
- [ ] `dotnet test Sharphy.sln` passes all test cases.
- [ ] Documentation files ([README.md](README.md) and [README.en.md](README.en.md)) reflect updated tool listings when adding or modifying MCP tools.
