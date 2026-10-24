'use client';

import { useState, useEffect, useMemo } from 'react';
import { 
  Cpu, 
  Search, 
  ExternalLink, 
  Copy, 
  Check, 
  FileCode,
  Activity,
  Boxes,
  Terminal,
  ShieldCheck,
  Workflow,
  Sparkles,
  ArrowRight
} from 'lucide-react';
import { Navbar } from '@/components/Navbar';

interface ToolItem {
  name: string;
  category: 'Diagnostics' | 'Reconstruction' | 'Metadata' | 'Integrity' | 'Mutation';
  descId: string;
  descEn: string;
}

const toolsCatalog: ToolItem[] = [
  { 
    name: 'sharphy_analyze_dump', 
    category: 'Diagnostics', 
    descId: 'Root Cause Analysis (RCA) otomatis pada crash dump (.dmp) Windows / Linux ELF via ClrMD.',
    descEn: 'Automated Root Cause Analysis (RCA) on Windows / Linux ELF memory crash dumps via ClrMD.'
  },
  { 
    name: 'sharphy_inspect_heap', 
    category: 'Diagnostics', 
    descId: 'Audit managed heap, deteksi memory leak, dan fragmentasi Large Object Heap (LOH).',
    descEn: 'Managed heap audit, memory leak detection, and Large Object Heap (LOH) fragmentation analysis.'
  },
  { 
    name: 'sharphy_deadlock_detector', 
    category: 'Diagnostics', 
    descId: 'Pelacakan rantai thread lock dan sync-block lock contention antar thread managed.',
    descEn: 'Thread lock cycle graph traversal and sync-block contention detection across managed threads.'
  },
  { 
    name: 'sharphy_decompile_method', 
    category: 'Reconstruction', 
    descId: 'Dekompilasi method CIL target menjadi kode C# bersih modern via engine ILSpy terintegrasi.',
    descEn: 'Decompiles target CIL method into clean, modern C# source code utilizing ILSpy engine.'
  },
  { 
    name: 'sharphy_diff_assemblies', 
    category: 'Metadata', 
    descId: 'Komparasi biner dua versi assembly .dll/.exe untuk mendeteksi perubahan API dan breaking changes.',
    descEn: 'Binary comparison between two assembly versions to detect public surface mutations and breaking changes.'
  },
  { 
    name: 'sharphy_inspect_metadata', 
    category: 'Metadata', 
    descId: 'Inspeksi hierarki ECMA-335, signature P/Invoke, attribute metadata, dan embedded resources.',
    descEn: 'Rigid ECMA-335 metadata hierarchy, P/Invoke signatures, attributes, and embedded resource inspection.'
  },
  { 
    name: 'sharphy_clean_obfuscation', 
    category: 'Integrity', 
    descId: 'Pembersihan teknik obfuscation dasar (CFG flattening, string encryption, proxy delegates).',
    descEn: 'De-obfuscates control flow flattening, decrypts literals, and unwraps proxy delegates.'
  },
  { 
    name: 'sharphy_extract_tokens', 
    category: 'Integrity', 
    descId: 'Eksekusi dekripsi token di dalam AssemblyLoadContext kolektibel terisolasi secara aman.',
    descEn: 'Safely runs token decryption routines inside an isolated, collectible AssemblyLoadContext sandbox.'
  },
  { 
    name: 'sharphy_patch_il', 
    category: 'Mutation', 
    descId: 'Patching instruksi CIL langsung (bypass logic / nop-out) dan re-emit biner via Mono.Cecil.',
    descEn: 'Direct CIL instruction injection/patching and binary re-emitting powered by Mono.Cecil.'
  },
  { 
    name: 'sharphy_generate_wrapper', 
    category: 'Mutation', 
    descId: 'Otomatisasi wrapper C# strongly-typed performa tinggi via UnsafeAccessor (.NET 8+) untuk private members.',
    descEn: 'Zero-overhead strongly-typed C# wrapper generation via [UnsafeAccessor] targeting private members.'
  },
];

