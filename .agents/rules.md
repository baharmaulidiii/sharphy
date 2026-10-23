# Agent Coding Rules & Standards for Sharphy

Dokumen ini mendefinisikan aturan ketat yang wajib dipatuhi oleh setiap AI Agent saat menulis atau memodifikasi kode di repositori ini.

---

## 1. Standar C# & Framework

1. **C# Version:** Menggunakan `latest` C# features yang didukung oleh compiler SDK saat ini.
2. **Nullable Reference Types:** Wajib diaktifkan (`<Nullable>enable</Nullable>`). Hindari `null!` kecuali pada interoperabilitas COM/P-Invoke yang terjustifikasi.
3. **Multi-Targeting Guards:**
   - Apabila menggunakan API baru yang hanya ada di .NET 9 atau .NET 10 (misalnya ekspansi `UnsafeAccessor` atau optimasi runtime baru), gunakan conditional compilation:
     ```csharp
     #if NET10_0_OR_GREATER
     // .NET 10 specific optimization
     #elif NET9_0_OR_GREATER
     // .NET 9 specific implementation
     #else
     // .NET 8 LTS fallback
     #endif
     ```
4. **Performance & Allocations:**
   - Gunakan `ReadOnlySpan<byte>`, `Memory<T>`, dan `ref struct` saat membaca raw metadata biner untuk meminimalkan beban GC.
   - Hindari memory leak saat membuka dump; pastikan `DataTarget` dan `ClrRuntime` selalu dibungkus dalam blok `using` atau di-dispose secara eksplisit.

---

## 2. Standar MCP Server (`Sharphy.Server`)

1. **Protokol:** Ikuti spesifikasi resmi Model Context Protocol (JSON-RPC 2.0).
2. **Error Handling:** Tool MCP tidak boleh menyebabkan server crash jika biner target korup, obfuscated, atau tidak valid. Bungkus eksekusi dalam try-catch dan kembalikan pesan error deskriptif dalam response MCP.
3. **Streaming & Stdio:** Transport stdio tidak boleh tercemar oleh log `Console.WriteLine` biasa; semua internal diagnostics harus diarahkan ke `Console.Error.WriteLine` (stderr).

---

## 3. Isolasi Assembly Loading

1. Saat melakukan parsing atau inspeksi terhadap biner asing yang diuji (`CrackmeDemo` atau biner pihak ketiga), gunakan `AssemblyLoadContext` terpisah:
   ```csharp
   var alc = new AssemblyLoadContext("SharphyIsolatedInspectContext", isCollectible: true);
   ```
2. Pastikan context dapat di-unload setelah proses analisis selesai untuk mencegah file lock pada disk.
