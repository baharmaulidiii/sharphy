# Kebijakan Keamanan & Model Mitigasi Ancaman (Security Policy)

Keamanan, integritas biner, dan proteksi lingkungan komputasi pengguna merupakan prioritas tertinggi dalam perancangan ekosistem **Sharphy**. Sebagai instrumen diagnostik runtime tingkat rendah, Sharphy dirancang dengan model ancaman (*threat modeling*) yang kokoh untuk mencegah risiko eksekusi kode berbahaya.

---

## 1. Versi Terkelola & Siklus Pembaruan Keamanan

Dukungan pembaruan keamanan aktif diberikan kepada versi terkelola berikut:

| Lini Rilis | Status Dukungan | Kebijakan Patch Keamanan |
| :--- | :--- | :--- |
| `0.1.x` (Prarilis / Preview) | :white_check_mark: Didukung Penuh | Penambalan aktif terhadap kerentanan kritis, ketergantungan paket pihak ketiga, dan stabilitas server MCP. |
| `< 0.1.0` | :x: Dihentikan | Tidak didukung. Pengguna diharapkan memperbarui ke versi prarilis teranyar. |

---

## 2. Model Ancaman & Lapisan Pertahanan Arsitektural

Dalam fungsinya menganalisis biner dan memory dump, Sharphy menerapkan mitigasi defensif proaktif terhadap vektor ancaman berikut:

### 2.1 Analisis Biner Tidak Tepercaya (*Untrusted Binary Ingestion*)
- **Vektor Ancaman:** Biner yang diaudit berpotensi memuat konstruktor tipe statis (`.cctor`) yang dirancang jahat untuk mengeksekusi payload ketika tipe dimuat melalui mekanisme refleksi konvensional.
- **Lapisan Mitigasi:** Sharphy mengandalkan pembacaan metadata statis tanpa eksekusi via `Mono.Cecil`. Apabila eksekusi dinamis dibutuhkan (misalnya pada tool `sharphy_extract_tokens`), pemuatan diisolasi secara ketat di dalam `AssemblyLoadContext` kolektibel tersendiri (`isCollectible: true`) yang langsung dimusnahkan setelah eksekusi.

### 2.2 Penanganan Crash Dump Raksasa & Kehabisan Sumber Daya (*Resource Exhaustion / DoS*)
- **Vektor Ancaman:** Berkas crash dump memori berukuran puluhan gigabyte dapat memicu kondisi kehabisan memori (*Out of Memory* / OOM) pada host server jika dipetakan sekaligus ke RAM.
- **Lapisan Mitigasi:** Engine forensik ClrMD memanfaatkan pemetaan memori virtual bertahap (*lazy virtual memory mapping*) dan penelusuran segmen selektif, sehingga membatasi pemuatan hanya pada struktur data atau segmen yang sedang diaudit.

### 2.3 Pencegahan Eksfiltrasi Data via Kanal MCP
- **Vektor Ancaman:** Penyisipan payload berbahaya melalui parameter masukan JSON-RPC untuk mengintersepsi kanal komunikasi atau membaca berkas sembarangan (*path traversal*).
- **Lapisan Mitigasi:** Seluruh masukan path absolut divalidasi dan dinormalisasi secara ketat sebelum diteruskan ke mesin I/O sistem berkas lokal.

---

## 3. Tata Cara Pelaporan Kerentanan & Pengungkapan Terkoordinasi

Apabila Anda mengidentifikasi celah keamanan, kelemahan isolasi sandbox, atau potensi kerentanan lainnya pada Sharphy:

1. **JANGAN membuat issue publik atau memublikasikan eksploitasi di forum publik sebelum perbaikan dirilis.**
2. Laporkan temuan Anda secara privat dan rahasia melalui mekanisme resmi **GitHub Security Advisories** pada repositori ini:
   * Kunjungi: [https://github.com/baharmaulidiii/sharphy/security/advisories/new](https://github.com/baharmaulidiii/sharphy/security/advisories/new)
3. Sertakan informasi komprehensif berikut:
   - Ringkasan teknis dan potensi dampak kerentanan.
   - Langkah-langkah reproduksi terperinci (*step-by-step reproduction guide*).
   - Kode sampel, konfigurasi agen, atau berkas biner Proof-of-Concept (POC) minimal.
   - Versi .NET SDK, arsitektur CPU, dan Sistem Operasi yang digunakan saat pengujian.

Tim pengembang akan merespons laporan dalam waktu maksimal 48 jam kerja untuk memverifikasi temuan, mengoordinasikan jadwal penambalan (*Coordinated Vulnerability Disclosure*), serta memberikan atribusi penghargaan dalam catatan rilis keamanan.

---

<details>
<summary><b>🇬🇧 English Translation (Click to expand)</b></summary>

# Security Policy & Threat Modeling

Security and process integrity are foundational tenets of the **Sharphy** project. As a low-level runtime diagnostic bridge, Sharphy incorporates proactive threat mitigation strategies against malicious code execution vectors.

## 1. Supported Versions

| Release Line | Status | Security Patch Policy |
| :--- | :--- | :--- |
| `0.1.x` (Preview) | :white_check_mark: Supported | Active security patches for critical vulnerabilities and MCP server stability. |
| `< 0.1.0` | :x: Deprecated | Unsupported. Please upgrade to the latest preview release. |

## 2. Architectural Threat Mitigations

- **Untrusted Binary Ingestion:** Static parsing is enforced via `Mono.Cecil` without executing module constructors. Dynamic execution runs strictly inside collectible `AssemblyLoadContext` sandboxes.
- **Memory Exhaustion (DoS):** ClrMD virtual memory paging ensures large memory dumps are analyzed incrementally without overwhelming host physical RAM.
- **MCP Channel Purity:** Rigid absolute path normalization prevents unauthorized file path traversal attacks.

## 3. Reporting a Vulnerability

Please do not report vulnerabilities via public GitHub issues. Submit reports privately through **GitHub Security Advisories**:
- Advisory Portal: [https://github.com/baharmaulidiii/sharphy/security/advisories/new](https://github.com/baharmaulidiii/sharphy/security/advisories/new)
- Maintainers acknowledge reports within 48 business hours following standard Coordinated Vulnerability Disclosure (CVD) timelines.

</details>