export default function Home() {
  const [lang, setLang] = useState<'id' | 'en'>('id');
  const [copiedInstall, setCopiedInstall] = useState(false);
  const [copiedConfig, setCopiedConfig] = useState(false);
  const [copiedTool, setCopiedTool] = useState<string | null>(null);
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedCategory, setSelectedCategory] = useState<string>('All');

  // Prefix handling for GitHub Pages asset resolution
  const basePath = typeof window !== 'undefined' && window.location.pathname.startsWith('/sharphy') ? '/sharphy' : '';

  useEffect(() => {
    const savedLang = localStorage.getItem('sharphy-lang') as 'id' | 'en' | null;
    if (savedLang) {
      setLang(savedLang);
    }
  }, []);

  const handleSetLang = (newLang: 'id' | 'en') => {
    setLang(newLang);
    localStorage.setItem('sharphy-lang', newLang);
  };

  const copyInstall = () => {
    navigator.clipboard.writeText('dotnet tool install -g Sharphy.Cli');
    setCopiedInstall(true);
    setTimeout(() => setCopiedInstall(false), 2000);
  };

  const mcpConfigJson = `{
  "mcpServers": {
    "sharphy": {
      "command": "sharphy",
      "args": ["mcp", "start"]
    }
  }
}`;

  const copyConfig = () => {
    navigator.clipboard.writeText(mcpConfigJson);
    setCopiedConfig(true);
    setTimeout(() => setCopiedConfig(false), 2000);
  };

  const copyToolName = (name: string) => {
    navigator.clipboard.writeText(name);
    setCopiedTool(name);
    setTimeout(() => setCopiedTool(null), 2000);
  };

  const categories = ['All', 'Diagnostics', 'Reconstruction', 'Metadata', 'Integrity', 'Mutation'];

  const filteredTools = useMemo(() => {
    return toolsCatalog.filter((tool) => {
      const matchesCategory = selectedCategory === 'All' || tool.category === selectedCategory;
      const desc = lang === 'id' ? tool.descId : tool.descEn;
      const matchesSearch = 
        tool.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
        desc.toLowerCase().includes(searchQuery.toLowerCase());
      return matchesCategory && matchesSearch;
    });
  }, [searchQuery, selectedCategory, lang]);

  return (
    <div className="flex min-h-screen flex-col bg-[#fdfdfd] dark:bg-[#0a0a0a] text-neutral-900 dark:text-neutral-100 transition-colors">
      <Navbar lang={lang} setLang={handleSetLang} />

      <main className="flex-1 max-w-5xl w-full mx-auto px-6 py-10 space-y-16 sm:space-y-24">
        {/* Hero Section */}
        <section className="text-center pt-6 sm:pt-10">
          <div className="inline-flex items-center gap-2 rounded-full border border-neutral-200 dark:border-neutral-800 bg-neutral-100 dark:bg-neutral-900/80 px-3 py-1 text-xs font-medium text-neutral-600 dark:text-neutral-400 mb-6">
            <Sparkles className="h-3.5 w-3.5 text-blue-500" />
            <span>{lang === 'id' ? 'Protokol MCP Resmi untuk .NET CLR' : 'Native Model Context Protocol for .NET CLR'}</span>
          </div>

          <h1 className="text-3xl font-bold tracking-tight sm:text-5xl max-w-3xl mx-auto leading-tight sm:leading-tight">
            {lang === 'id' 
              ? 'Inteligensi Runtime & Jembatan Forensik CLR untuk AI Coding Agents' 
              : 'CLR Intelligence & Runtime Forensic Bridge for AI Coding Agents'}
          </h1>

          <p className="mt-5 text-sm sm:text-base text-neutral-600 dark:text-neutral-400 max-w-2xl mx-auto leading-relaxed">
            {lang === 'id'
              ? 'Sharphy membekali AI agents (Cursor, Claude Code, Windsurf) dengan kapabilitas forensik memori, audit crash dump, dekompilasi CIL, dan mutasi biner terisolasi.'
              : 'Sharphy empowers AI agents (Cursor, Claude Code, Windsurf) with deep memory forensics, crash dump root cause analysis, CIL decompilation, and isolated binary mutations.'}
          </p>

          {/* Quick Install Banner */}
          <div className="mt-8 flex flex-col sm:flex-row items-center justify-center gap-3">
            <div className="flex items-center justify-between gap-3 w-full sm:w-auto rounded-lg border border-neutral-200 dark:border-neutral-800 bg-white dark:bg-neutral-900 px-4 py-2 text-xs font-mono shadow-sm">
              <span className="text-blue-600 dark:text-blue-400">dotnet tool install -g Sharphy.Cli</span>
              <button 
                onClick={copyInstall}
                aria-label="Copy install command"
                className="flex items-center gap-1 rounded bg-neutral-100 dark:bg-neutral-800 px-2 py-1 text-[11px] font-sans font-medium text-neutral-700 dark:text-neutral-300 transition-colors hover:bg-neutral-200 dark:hover:bg-neutral-700"
              >
                {copiedInstall ? <Check className="h-3.5 w-3.5 text-emerald-500" /> : <Copy className="h-3.5 w-3.5" />}
                <span>{copiedInstall ? (lang === 'id' ? 'Disalin' : 'Copied') : (lang === 'id' ? 'Salin' : 'Copy')}</span>
              </button>
            </div>

            <a
              href="#tools"
              className="inline-flex items-center gap-1.5 rounded-lg border border-neutral-200 dark:border-neutral-800 bg-neutral-900 text-white dark:bg-white dark:text-neutral-900 px-4 py-2 text-xs font-medium transition-colors hover:opacity-90 w-full sm:w-auto justify-center"
            >
              <span>{lang === 'id' ? 'Eksplorasi Tools' : 'Explore Tools'}</span>
              <ArrowRight className="h-3.5 w-3.5" />
            </a>
          </div>
        </section>

        {/* 4 Architectural Pillars Grid */}
        <section id="features" className="space-y-6">
          <div>
            <h2 className="text-xl font-bold tracking-tight text-neutral-900 dark:text-neutral-100">
              {lang === 'id' ? '4 Pilar Arsitektur Forensik' : '4 Forensic Architecture Pillars'}
            </h2>
            <p className="mt-1 text-xs sm:text-sm text-neutral-500 dark:text-neutral-400">
              {lang === 'id' ? 'Fondasi analisis mendalam runtime .NET berbasis ECMA-335 dan ClrMD.' : 'Low-level .NET runtime diagnostics built on ECMA-335 standards and ClrMD.'}
            </p>
          </div>

          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
            {/* Pillar 1 */}
            <div className="rounded-xl border border-neutral-200/80 dark:border-neutral-800/80 bg-white dark:bg-neutral-900/60 p-5 transition-all hover:border-neutral-300 dark:hover:border-neutral-700 shadow-sm">
              <div className="mb-3 inline-block rounded-lg bg-blue-500/10 p-2 text-blue-600 dark:text-blue-400">
                <Activity className="h-5 w-5" />
              </div>
              <h3 className="font-semibold text-sm text-neutral-900 dark:text-neutral-100">
                {lang === 'id' ? 'CLR Dump Forensics' : 'CLR Dump Forensics'}
              </h3>
              <p className="mt-2 text-xs text-neutral-600 dark:text-neutral-400 leading-relaxed">
                {lang === 'id'
                  ? 'Audit crash dump Windows & Linux ELF. Deteksi deadlock sync-block dan fragmentasi Large Object Heap (LOH).'
                  : 'Automated RCA on crash dumps. Unravels deadlock lock graphs and Large Object Heap (LOH) leaks.'}
              </p>
            </div>

            {/* Pillar 2 */}
            <div className="rounded-xl border border-neutral-200/80 dark:border-neutral-800/80 bg-white dark:bg-neutral-900/60 p-5 transition-all hover:border-neutral-300 dark:hover:border-neutral-700 shadow-sm">
              <div className="mb-3 inline-block rounded-lg bg-indigo-500/10 p-2 text-indigo-600 dark:text-indigo-400">
                <FileCode className="h-5 w-5" />
              </div>
              <h3 className="font-semibold text-sm text-neutral-900 dark:text-neutral-100">
                {lang === 'id' ? 'Dekomplikasi CIL' : 'CIL Decompilation'}
              </h3>
              <p className="mt-2 text-xs text-neutral-600 dark:text-neutral-400 leading-relaxed">
                {lang === 'id'
                  ? 'Rekonstruksi method CIL menjadi source code C# modern lengkap dengan hierarki tipe dan signature P/Invoke.'
                  : 'Reconstructs clean, modern C# source code from raw CIL bytecode with full ECMA-335 type hierarchies.'}
              </p>
            </div>

            {/* Pillar 3 */}
            <div className="rounded-xl border border-neutral-200/80 dark:border-neutral-800/80 bg-white dark:bg-neutral-900/60 p-5 transition-all hover:border-neutral-300 dark:hover:border-neutral-700 shadow-sm">
              <div className="mb-3 inline-block rounded-lg bg-emerald-500/10 p-2 text-emerald-600 dark:text-emerald-400">
                <Boxes className="h-5 w-5" />
              </div>
              <h3 className="font-semibold text-sm text-neutral-900 dark:text-neutral-100">
                {lang === 'id' ? 'Anti-Obfuscation Sandbox' : 'Sandbox Anti-Obfuscation'}
              </h3>
              <p className="mt-2 text-xs text-neutral-600 dark:text-neutral-400 leading-relaxed">
                {lang === 'id'
                  ? 'Inspeksi biner dan eksekusi rutin dekripsi token di dalam AssemblyLoadContext kolektibel terisolasi.'
                  : 'Demangles CFG flattening and executes decryption tokens safely within collectible ALC sandboxes.'}
              </p>
            </div>

            {/* Pillar 4 */}
            <div className="rounded-xl border border-neutral-200/80 dark:border-neutral-800/80 bg-white dark:bg-neutral-900/60 p-5 transition-all hover:border-neutral-300 dark:hover:border-neutral-700 shadow-sm">
              <div className="mb-3 inline-block rounded-lg bg-amber-500/10 p-2 text-amber-600 dark:text-amber-400">
                <Cpu className="h-5 w-5" />
              </div>
              <h3 className="font-semibold text-sm text-neutral-900 dark:text-neutral-100">
                {lang === 'id' ? 'Mutasi CIL & Wrapper' : 'CIL Mutation & Wrappers'}
              </h3>
              <p className="mt-2 text-xs text-neutral-600 dark:text-neutral-400 leading-relaxed">
                {lang === 'id'
                  ? 'Modifikasi instruksi CIL langsung dan pembuatan wrapper strongly-typed via UnsafeAccessor (.NET 8+).'
                  : 'Direct opcode patching via Mono.Cecil and zero-overhead private access wrappers using [UnsafeAccessor].'}
              </p>
            </div>
          </div>
        </section>

        {/* MCP Tools Catalog (Interactive Search & Filters) */}
        <section id="tools" className="space-y-6">
          <div className="flex flex-col sm:flex-row sm:items-end justify-between gap-4">
            <div>
              <h2 className="text-xl font-bold tracking-tight text-neutral-900 dark:text-neutral-100">
                {lang === 'id' ? 'Katalog 10 MCP Tools Forensik' : '10 Diagnostic MCP Tools Catalog'}
              </h2>
              <p className="mt-1 text-xs sm:text-sm text-neutral-500 dark:text-neutral-400">
                {lang === 'id' ? 'Dapat dipanggil langsung oleh AI agent melalui protokol JSON-RPC 2.0.' : 'Available for seamless invocation by AI agents over JSON-RPC 2.0.'}
              </p>
            </div>

            {/* Search Input */}
            <div className="relative w-full sm:w-64">
              <Search className="absolute left-3 top-2.5 h-3.5 w-3.5 text-neutral-400" />
              <input
                type="text"
                placeholder={lang === 'id' ? 'Cari tool atau fungsi...' : 'Search tools or keywords...'}
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="w-full rounded-lg border border-neutral-200 dark:border-neutral-800 bg-white dark:bg-neutral-900 pl-9 pr-3 py-1.5 text-xs text-neutral-900 dark:text-neutral-100 focus:outline-none focus:ring-1 focus:ring-blue-500"
              />
            </div>
          </div>

          {/* Category Filter Pills */}
          <div className="flex flex-wrap items-center gap-1.5 text-xs">
            {categories.map((cat) => (
              <button
                key={cat}
                onClick={() => setSelectedCategory(cat)}
                className={`rounded-full px-3 py-1 font-medium transition-colors ${
                  selectedCategory === cat
                    ? 'bg-neutral-900 text-white dark:bg-neutral-100 dark:text-neutral-900'
                    : 'border border-neutral-200 dark:border-neutral-800 bg-white dark:bg-neutral-900 text-neutral-600 dark:text-neutral-400 hover:border-neutral-300 dark:hover:border-neutral-700'
                }`}
              >
                {cat}
              </button>
            ))}
          </div>

          {/* Tools List */}
          <div className="overflow-hidden rounded-xl border border-neutral-200 dark:border-neutral-800 bg-white dark:bg-neutral-900 shadow-sm">
            <div className="divide-y divide-neutral-200 dark:divide-neutral-800">
              {filteredTools.length > 0 ? (
                filteredTools.map((tool) => (
                  <div 
                    key={tool.name} 
                    className="p-4 sm:px-6 flex flex-col sm:flex-row sm:items-center justify-between gap-3 hover:bg-neutral-50 dark:hover:bg-neutral-800/40 transition-colors"
                  >
                    <div className="space-y-1">
                      <div className="flex items-center gap-2">
                        <span className="font-mono text-xs font-semibold text-blue-600 dark:text-blue-400">
                          {tool.name}
                        </span>
                        <span className="rounded-full border border-neutral-200 dark:border-neutral-800 bg-neutral-100 dark:bg-neutral-800 px-2 py-0.5 text-[10px] text-neutral-600 dark:text-neutral-400">
                          {tool.category}
                        </span>
                      </div>
                      <p className="text-xs text-neutral-600 dark:text-neutral-400">
                        {lang === 'id' ? tool.descId : tool.descEn}
                      </p>
                    </div>

                    <button
                      onClick={() => copyToolName(tool.name)}
                      className="self-start sm:self-auto flex items-center gap-1 rounded border border-neutral-200 dark:border-neutral-800 bg-neutral-50 dark:bg-neutral-800 px-2.5 py-1 text-[11px] font-medium text-neutral-600 dark:text-neutral-300 hover:bg-neutral-100 dark:hover:bg-neutral-700 transition-colors"
                    >
                      {copiedTool === tool.name ? (
                        <>
                          <Check className="h-3 w-3 text-emerald-500" />
                          <span>{lang === 'id' ? 'Disalin' : 'Copied'}</span>
                        </>
                      ) : (
                        <>
                          <Copy className="h-3 w-3 text-neutral-400" />
                          <span>{lang === 'id' ? 'Salin Nama' : 'Copy'}</span>
                        </>
                      )}
                    </button>
                  </div>
                ))
              ) : (
                <div className="p-8 text-center text-xs text-neutral-500 dark:text-neutral-400">
                  {lang === 'id' ? 'Tidak ada tool yang cocok dengan pencarian.' : 'No tools matched your criteria.'}
                </div>
              )}
            </div>
          </div>
        </section>

        {/* MCP Setup for AI Agents */}
        <section id="setup" className="space-y-4">
          <div>
            <h2 className="text-xl font-bold tracking-tight text-neutral-900 dark:text-neutral-100">
              {lang === 'id' ? 'Setup MCP Server untuk AI Agent' : 'MCP Server Setup for AI Agents'}
            </h2>
            <p className="mt-1 text-xs sm:text-sm text-neutral-500 dark:text-neutral-400">
              {lang === 'id'
                ? 'Salin konfigurasi berikut ke file settings Claude Code, Cursor, Windsurf, atau AGY CLI:'
                : 'Add this snippet to your Claude Code, Cursor, Windsurf, or AGY agent configuration:'}
            </p>
          </div>

          <div className="relative rounded-xl border border-neutral-200 dark:border-neutral-800 bg-neutral-950 p-4 font-mono text-xs text-neutral-200 shadow-sm overflow-x-auto">
            <button
              onClick={copyConfig}
              className="absolute right-3 top-3 flex items-center gap-1 rounded bg-neutral-800 hover:bg-neutral-700 px-2.5 py-1 text-[11px] font-sans font-medium text-neutral-300 transition-colors"
            >
              {copiedConfig ? <Check className="h-3.5 w-3.5 text-emerald-400" /> : <Copy className="h-3.5 w-3.5" />}
              <span>{copiedConfig ? (lang === 'id' ? 'Disalin' : 'Copied') : (lang === 'id' ? 'Salin JSON' : 'Copy JSON')}</span>
            </button>
            <pre className="pt-2">{mcpConfigJson}</pre>
          </div>
        </section>

        {/* Multi-Targeting & Release Orchestrator */}
        <section id="workflow" className="rounded-xl border border-neutral-200 dark:border-neutral-800 bg-neutral-50 dark:bg-neutral-900/40 p-6 sm:p-8 space-y-4 shadow-sm">
          <div className="flex items-center gap-2 text-blue-600 dark:text-blue-400 font-medium text-xs">
            <Workflow className="h-4 w-4" />
            <span>{lang === 'id' ? 'Siklus Hidup Rilis Terpadu' : 'Unified Release Lifecycle'}</span>
          </div>
          <h2 className="text-lg sm:text-xl font-bold tracking-tight text-neutral-900 dark:text-neutral-100">
            {lang === 'id' ? 'Multi-Targeting .NET & Otomatisasi GitHub Actions' : 'Multi-Targeting .NET & Automated GitHub Actions'}
          </h2>
          <p className="text-xs sm:text-sm text-neutral-600 dark:text-neutral-400 leading-relaxed">
            {lang === 'id'
              ? 'Sharphy mendukung multi-targeting native lintas .NET 8.0 (LTS), .NET 9.0 (STS), dan .NET 10.0 (Latest). Seluruh siklus verifikasi build, pengujian unit, packaging NuGet, dan deploy web docs diorkestrasi secara terpusat melalui workflow Release Orchestrator.'
              : 'Sharphy provides native multi-targeting across .NET 8.0 (LTS), .NET 9.0 (STS), and .NET 10.0 (Latest). Verification builds, unit tests, NuGet packaging, and docs deployments are orchestrated through the Release Orchestrator workflow.'}
          </p>
        </section>
      </main>

      {/* Minimalist Footer */}
      <footer className="mt-auto border-t border-neutral-200 dark:border-neutral-800 bg-white dark:bg-neutral-950 px-6 py-8 text-xs text-neutral-500 dark:text-neutral-400 transition-colors">
        <div className="mx-auto max-w-5xl flex flex-col sm:flex-row items-center justify-between gap-4">
          <p>© 2026 Bahar Maulidi & Sharphy Contributors. MIT License.</p>
          <div className="flex items-center gap-5">
            <a href="https://github.com/baharmaulidiii/sharphy" className="hover:text-neutral-900 dark:hover:text-neutral-100 transition-colors">GitHub</a>
            <a href="https://github.com/baharmaulidiii/sharphy/blob/main/CHANGELOG.md" className="hover:text-neutral-900 dark:hover:text-neutral-100 transition-colors">Changelog</a>
            <a href="https://github.com/baharmaulidiii/sharphy/blob/main/DISCLAIMER.md" className="hover:text-neutral-900 dark:hover:text-neutral-100 transition-colors">Disclaimer</a>
            <a href="https://github.com/baharmaulidiii/sharphy/blob/main/SECURITY.md" className="hover:text-neutral-900 dark:hover:text-neutral-100 transition-colors">Security</a>
          </div>
        </div>
      </footer>
    </div>
  );
}
