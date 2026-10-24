# Pernyataan Aksesibilitas (Accessibility Statement)

Komunitas pengembang **Sharphy** berkomitmen untuk menyediakan antarmuka baris perintah (CLI), server Model Context Protocol (MCP), dokumentasi, dan portal web yang dapat diakses oleh semua kalangan, termasuk pengguna dengan disabilitas fisik, motorik, penglihatan, pendengaran, maupun kognitif.

---

## 1. Komitmen Aksesibilitas

Kami berusaha mematuhi standar aksesibilitas internasional, termasuk **Web Content Accessibility Guidelines (WCAG) 2.1 Level AA** pada portal dokumentasi web kami, serta prinsip-prinsip desain CLI yang ramah pembaca layar (*screen reader*).

### Standar pada Antarmuka CLI & Server MCP
- **Dukungan Terminal & Pembaca Layar:** Seluruh keluaran CLI Sharphy dirancang menggunakan teks terstruktur yang dapat diinterpretasikan dengan baik oleh pembaca layar seperti NVDA, JAWS, dan Orca.
- **Dukungan Warna & Kontras Tinggi:** CLI menggunakan format pesan diagnostik yang tidak hanya mengandalkan kode warna semata, melainkan juga simbol berbasis teks dan label status eksplisit untuk mendukung pengguna dengan buta warna atau kontras rendah.
- **Stabilitas Protokol Stdio:** Keluaran standar (`stdout`) dipisahkan secara murni dari aliran log kesalahan (`stderr`), mencegah pembaca layar dibanjiri galat internal yang tidak relevan.

### Standar pada Dokumentasi Web
- Struktur semantik HTML5 yang benar (`<nav>`, `<main>`, `<article>`, `<header>`, `<footer>`).
- Dukungan navigasi papan ketik (*full keyboard navigation*) dan indikator fokus (*focus outline*) yang jelas.
- Kontras warna teks dan latar belakang yang memenuhi rasio minimal WCAG 2.1 AA (4.5:1 untuk teks normal).
- Atribut teks alternatif (*alt text*) untuk ilustrasi, diagram, dan tangkapan layar.

---

## 2. Pelaporan Kendala Aksesibilitas

Jika Anda menemukan hambatan aksesibilitas saat menggunakan CLI, membaca dokumentasi, atau berinteraksi dengan repositori Sharphy, mohon beri tahu kami agar kami dapat memperbaikinya:

- **Buka Issue di GitHub:** [Buat Issue Baru](https://github.com/baharmaulidiii/sharphy/issues/new/choose) dengan label `accessibility`.
- **Informasi yang Membantu:**
  - Perangkat bantu yang Anda gunakan (misalnya NVDA, VoiceOver, peramban tertentu, pembesar layar).
  - URL dokumentasi atau perintah CLI yang memicu kendala.
  - Deskripsi kendala yang dihadapi dan saran perbaikan jika ada.

Kami akan menanggapi laporan aksesibilitas secepat mungkin dan memprioritaskan perbaikannya.

---

<details>
<summary><b>🇬🇧 English Translation (Click to expand)</b></summary>

# Accessibility Statement

The **Sharphy** developer community is committed to ensuring that our Command Line Interface (CLI), Model Context Protocol (MCP) server, documentation, and web portal are accessible to everyone, including users with visual, auditory, motor, or cognitive disabilities.

## 1. Accessibility Commitment

We strive to conform to international standards, including **Web Content Accessibility Guidelines (WCAG) 2.1 Level AA** across our documentation portal, as well as screen-reader-friendly CLI design principles.

### CLI & MCP Server Standards
- **Screen Reader Compatibility:** CLI output provides structured, plain-text diagnostic messages easily read by screen readers (NVDA, JAWS, VoiceOver, Orca).
- **Color Independence:** CLI messages never rely solely on color to convey meaning; text-based prefixes and explicit status badges are always provided.
- **Stdio Stream Separation:** Clear isolation between `stdout` (JSON-RPC) and `stderr` (diagnostics/errors) prevents unexpected noise for assistive tools.

### Web Documentation Standards
- Semantic HTML5 structure (`<nav>`, `<main>`, `<article>`, `<header>`, `<footer>`).
- Full keyboard navigation and visible focus rings.
- Color contrast compliant with WCAG 2.1 AA ratios (minimum 4.5:1 for normal text).
- Informative alternative text (`alt`) for diagrams, architectural schemas, and preview images.

## 2. Reporting Accessibility Barriers

If you encounter accessibility issues or barriers while using Sharphy or its documentation:
- **Open an Issue:** Submit an issue on [GitHub Issues](https://github.com/baharmaulidiii/sharphy/issues/new/choose) with the `accessibility` label.
- **Helpful Details:** Mention your assistive technology (e.g., NVDA, VoiceOver, zoom software), the command or page URL, and a description of the barrier.

We review accessibility feedback promptly and prioritize fixes accordingly.

</details>
