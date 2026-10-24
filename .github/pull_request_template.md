## 📝 Deskripsi Perubahan (Change Description)
<!-- Jelaskan ringkasan perubahan, motivasi, dan masalah yang diselesaikan oleh PR ini. -->

## 🔗 Issue Terkait (Related Issue)
<!-- Tautkan issue yang diselesaikan, contoh: Closes #12 atau Fixes #34 -->
Closes #

---

## 🏷️ Jenis Perubahan (Type of Change)
<!-- Centang salah satu atau lebih yang relevan dengan menandai [x] -->
- [ ] 🐛 **Perbaikan Bug** (*non-breaking change* yang memperbaiki kesalahan/anomali)
- [ ] ✨ **Fitur Baru** (*non-breaking change* yang menambahkan fungsionalitas baru)
- [ ] ⚡ **Optimasi Performa** (perbaikan alokasi memori, latensi MCP, dsb.)
- [ ] 💥 **Breaking Change** (perubahan yang dapat memengaruhi kompatibilitas ke belakang)
- [ ] 📚 **Dokumentasi** (pembaruan berkas dokumentasi atau markdown)
- [ ] 🧪 **Pengujian / CI** (penambahan unit test baru atau pembaruan alur GitHub Actions)

---

## ✅ Checklist Verifikasi Rekayasa (Engineering Checklist)
<!-- Pastikan seluruh item berikut telah divalidasi sebelum mengajukan PR -->
- [ ] **Multi-Targeting Clean Build:** Solusi berhasil dikompilasi pada seluruh target kerangka kerja (`net8.0`, `net9.0`, `net10.0`) dengan 0 Warning dan 0 Error (`dotnet build Sharphy.sln`).
- [ ] **Unit Tests:** Seluruh rangkaian pengujian lolos 100% (`dotnet test Sharphy.sln`).
- [ ] **Disiplin Memori Unmanaged:** Komponen diagnostik runtime native (ClrMD `DataTarget`, `ClrRuntime`, dsb.) dibungkus dalam deklarasi `using` deklaratif atau dilepas secara deterministik via `.Dispose()`.
- [ ] **Isolasi Sandbox:** Pemrosesan biner tidak tepercaya dieksekusi dalam `AssemblyLoadContext` kolektibel (`isCollectible: true`) dan dibongkar (*unloaded*) setelah selesai.
- [ ] **Kemurnian Stdio MCP:** Tidak ada pesan log atau jejak galat yang bocor ke `stdout`. Seluruh log diagnostik dialirkan ke `stderr`.
- [ ] **Dokumentasi Terintegrasi:** Berkas markdown diperbarui dengan format dwibahasa (Bahasa Indonesia sebagai teks utama, bahasa Inggris di dalam collapsible section `<details>`).

---

<details>
<summary><b>🇬🇧 English Translation (Click to expand)</b></summary>

## 📝 Change Description
<!-- Provide a concise summary of the changes and the rationale behind them. -->

## 🔗 Related Issue
<!-- Link related issue, e.g., Closes #12 or Fixes #34 -->
Closes #

## 🏷️ Type of Change
- [ ] 🐛 **Bug fix** (non-breaking change fixing an issue)
- [ ] ✨ **New feature** (non-breaking change adding functionality)
- [ ] ⚡ **Performance optimization** (memory allocation, MCP latency, etc.)
- [ ] 💥 **Breaking change** (fix or feature causing existing functionality to break)
- [ ] 📚 **Documentation** (documentation or markdown updates)
- [ ] 🧪 **Tests / CI** (new unit tests or CI workflow updates)

## ✅ Engineering Checklist
- [ ] **Multi-Targeting Clean Build:** Solution builds cleanly on all targets (`net8.0`, `net9.0`, `net10.0`) with 0 warnings and 0 errors (`dotnet build Sharphy.sln`).
- [ ] **Unit Tests:** All unit test suites pass 100% (`dotnet test Sharphy.sln`).
- [ ] **Unmanaged Memory Discipline:** ClrMD/native diagnostic resources are properly disposed of using `using` declarations or `.Dispose()`.
- [ ] **Sandboxed Isolation:** Untrusted assemblies are inspected within an isolated collectible `AssemblyLoadContext` (`isCollectible: true`) and unloaded promptly.
- [ ] **MCP Stdio Protocol Purity:** No debug logs or text output are written to `stdout`. All diagnostics are routed through `stderr`.
- [ ] **Documentation Sync:** Markdown docs maintain single-file bilingual format (Indonesian as primary body, English in collapsible `<details>`).

</details>
