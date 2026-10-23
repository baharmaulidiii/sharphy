'use client';

import { useState } from 'react';
import { 
  Terminal, 
  Cpu, 
  ShieldAlert, 
  Layers, 
  Search, 
  BookOpen, 
  ExternalLink, 
  ChevronRight, 
  Copy, 
  Check, 
  FileCode,
  Bug,
  Activity,
  Boxes
} from 'lucide-react';

const toolsCatalog = [
  { name: 'sharphy_analyze_dump', category: 'Diagnostics', desc: 'Root Cause Analysis (RCA) otomatis pada crash dump (.dmp) Windows / Linux.' },
  { name: 'sharphy_inspect_heap', category: 'Diagnostics', desc: 'Audit managed heap, deteksi memory leak, dan fragmentasi Large Object Heap (LOH).' },
  { name: 'sharphy_deadlock_detector', category: 'Diagnostics', desc: 'Pelacakan rantai thread lock dan sync-block lock contention.' },
  { name: 'sharphy_decompile_method', category: 'Reconstruction', desc: 'Dekompilasi method CIL target menjadi kode C# bersih via ILSpy engine.' },
  { name: 'sharphy_diff_assemblies', category: 'Metadata', desc: 'Komparasi biner dua versi assembly .dll/.exe untuk mendeteksi perubahan API.' },
  { name: 'sharphy_inspect_metadata', category: 'Metadata', desc: 'Inspeksi hierarki ECMA-335, signature P/Invoke, attribute, dan embedded resources.' },
  { name: 'sharphy_clean_obfuscation', category: 'Integrity', desc: 'Pembersihan teknik obfuscation dasar (CFG flattening, proxy delegates).' },
  { name: 'sharphy_extract_tokens', category: 'Integrity', desc: 'Eksekusi dekripsi token di dalam AssemblyLoadContext terisolasi.' },
  { name: 'sharphy_patch_il', category: 'Mutation', desc: 'Patching instruksi CIL langsung dan re-emit biner tanpa source code asli.' },
  { name: 'sharphy_generate_wrapper', category: 'Mutation', desc: 'Otomatisasi wrapper C# strongly-typed performa tinggi via UnsafeAccessor.' },
];

