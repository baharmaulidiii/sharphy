# Panduan Berkontribusi untuk Sharphy

Dokumen ini menetapkan protokol rekayasa perangkat lunak, standar arsitektur tingkat rendah, serta kebijakan kontribusi kode yang berlaku secara mengikat bagi seluruh kontributor manusia maupun agen pengembang (*AI Coding Agents*) pada repositori **Sharphy**.

---

## 1. Prinsip Utama & Standar Rekayasa

Sebagai repositori yang menangani diagnostik memori internal dan manipulasi bytecode CIL, setiap kontribusi wajib memenuhi empat pilar rekayasa berikut:

### 1.1 Manajemen Memori Unmanaged & Pencegahan Kebocoran
- Objek diagnostik runtime seperti `DataTarget`, `ClrRuntime`, dan `ClrHeap` mengalokasikan sumber daya native tingkat rendah yang berada di luar jangkauan GC reguler.
- Setiap instansiasi komponen ClrMD **wajib** dibungkus dalam pernyataan `using` deklaratif atau dilepaskan secara eksplisit melalui pemanggilan `.Dispose()`.
- Penggunaan tipe `ReadOnlySpan<byte>`, `ReadOnlyMemory<byte>`, dan `ref struct` sangat diutamakan dalam pembacaan header metadata biner PE/COFF guna meminimalkan tekanan GC (*zero-allocation philosophy*).

### 1.2 Isolasi Biner & Sandboxing Lingkungan Eksekusi
- Pemeriksaan atau pengeksekusian biner pihak ketiga tidak tepercaya (*untrusted binaries*) dilarang keras dimuat langsung ke dalam konteks eksekusi default proses host (`AssemblyLoadContext.Default`).
- Seluruh inspeksi dinamis wajib dijalankan di dalam instansi `AssemblyLoadContext` kolektibel tersendiri:
  ```csharp
  var isolatedContext = new AssemblyLoadContext("SharphySandbox", isCollectible: true);
  try
  {
      // Jalankan evaluasi atau inspeksi biner terisolasi di sini
  }
  finally
  {
      isolatedContext.Unload();
  }
  ```
- Pastikan unloading berhasil secara deterministik untuk mencegah penguncian berkas (*file locks*) pada media penyimpanan dan pencemaran ruang memori host.

### 1.3 Multi-Targeting Lintas Kerangka Kerja (.NET 8, 9, 10)
- Setiap modifikasi kode pada `Sharphy.Core`, `Sharphy.Server`, dan `Sharphy.Cli` harus berhasil dikompilasi secara bersih pada ketiga target kerangka kerja: `.NET 8.0 (LTS)`, `.NET 9.0 (STS)`, dan `.NET 10.0 (Latest)`.
- Jika memanfaatkan fitur runtime atau API compiler teranyar, gunakan direktif kompilasi kondisional (*preprocessor guards*):
  ```csharp
  #if NET10_0_OR_GREATER
  // Implementasi performa tinggi teranyar .NET 10
  #elif NET9_0_OR_GREATER
  // Implementasi khusus .NET 9
  #else
  // Fallback standar .NET 8 LTS
  #endif
  ```

### 1.4 Kemurnian Kanal Komunikasi Stdio (Model Context Protocol)
- Pada proyek `Sharphy.Server`, aliran keluaran standar (`Console.Out`) didedikasikan secara eksklusif untuk transmisi payload protokol JSON-RPC 2.0.
- Dilarang keras menuliskan pesan log, jejak eksepsi, atau teks *debugging* ke `stdout`. Seluruh instrumentasi dan diagnostik internal wajib disalurkan melalui `stderr` (`Console.Error.WriteLine`).

---

## 2. Lingkungan Pengembangan Lokal

### Prasyarat Perangkat Lunak
1. [.NET SDK 8.0+](https://dotnet.microsoft.com/download) (direkomendasikan memiliki instalasi SDK 8.0 dan 10.0 untuk memvalidasi multi-target penuh).
2. Git CLI versi 2.40 ke atas.

### Alur Eksekusi Perintah Standar
```powershell
# Memulihkan dependensi paket
dotnet restore Sharphy.sln

# Mengompilasi seluruh solusi dalam konfigurasi Debug
dotnet build Sharphy.sln -c Debug

# Menjalankan suite pengujian unit secara lengkap
dotnet test Sharphy.sln

# Menjalankan server MCP secara lokal melalui CLI
dotnet run --project src/Sharphy.Cli/Sharphy.Cli.csproj --framework net8.0 -- mcp start
```

---

## 3. Alur Kerja Kontribusi & Pengajuan Pull Request (PR)

1. **Pembuatan Branch Spesifik:**
   - Buat branch baru dari `main` dengan konvensi penamaan standar:
     * `feat/nama-fitur` untuk kapabilitas atau tool MCP baru.
     * `fix/deskripsi-perbaikan` untuk penanganan anomali atau bug.
     * `perf/optimasi-alokasi` untuk perbaikan efisiensi memori.
2. **Kepatuhan Pengujian:**
   - Setiap fitur atau tool baru wajib disertai pengujian unit di `tests/Sharphy.Tests` berbasis framework `xunit.v3`.
   - Pastikan kompilasi menghasilkan **0 Warning** dan **0 Error**.
3. **Larangan Mutlak Agen (Agent Git Policy):**
   - AI Coding Agent yang beroperasi di repositori ini dilarang keras menjalankan perintah `git commit` maupun `git push`.
   - Hanya pemilik repositori / pengembang manusia yang berhak melakukan peninjauan akhir dan melakukan commit ke riwayat Git.

---

<details>
<summary><b>🇬🇧 English Translation (Click to expand)</b></summary>

# Contributing Guidelines for Sharphy

This document outlines mandatory engineering standards, low-level architecture policies, and contribution guidelines for human engineers and AI coding agents operating on the **Sharphy** repository.

## 1. Core Engineering Principles

1. **Unmanaged Memory Discipline:** Ensure all ClrMD instances (`DataTarget`, `ClrRuntime`) are properly disposed of using `using` declarations. Avoid memory leaks across forensic scans.
2. **Isolated Sandboxing:** Untrusted assemblies must never be loaded into the default context. Reflection must execute within an isolated collectible `AssemblyLoadContext` (`isCollectible: true`) and unloaded immediately after evaluation.
3. **Multi-Targeting Awareness:** All code within `Sharphy.Core`, `Sharphy.Server`, and `Sharphy.Cli` must cleanly compile across `.NET 8.0`, `.NET 9.0`, and `.NET 10.0`. Apply conditional directives (`#if NET10_0_OR_GREATER`) when utilizing newer BCL/CLR APIs.
4. **Stdio Purity:** The standard output stream (`stdout`) is reserved strictly for JSON-RPC 2.0 messages. All diagnostic logs must be written to `stderr`.

## 2. Standard CLI Commands

```powershell
dotnet restore Sharphy.sln
dotnet build Sharphy.sln -c Debug
dotnet test Sharphy.sln
dotnet run --project src/Sharphy.Cli/Sharphy.Cli.csproj --framework net8.0 -- mcp start
```

## 3. Pull Request Guidelines

- Create dedicated branches (`feat/`, `fix/`, `perf/`).
- Include comprehensive unit test coverage in `tests/Sharphy.Tests`.
- AI Coding Agents are strictly forbidden from executing `git commit` or `git push`. All changes must remain uncommitted in the local working directory for human owner review.

</details>
