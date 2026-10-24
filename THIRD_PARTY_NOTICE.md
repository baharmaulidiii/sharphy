# Pemberitahuan Pihak Ketiga (Third-Party Notices)

Dokumen ini berisi informasi atribusi, lisensi, dan pemberitahuan hak cipta atas komponen, pustaka sumber terbuka (*open-source software*), dan dependensi pihak ketiga yang digunakan atau diintegrasikan dalam proyek **Sharphy**.

Sharphy berterima kasih kepada komunitas sumber terbuka dan para pembuat pustaka luar biasa berikut:

---

## Daftar Komponen & Pustaka Pihak Ketiga

### 1. Microsoft.Diagnostics.Runtime (ClrMD)
- **Deskripsi:** Pustaka diagnostik mendalam untuk inspeksi proses runtime .NET Core / .NET Framework dan crash dump (`.dmp`). Digunakan pada modul analisis memori, deteksi deadlock, dan audit managed heap.
- **Pengembang / Pemilik:** .NET Foundation and Contributors / Microsoft Corporation
- **Repositori:** [https://github.com/microsoft/clrmd](https://github.com/microsoft/clrmd)
- **Lisensi:** [MIT License](https://github.com/microsoft/clrmd/blob/main/LICENSE.TXT)
- **Pernyataan Hak Cipta:** Copyright (c) .NET Foundation and Contributors. All rights reserved.

---

### 2. ICSharpCode.Decompiler (ILSpy Engine)
- **Deskripsi:** Mesin dekompilasi bytecode CIL .NET modern yang menghasilkan kode C# bersih dan terstruktur. Digunakan pada modul rekonstruksi kode dan analisis metode biner.
- **Pengembang / Pemilik:** Daniel Grunwald, David Srbecky, Ed Harvey, Siegfried Pammer, Artur Zgodzinski, dan kontributor ILSpy
- **Repositori:** [https://github.com/icsharpcode/ILSpy](https://github.com/icsharpcode/ILSpy)
- **Lisensi:** [MIT License](https://github.com/icsharpcode/ILSpy/blob/master/doc/license.txt)
- **Pernyataan Hak Cipta:** Copyright (c) 2011-2025 AlphaSierraPapa for the SharpDevelop team.

---

### 3. Mono.Cecil
- **Deskripsi:** Pustaka komprehensif untuk memeriksa, memodifikasi, dan menghasilkan assembly biner serta bytecode CIL ECMA-335. Digunakan pada modul inspeksi metadata, deobfuscation, dan mutasi/patching instruksi CIL.
- **Pengembang / Pemilik:** Jb Evain and Contributors
- **Repositori:** [https://github.com/jbevain/cecil](https://github.com/jbevain/cecil)
- **Lisensi:** [MIT License](https://github.com/jbevain/cecil/blob/master/LICENSE.txt)
- **Pernyataan Hak Cipta:** Copyright (c) 2008 - 2018 Jb Evain. Copyright (c) 2008 - 2011 Novell, Inc.

---

### 4. Microsoft.CodeAnalysis.CSharp (Roslyn)
- **Deskripsi:** Kompiler C# modern dan API analisis kode statis. Digunakan untuk analisis semantik, validasi tipe, dan pembuatan wrapper strongly-typed tingkat tinggi.
- **Pengembang / Pemilik:** .NET Foundation and Contributors / Microsoft Corporation
- **Repositori:** [https://github.com/dotnet/roslyn](https://github.com/dotnet/roslyn)
- **Lisensi:** [MIT License](https://github.com/dotnet/roslyn/blob/main/License.txt)
- **Pernyataan Hak Cipta:** Copyright (c) .NET Foundation and Contributors. All rights reserved.

---

## Teks Lisensi Standar (MIT License)

Komponen-komponen di atas dilisensikan di bawah persyaratan Lisensi MIT (kecuali dinyatakan lain secara spesifik pada masing-masing sub-komponen aslinya):

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

# Third-Party Notices

This document contains attribution, licensing, and copyright notices for third-party open-source software (OSS) libraries and components utilized or integrated within the **Sharphy** project.

Sharphy acknowledges and thanks the open-source community and the authors of the following libraries:

---

## List of Third-Party Libraries & Components

### 1. Microsoft.Diagnostics.Runtime (ClrMD)
- **Description:** Advanced diagnostics library for inspecting .NET Core / .NET Framework runtime processes and crash dump files (`.dmp`). Utilized in memory dump forensics, deadlock detection, and managed heap inspection.
- **Author / Copyright Holder:** .NET Foundation and Contributors / Microsoft Corporation
- **Repository:** [https://github.com/microsoft/clrmd](https://github.com/microsoft/clrmd)
- **License:** [MIT License](https://github.com/microsoft/clrmd/blob/main/LICENSE.TXT)
- **Copyright Statement:** Copyright (c) .NET Foundation and Contributors. All rights reserved.

---

### 2. ICSharpCode.Decompiler (ILSpy Engine)
- **Description:** Modern .NET CIL bytecode decompiler engine producing clean, idiomatic C# source code. Utilized in code reconstruction and method disassembly modules.
- **Author / Copyright Holder:** Daniel Grunwald, David Srbecky, Ed Harvey, Siegfried Pammer, Artur Zgodzinski, and ILSpy contributors
- **Repository:** [https://github.com/icsharpcode/ILSpy](https://github.com/icsharpcode/ILSpy)
- **License:** [MIT License](https://github.com/icsharpcode/ILSpy/blob/master/doc/license.txt)
- **Copyright Statement:** Copyright (c) 2011-2025 AlphaSierraPapa for the SharpDevelop team.

---

### 3. Mono.Cecil
- **Description:** Comprehensive library to inspect, modify, and emit ECMA-335 assemblies and CIL bytecode. Utilized in metadata inspection, deobfuscation, and CIL instruction patching/rewriting.
- **Author / Copyright Holder:** Jb Evain and Contributors
- **Repository:** [https://github.com/jbevain/cecil](https://github.com/jbevain/cecil)
- **License:** [MIT License](https://github.com/jbevain/cecil/blob/master/LICENSE.txt)
- **Copyright Statement:** Copyright (c) 2008 - 2018 Jb Evain. Copyright (c) 2008 - 2011 Novell, Inc.

---

### 4. Microsoft.CodeAnalysis.CSharp (Roslyn)
- **Description:** C# compiler and static analysis platform. Utilized for semantic analysis, type verification, and strongly-typed interop wrapper generation.
- **Author / Copyright Holder:** .NET Foundation and Contributors / Microsoft Corporation
- **Repository:** [https://github.com/dotnet/roslyn](https://github.com/dotnet/roslyn)
- **License:** [MIT License](https://github.com/dotnet/roslyn/blob/main/License.txt)
- **Copyright Statement:** Copyright (c) .NET Foundation and Contributors. All rights reserved.

---

## Standard License Text (MIT License)

The aforementioned components are licensed under the terms of the MIT License (unless otherwise specifically noted in their upstream components):

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

</details>
---

<details>
<summary><b>🇬🇧 English Translation (Click to expand)</b></summary>

# Third-Party Notices

This document contains attribution, licensing, and copyright notices for third-party open-source software (OSS) libraries and components utilized or integrated within the **Sharphy** project.

Sharphy acknowledges and thanks the open-source community and the authors of the following libraries:

---

## List of Third-Party Libraries & Components

### 1. Microsoft.Diagnostics.Runtime (ClrMD)
- **Description:** Advanced diagnostics library for inspecting .NET Core / .NET Framework runtime processes and crash dump files (`.dmp`). Utilized in memory dump forensics, deadlock detection, and managed heap inspection.
- **Author / Copyright Holder:** .NET Foundation and Contributors / Microsoft Corporation
- **Repository:** [https://github.com/microsoft/clrmd](https://github.com/microsoft/clrmd)
- **License:** [MIT License](https://github.com/microsoft/clrmd/blob/main/LICENSE.TXT)
- **Copyright Statement:** Copyright (c) .NET Foundation and Contributors. All rights reserved.

---

### 2. ICSharpCode.Decompiler (ILSpy Engine)
- **Description:** Modern .NET CIL bytecode decompiler engine producing clean, idiomatic C# source code. Utilized in code reconstruction and method disassembly modules.
- **Author / Copyright Holder:** Daniel Grunwald, David Srbecky, Ed Harvey, Siegfried Pammer, Artur Zgodzinski, and ILSpy contributors
- **Repository:** [https://github.com/icsharpcode/ILSpy](https://github.com/icsharpcode/ILSpy)
- **License:** [MIT License](https://github.com/icsharpcode/ILSpy/blob/master/doc/license.txt)
- **Copyright Statement:** Copyright (c) 2011-2025 AlphaSierraPapa for the SharpDevelop team.

---

### 3. Mono.Cecil
- **Description:** Comprehensive library to inspect, modify, and emit ECMA-335 assemblies and CIL bytecode. Utilized in metadata inspection, deobfuscation, and CIL instruction patching/rewriting.
- **Author / Copyright Holder:** Jb Evain and Contributors
- **Repository:** [https://github.com/jbevain/cecil](https://github.com/jbevain/cecil)
- **License:** [MIT License](https://github.com/jbevain/cecil/blob/master/LICENSE.txt)
- **Copyright Statement:** Copyright (c) 2008 - 2018 Jb Evain. Copyright (c) 2008 - 2011 Novell, Inc.

---

### 4. Microsoft.CodeAnalysis.CSharp (Roslyn)
- **Description:** C# compiler and static analysis platform. Utilized for semantic analysis, type verification, and strongly-typed interop wrapper generation.
- **Author / Copyright Holder:** .NET Foundation and Contributors / Microsoft Corporation
- **Repository:** [https://github.com/dotnet/roslyn](https://github.com/dotnet/roslyn)
- **License:** [MIT License](https://github.com/dotnet/roslyn/blob/main/License.txt)
- **Copyright Statement:** Copyright (c) .NET Foundation and Contributors. All rights reserved.

---

## Standard License Text (MIT License)

The aforementioned components are licensed under the terms of the MIT License (unless otherwise specifically noted in their upstream components):

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

</details>