export default function Home() {
  const [lang, setLang] = useState<'id' | 'en'>('id');
  const [copied, setCopied] = useState(false);

  const copyInstallCommand = () => {
    navigator.clipboard.writeText('dotnet tool install -g Sharphy.Cli');
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  return (
    <div className="flex min-h-screen flex-col">
      {/* Top Navbar */}
      <header className="sticky top-0 z-50 border-b border-[#30363d] bg-[#0d1117]/80 backdrop-blur-md">
        <div className="mx-auto flex max-w-7xl items-center justify-between px-6 py-4">
          <div className="flex items-center gap-3">
            <img src="/logo.png" alt="Sharphy Logo" className="h-9 w-9 rounded object-contain drop-shadow" />
            <div>
              <span className="text-lg font-bold tracking-tight text-white">Sharphy</span>
              <span className="ml-2 rounded-full border border-blue-500/30 bg-blue-500/10 px-2 py-0.5 text-xs text-blue-400">
                v0.1.0-preview
              </span>
            </div>
          </div>

          <div className="flex items-center gap-4">
            {/* Language Switcher */}
            <div className="flex rounded-lg border border-[#30363d] bg-[#161b22] p-1 text-xs">
              <button 
                onClick={() => setLang('id')} 
                className={`rounded px-2.5 py-1 transition-colors ${lang === 'id' ? 'bg-blue-600 font-semibold text-white' : 'text-[#8b949e] hover:text-white'}`}
              >
                ID
              </button>
              <button 
                onClick={() => setLang('en')} 
                className={`rounded px-2.5 py-1 transition-colors ${lang === 'en' ? 'bg-blue-600 font-semibold text-white' : 'text-[#8b949e] hover:text-white'}`}
              >
                EN
              </button>
            </div>

            <a 
              href="https://github.com/baharmaulidiii/sharphy" 
              target="_blank" 
              rel="noreferrer" 
              className="flex items-center gap-1.5 rounded-lg border border-[#30363d] bg-[#161b22] px-3 py-1.5 text-xs font-medium text-[#c9d1d9] transition-colors hover:border-[#8b949e] hover:text-white"
            >
              GitHub <ExternalLink className="h-3 w-3" />
            </a>
          </div>
        </div>
      </header>

      {/* Hero Section */}
      <section className="relative overflow-hidden border-b border-[#30363d] bg-gradient-to-b from-[#161b22] to-[#0d1117] px-6 py-16 text-center">
        <div className="mx-auto max-w-4xl">
          <div className="mb-4 inline-flex items-center gap-2 rounded-full border border-blue-500/30 bg-blue-500/10 px-3 py-1 text-xs font-medium text-blue-400">
            <Cpu className="h-3.5 w-3.5" />
            {lang === 'id' ? 'Model Context Protocol (MCP) untuk .NET CLR' : 'Native Model Context Protocol (MCP) for .NET CLR'}
          </div>

          <h1 className="text-3xl font-extrabold tracking-tight text-white sm:text-5xl">
            {lang === 'id' ? 'Sharphy: Jembatan Forensik & Inteligensi Runtime .NET' : 'Sharphy: CLR Intelligence & Runtime Forensic Bridge'}
          </h1>

          <p className="mx-auto mt-4 max-w-2xl text-base text-[#8b949e] sm:text-lg">
            {lang === 'id'
              ? 'Membekali AI coding agent dengan kemampuan deep runtime diagnostics, audit memory dump, rekonstruksi source code CIL, dan manipulasi biner secara aman.'
              : 'Empowering AI coding agents to diagnose memory crash dumps, decompile CIL instructions, inspect ECMA-335 metadata, and safely mutate binaries.'}
          </p>

          {/* Quick Install Banner */}
          <div className="mx-auto mt-8 flex max-w-md items-center justify-between rounded-lg border border-[#30363d] bg-[#0d1117] p-2 pl-4 shadow-xl">
            <code className="text-xs text-blue-400 sm:text-sm font-mono">dotnet tool install -g Sharphy.Cli</code>
            <button 
              onClick={copyInstallCommand} 
              className="flex items-center gap-1 rounded bg-[#21262d] px-3 py-1.5 text-xs font-semibold text-white transition-colors hover:bg-[#30363d]"
            >
              {copied ? <Check className="h-3.5 w-3.5 text-green-400" /> : <Copy className="h-3.5 w-3.5" />}
              {copied ? 'Copied' : 'Copy'}
            </button>
          </div>
        </div>
      </section>

      {/* 4 Pillars Grid */}
      <section className="mx-auto max-w-7xl px-6 py-16">
        <h2 className="mb-8 text-2xl font-bold tracking-tight text-white">
          {lang === 'id' ? '4 Pilar Arsitektur Sharphy' : '4 Core Architectural Pillars'}
        </h2>

        <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-4">
          <div className="rounded-xl border border-[#30363d] bg-[#161b22] p-6 shadow-sm">
            <div className="mb-4 inline-block rounded-lg bg-blue-500/10 p-2.5 text-blue-400">
              <Activity className="h-6 w-6" />
            </div>
            <h3 className="text-base font-semibold text-white">
              {lang === 'id' ? 'CLR Dump Forensics' : 'CLR Dump Forensics'}
            </h3>
            <p className="mt-2 text-xs leading-relaxed text-[#8b949e]">
              {lang === 'id' 
                ? 'Analisis crash dump (.dmp) Windows & Linux ELF via ClrMD. Deteksi deadlock, sync-block contention, dan LOH fragmentation.'
                : 'Automated Root Cause Analysis on Windows & Linux ELF memory dumps via ClrMD. Thread lock trace & LOH leak audit.'}
            </p>
          </div>

          <div className="rounded-xl border border-[#30363d] bg-[#161b22] p-6 shadow-sm">
            <div className="mb-4 inline-block rounded-lg bg-indigo-500/10 p-2.5 text-indigo-400">
              <FileCode className="h-6 w-6" />
            </div>
            <h3 className="text-base font-semibold text-white">
              {lang === 'id' ? 'Dekomplikasi & Metadata' : 'Decompilation & Metadata'}
            </h3>
            <p className="mt-2 text-xs leading-relaxed text-[#8b949e]">
              {lang === 'id'
                ? 'ILSpy engine + ECMA-335 parser untuk merekonstruksi method CIL menjadi C# modern beserta atribut kompilernya.'
                : 'ILSpy engine + ECMA-335 parser reconstructing clean C# methods, type hierarchies, and P/Invoke signatures.'}
            </p>
          </div>

          <div className="rounded-xl border border-[#30363d] bg-[#161b22] p-6 shadow-sm">
            <div className="mb-4 inline-block rounded-lg bg-emerald-500/10 p-2.5 text-emerald-400">
              <Boxes className="h-6 w-6" />
            </div>
            <h3 className="text-base font-semibold text-white">
              {lang === 'id' ? 'Anti-Obfuscation Unpacking' : 'Anti-Obfuscation Unpacking'}
            </h3>
            <p className="mt-2 text-xs leading-relaxed text-[#8b949e]">
              {lang === 'id'
                ? 'Analisis pola CFG flattening, proxy delegates, serta eksekusi decrypter di dalam AssemblyLoadContext terisolasi.'
                : 'Demangle CFG flattening, resolve proxy delegates, and safely run decryption routines in isolated AssemblyLoadContext.'}
            </p>
          </div>

          <div className="rounded-xl border border-[#30363d] bg-[#161b22] p-6 shadow-sm">
            <div className="mb-4 inline-block rounded-lg bg-amber-500/10 p-2.5 text-amber-400">
              <Cpu className="h-6 w-6" />
            </div>
            <h3 className="text-base font-semibold text-white">
              {lang === 'id' ? 'Mutasi CIL & Live Patch' : 'CIL Mutation & Live Patch'}
            </h3>
            <p className="mt-2 text-xs leading-relaxed text-[#8b949e]">
              {lang === 'id'
                ? 'Modifikasi instruksi CIL (nop-out, bypass logic) dan re-emit biner tanpa membutuhkan source code awal.'
                : 'Patch CIL opcodes directly and re-emit binaries without requiring original project source.'}
            </p>
          </div>
        </div>
      </section>

      {/* MCP Tools Catalog Table */}
      <section className="mx-auto max-w-7xl px-6 py-8">
        <h2 className="mb-6 text-2xl font-bold tracking-tight text-white">
          {lang === 'id' ? 'Katalog MCP Tools' : 'MCP Tools Catalog'}
        </h2>

        <div className="overflow-x-auto rounded-xl border border-[#30363d] bg-[#161b22]">
          <table className="w-full text-left text-xs">
            <thead className="border-b border-[#30363d] bg-[#0d1117] text-[#8b949e]">
              <tr>
                <th className="px-6 py-3.5 font-semibold text-white">Tool Name</th>
                <th className="px-6 py-3.5 font-semibold text-white">Category</th>
                <th className="px-6 py-3.5 font-semibold text-white">Description</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-[#30363d]">
              {toolsCatalog.map((tool) => (
                <tr key={tool.name} className="hover:bg-[#21262d]/50 transition-colors">
                  <td className="px-6 py-3.5 font-mono font-medium text-blue-400">{tool.name}</td>
                  <td className="px-6 py-3.5">
                    <span className="rounded-full border border-[#30363d] bg-[#0d1117] px-2.5 py-0.5 text-[11px] text-[#c9d1d9]">
                      {tool.category}
                    </span>
                  </td>
                  <td className="px-6 py-3.5 text-[#8b949e]">{tool.desc}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </section>

      {/* MCP Agent Configuration Example */}
      <section className="mx-auto max-w-7xl px-6 py-12">
        <h2 className="mb-4 text-2xl font-bold tracking-tight text-white">
          {lang === 'id' ? 'Konfigurasi MCP Server untuk AI Agent' : 'MCP Server Setup for AI Agents'}
        </h2>
        <p className="mb-4 text-xs text-[#8b949e]">
          {lang === 'id' 
            ? 'Tambahkan konfigurasi berikut ke file pengaturan Claude Code, Cursor, Windsurf, atau AGY CLI:' 
            : 'Add the following block to your agent configuration file (Cursor, Claude Code, Windsurf):'}
        </p>

        <div className="rounded-xl border border-[#30363d] bg-[#0d1117] p-4 font-mono text-xs text-[#c9d1d9] overflow-x-auto">
          <pre>{`{
  "mcpServers": {
    "sharphy": {
      "command": "sharphy",
      "args": ["mcp", "start"]
    }
  }
}`}</pre>
        </div>
      </section>

      {/* Footer */}
      <footer className="mt-auto border-t border-[#30363d] bg-[#161b22] px-6 py-8 text-center text-xs text-[#8b949e]">
        <div className="mx-auto max-w-7xl flex flex-col sm:flex-row items-center justify-between gap-4">
          <p>© 2026 Bahar Maulidi & Sharphy Contributors. Licensed under MIT.</p>
          <div className="flex items-center gap-6">
            <a href="https://github.com/baharmaulidiii/sharphy" className="hover:text-white transition-colors">GitHub</a>
            <a href="https://github.com/baharmaulidiii/sharphy/blob/main/DISCLAIMER.md" className="hover:text-white transition-colors">Disclaimer</a>
            <a href="https://github.com/baharmaulidiii/sharphy/blob/main/SECURITY.md" className="hover:text-white transition-colors">Security</a>
          </div>
        </div>
      </footer>
    </div>
  );
}
