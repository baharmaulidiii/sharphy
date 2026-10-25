# Third-Party Notices & Licensing Attributions

This document contains licensing acknowledgments, copyright claims, and official legal notices for third-party open-source components integrated into the **Sharphy** project.

The maintainer of Sharphy extends deep gratitude to the open-source community and the architects behind these four foundational libraries:

---

## 1. Third-Party Library Components

### 1.1 Microsoft.Diagnostics.Runtime (ClrMD)
- **Technical Role:** Powers runtime memory dump forensics (`sharphy_analyze_dump`), managed heap segment traversals and GC generation audits (`sharphy_inspect_heap`), and Monitor lock cycle detection (`sharphy_deadlock_detector`).
- **Copyright:** Copyright (c) .NET Foundation and Contributors / Microsoft Corporation.
- **Repository:** [https://github.com/microsoft/clrmd](https://github.com/microsoft/clrmd)
- **License:** [MIT License](https://github.com/microsoft/clrmd/blob/main/LICENSE.TXT)

---

### 1.2 ICSharpCode.Decompiler (ILSpy Engine)
- **Technical Role:** Powers CIL method decompilation, transforming low-level bytecode into modern idiomatic C# source code (`sharphy_decompile_method`), including async state machine reconstruction and compiler-generated attribute synthesis.
- **Copyright:** Copyright (c) 2011-2025 AlphaSierraPapa for the SharpDevelop team, Daniel Grunwald, David Srbecky, Ed Harvey, Siegfried Pammer, Artur Zgodzinski, and ILSpy contributors.
- **Repository:** [https://github.com/icsharpcode/ILSpy](https://github.com/icsharpcode/ILSpy)
- **License:** [MIT License](https://github.com/icsharpcode/ILSpy/blob/master/doc/license.txt)

---

### 1.3 Mono.Cecil
- **Technical Role:** Powers ECMA-335 metadata hierarchy inspection (`sharphy_inspect_metadata`), binary assembly diffing (`sharphy_diff_assemblies`), control-flow obfuscation analysis (`sharphy_clean_obfuscation`), and surgical CIL instruction bytecode patching (`sharphy_patch_il`).
- **Copyright:** Copyright (c) 2008 - 2018 Jb Evain. Copyright (c) 2008 - 2011 Novell, Inc.
- **Repository:** [https://github.com/jbevain/cecil](https://github.com/jbevain/cecil)
- **License:** [MIT License](https://github.com/jbevain/cecil/blob/master/LICENSE.txt)

---

### 1.4 Microsoft.CodeAnalysis.CSharp (Roslyn Compiler Platform)
- **Technical Role:** Powers semantic type resolution and strongly-typed wrapper synthesis utilizing high-performance .NET 8+ `[UnsafeAccessor]` attributes (`sharphy_generate_wrapper`).
- **Copyright:** Copyright (c) .NET Foundation and Contributors / Microsoft Corporation.
- **Repository:** [https://github.com/dotnet/roslyn](https://github.com/dotnet/roslyn)
- **License:** [MIT License](https://github.com/dotnet/roslyn/blob/main/License.txt)

---

## 2. Standard MIT License Text

The third-party components listed above are distributed under the standard terms of the MIT License:

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

## 3. License Compatibility Statement

The **Sharphy** project as a whole is distributed under the **Apache License, Version 2.0**. Integrating MIT-licensed dependencies into an Apache-2.0 licensed project is fully permissible and conforms to industry-standard permissive open-source compatibility (*permissive-to-permissive compatibility*). All original copyright notices, contributor credits, and license headers are preserved intact.
