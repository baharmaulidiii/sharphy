# Testing Policy for AI Agents

Dokumen ini memandu prosedur pengujian otomatis yang wajib dijalankan oleh AI Agent sebelum menyelesaikan tugas.

---

## 1. Lingkup Pengujian

1. **Unit Testing (`tests/Sharphy.Tests`):**
   - Menguji isolasi unit logika pada `Sharphy.Core` (parsing metadata, helper IL, dan kalkulator heap).
   - Menjamin bahwa tool dispatching di `Sharphy.Server` memformat JSON-RPC sesuai spesifikasi.
2. **Sample Testbed (`samples/CrackmeDemo`):**
   - Projek target yang sengaja dibuat untuk memvalidasi dekompilasi, forensic snapshot, dan manipulasi CIL instruksi secara aman.

---

## 2. Checklist Verifikasi Agent

Sebelum melakukan commit atau memberi tahu pengguna bahwa implementasi selesai, Agent **wajib** memastikan:
- [ ] `dotnet build Sharphy.sln` sukses dengan 0 error dan 0 warning baru.
- [ ] `dotnet test Sharphy.sln` berhasil melewati seluruh pengujian.
- [ ] Berkas dokumentasi ([README.md](file:///d:/Programming/GitHub/sharphy/README.md) & [README.en.md](file:///d:/Programming/GitHub/sharphy/README.en.md)) mencerminkan status terbaru jika ada penambahan MCP tools baru.
