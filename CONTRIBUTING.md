<div align="center">
  <p>
    <b>Bahasa Indonesia</b> · <a href="CONTRIBUTING.en.md">English</a>
  </p>
</div>

# Panduan Berkontribusi untuk Sharphy

Terima kasih atas ketertarikan Anda untuk berkontribusi pada proyek **Sharphy**! Proyek ini terbuka untuk siapa saja yang ingin mengembangkan kapabilitas diagnostik runtime, forensik CLR, dan integrasi protokol MCP ke dalam ekosistem .NET.

---

## Kode Etik

Harap selalu bersikap saling menghargai, ramah, dan profesional dalam setiap diskusi isu, pull request, maupun saluran komunikasi proyek.

---

## Persiapan Lingkungan Pengembangan

### Kebutuhan Sistem
- [.NET SDK 8.0+](https://dotnet.microsoft.com/download) (dukungan multi-targeting mencakup .NET 8, 9, dan 10).
- Git.

### Melakukan Build
```bash
dotnet restore
dotnet build
```

### Menjalankan Unit Test
```bash
dotnet test
```

---

## Alur Pengajuan Pull Request (PR)

1. **Fork Repositori:** Buat branch fitur dari `main` Anda:
   ```bash
   git checkout -b feature/nama-fitur-anda
   ```
2. **Tambahkan Pengujian:** Sertakan unit test atau sample kasus uji yang relevan di `tests/Sharphy.Tests`.
3. **Patuhi Standar Kode:**
   - Gunakan fitur modern C# dan hormati preferensi nullable reference types.
   - Pastikan kode dapat dikompilasi bersih di seluruh target framework tanpa *warning* baru.
4. **Kirim PR:** Buat pull request ke branch `main` dengan deskripsi perubahan yang jelas dan tujuan yang ingin dicapai.
