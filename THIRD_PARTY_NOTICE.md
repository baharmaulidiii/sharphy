# Pemberitahuan Pihak Ketiga & Atribusi Lisensi (Third-Party Notices)

Dokumen ini memuat deklarasi atribusi, kepemilikan hak cipta, dan teks lisensi resmi atas seluruh pustaka sumber terbuka (*open-source software*) yang diintegrasikan atau dijadikan fondasi teknologi pada proyek **Sharphy**.

Pengembang Sharphy menyampaikan apresiasi mendalam kepada komunitas sumber terbuka dan para arsitek perangkat lunak di balik empat pilar pustaka utama berikut:

---

## 1. Komponen Pustaka Pihak Ketiga

### 1.1 Microsoft.Diagnostics.Runtime (ClrMD)
- **Cakupan Penggunaan Teknis:** Digunakan pada modul forensik memori (`Sharphy.Core`), mencakup analisis crash dump lintas platform Windows dan Linux (`sharphy_analyze_dump`), penelusuran graf objek managed heap dan segmentasi GC (`sharphy_inspect_heap`), serta traversal rantai `SyncBlock` untuk deteksi siklus deadlock (`sharphy_deadlock_detector`).
- **Pemegang Hak Cipta:** Copyright (c) .NET Foundation and Contributors / Microsoft Corporation.
- **Tautan Repositori:** [https://github.com/microsoft/clrmd](https://github.com/microsoft/clrmd)
- **Ketentuan Lisensi:** [MIT License](https://github.com/microsoft/clrmd/blob/main/LICENSE.TXT)

---

### 1.2 ICSharpCode.Decompiler (Mesin ILSpy)
- **Cakupan Penggunaan Teknis:** Digunakan pada modul rekonstruksi kode (`Sharphy.Core`), memfasilitasi transformasi bytecode CIL tingkat rendah menjadi kode sumber C# modern yang bersih dan terstruktur (`sharphy_decompile_method`), termasuk rekonstruksi *state machine* asinkron dan atribut kompilasi.
- **Pemegang Hak Cipta:** Copyright (c) 2011-2025 AlphaSierraPapa for the SharpDevelop team, Daniel Grunwald, David Srbecky, Ed Harvey, Siegfried Pammer, Artur Zgodzinski, dan kontributor ILSpy.
- **Tautan Repositori:** [https://github.com/icsharpcode/ILSpy](https://github.com/icsharpcode/ILSpy)
- **Ketentuan Lisensi:** [MIT License](https://github.com/icsharpcode/ILSpy/blob/master/doc/license.txt)

---

### 1.3 Mono.Cecil
- **Cakupan Penggunaan Teknis:** Digunakan pada modul inspeksi metadata dan manipulasi biner (`Sharphy.Core`), mencakup pembacaan tabel metadata ECMA-335 (`sharphy_inspect_metadata`), komparasi perbedaan signature biner (`sharphy_diff_assemblies`), analisis deobfuscation (`sharphy_clean_obfuscation`), serta penyuntikan atau mutasi instruksi CIL opcode secara presisi (`sharphy_patch_il`).
- **Pemegang Hak Cipta:** Copyright (c) 2008 - 2018 Jb Evain. Copyright (c) 2008 - 2011 Novell, Inc.
- **Tautan Repositori:** [https://github.com/jbevain/cecil](https://github.com/jbevain/cecil)
- **Ketentuan Lisensi:** [MIT License](https://github.com/jbevain/cecil/blob/master/LICENSE.txt)

---

### 1.4 Microsoft.CodeAnalysis.CSharp (Roslyn Compiler Platform)
- **Cakupan Penggunaan Teknis:** Digunakan pada modul sintesis wrapper dan analisis semantik tipe (`Sharphy.Core`), mencakup pembuatan interop strongly-typed berbasis C# modern dan generasi atribut `[UnsafeAccessor]` berperforma tinggi (`sharphy_generate_wrapper`).
- **Pemegang Hak Cipta:** Copyright (c) .NET Foundation and Contributors / Microsoft Corporation.
- **Tautan Repositori:** [https://github.com/dotnet/roslyn](https://github.com/dotnet/roslyn)
- **Ketentuan Lisensi:** [MIT License](https://github.com/dotnet/roslyn/blob/main/License.txt)

---

## 2. Salinan Teks Lisensi Standar (MIT License)

Komponen-komponen perangkat lunak di atas didistribusikan di bawah persyaratan Lisensi MIT sebagaimana tertera di bawah ini:

```text
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

---

<details>
<summary><b>🇬🇧 English Translation (Click to expand)</b></summary>

# Third-Party Notices & Licensing Attributions

This document contains licensing acknowledgments, copyright claims, and official legal texts for third-party open-source components integrated into **Sharphy**.

## 1. Third-Party Library Components

### 1.1 Microsoft.Diagnostics.Runtime (ClrMD)
- **Technical Role:** Powers runtime memory dump forensics, managed heap segment traversals, and thread lock synchronization graph detection.
- **Copyright:** Copyright (c) .NET Foundation and Contributors / Microsoft Corporation.
- **License:** [MIT License](https://github.com/microsoft/clrmd/blob/main/LICENSE.TXT)

### 1.2 ICSharpCode.Decompiler (ILSpy Engine)
- **Technical Role:** Core CIL method decompilation engine reconstructing modern idiomatic C# code from binary assemblies.
- **Copyright:** Copyright (c) 2011-2025 AlphaSierraPapa, Daniel Grunwald, and ILSpy contributors.
- **License:** [MIT License](https://github.com/icsharpcode/ILSpy/blob/master/doc/license.txt)

### 1.3 Mono.Cecil
- **Technical Role:** ECMA-335 metadata parsing, binary assembly diffing, and surgical CIL instruction bytecode patching.
- **Copyright:** Copyright (c) 2008 - 2018 Jb Evain, Novell Inc.
- **License:** [MIT License](https://github.com/jbevain/cecil/blob/master/LICENSE.txt)

### 1.4 Microsoft.CodeAnalysis.CSharp (Roslyn)
- **Technical Role:** Semantic code synthesis and strongly-typed `[UnsafeAccessor]` interop code generation.
- **Copyright:** Copyright (c) .NET Foundation and Contributors / Microsoft Corporation.
- **License:** [MIT License](https://github.com/dotnet/roslyn/blob/main/License.txt)

## 2. Standard MIT License Text

*(See text included above in standard form).*

</details>
