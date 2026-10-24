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

---

<details>
<summary><b>🇬🇧 English Translation (Click to expand)</b></summary>

# Contributing to Sharphy

Thank you for your interest in contributing to **Sharphy**! This project welcomes anyone looking to expand runtime diagnostics, CLR forensics, and MCP server integrations within the .NET ecosystem.

---

## Code of Conduct

Please be respectful, collaborative, and constructive in all issues, pull requests, and discussions.

---

## Development Setup

### Prerequisites
- [.NET SDK 8.0+](https://dotnet.microsoft.com/download) (multi-targeting covers .NET 8, 9, and 10).
- Git.

### Building the Project
```bash
dotnet restore
dotnet build
```

### Running Tests
```bash
dotnet test
```

---

## Pull Request Guidelines

1. **Fork the Repository:** Create a dedicated feature branch from `main`:
   ```bash
   git checkout -b feature/your-feature-name
   ```
2. **Include Tests:** Add relevant unit tests or test scenarios in `tests/Sharphy.Tests`.
3. **Follow Code Standards:**
   - Write clean, modern C# and respect nullable reference types.
   - Ensure the solution builds cleanly across all target frameworks without introducing new warnings.
4. **Submit PR:** Open a pull request targeting `main` with a descriptive summary of your changes.

</details>
