## 📝 Change Description
<!-- Provide a concise summary of the changes, architectural motivation, and the problem solved by this PR. -->

## 🔗 Related Issue
<!-- Link related issue, e.g., Closes #12 or Fixes #34 -->
Closes #

---

## 🏷️ Type of Change
<!-- Select all applicable options with an [x] -->
- [ ] 🐛 **Bug fix** (non-breaking change which fixes an issue)
- [ ] ✨ **New feature** (non-breaking change which adds functionality)
- [ ] ⚡ **Performance optimization** (memory allocation, GC pressure, or MCP latency improvements)
- [ ] 💥 **Breaking change** (fix or feature that would cause existing functionality to not work as expected)
- [ ] 📚 **Documentation** (updates to documentation or markdown files)
- [ ] 🧪 **Tests / CI** (new unit tests, integration tests, or workflow modifications)

---

## ✅ Engineering Checklist
<!-- Ensure all items have been fulfilled prior to opening the PR -->
- [ ] **Multi-Targeting Clean Build:** Solution compiles cleanly across all target frameworks (`net8.0`, `net9.0`, `net10.0`) with 0 Warnings and 0 Errors (`dotnet build Sharphy.sln`).
- [ ] **Unit Tests:** 100% of tests pass cleanly (`dotnet test Sharphy.sln`).
- [ ] **Unmanaged Memory Discipline:** Native runtime diagnostic instances (ClrMD `DataTarget`, `ClrRuntime`) are properly wrapped in `using` declarations or disposed deterministically.
- [ ] **Sandbox Isolation:** Untrusted binary inspections execute inside a collectible `AssemblyLoadContext` (`isCollectible: true`) and are unloaded immediately upon completion.
- [ ] **MCP Stdio Purity:** Standard output (`stdout`) is strictly reserved for JSON-RPC payloads; all diagnostic logs are directed to `stderr`.
- [ ] **English Purity:** All code comments, XML documentation, and markdown files are written in pure English.
- [ ] **DCO 1.1 Sign-Off:** Commits are signed off (`git commit -s`).
