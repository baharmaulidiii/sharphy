'use client';

import { useState } from 'react';
import { Navbar } from '@/components/Navbar';
import { 
  Check, 
  Copy, 
  Terminal, 
  Cpu, 
  ShieldCheck, 
  Binary, 
  Search, 
  GitFork, 
  Box, 
  KeyRound, 
  Wrench, 
  FileCode, 
  Layers, 
  Workflow, 
  Sparkles 
} from 'lucide-react';

export default function Home() {
  const [copiedCmd, setCopiedCmd] = useState<string | null>(null);

  const copyToClipboard = (text: string, id: string) => {
    navigator.clipboard.writeText(text);
    setCopiedCmd(id);
    setTimeout(() => setCopiedCmd(null), 2000);
  };

  const tools = [
    {
      name: 'sharphy_analyze_dump',
      category: 'Diagnostics',
      desc: 'Automatic Root Cause Analysis (RCA) on Windows and Linux crash dumps. Extracts thread callstacks and exception objects.',
    },
    {
      name: 'sharphy_inspect_heap',
      category: 'Diagnostics',
      desc: 'Deep managed heap analysis, GC generation allocations, memory leak retention graphs, and LOH fragmentation.',
    },
    {
      name: 'sharphy_deadlock_detector',
      category: 'Diagnostics',
      desc: 'Traces thread Monitor locks and SyncBlock synchronization caches to detect deadlock cycles.',
    },
    {
      name: 'sharphy_decompile_method',
      category: 'Decompiler',
      desc: 'High-fidelity decompilation of CIL bytecode into idiomatic modern C# using the embedded ILSpy engine.',
    },
    {
      name: 'sharphy_diff_assemblies',
      category: 'Decompiler',
      desc: 'Semantic ECMA-335 assembly comparison detecting added/removed types, breaking API changes, and CIL size shifts.',
    },
    {
      name: 'sharphy_clean_obfuscation',
      category: 'Decompiler',
      desc: 'Control Flow Graph (CFG) switch flattening demangling, proxy delegate resolution, and excessive NOP trimming.',
    },
    {
      name: 'sharphy_inspect_metadata',
      category: 'Bytecode',
      desc: 'ECMA-335 metadata hierarchy inspector, decoding TypeDef, MethodDef, FieldDef, custom attributes, and P/Invoke imports.',
    },
    {
      name: 'sharphy_extract_tokens',
      category: 'Bytecode',
      desc: 'Dynamic reflection inside an isolated, collectible AssemblyLoadContext without file locking or process pollution.',
    },
    {
      name: 'sharphy_patch_il',
      category: 'Bytecode',
      desc: 'Surgical CIL instruction patching, opcode replacement, branch offset adjustments, and re-emitting patched assemblies.',
    },
    {
      name: 'sharphy_generate_wrapper',
      category: 'Bytecode',
      desc: 'Synthesizes zero-overhead C# wrappers accessing private members and fields using .NET 8+ [UnsafeAccessor].',
    },
  ];

  return (
    <div className="flex min-h-screen flex-col bg-neutral-50 dark:bg-neutral-900 text-neutral-900 dark:text-neutral-100 selection:bg-blue-600 selection:text-white transition-colors">
      <Navbar />

      <main className="mx-auto w-full max-w-5xl px-6 py-12 sm:py-16 flex-1">
        {/* Hero Section */}
        <section className="text-center space-y-6">
          <div className="inline-flex items-center gap-2 rounded-full border border-blue-500/20 bg-blue-500/10 px-3 py-1 text-xs font-medium text-blue-600 dark:text-blue-400">
            <Sparkles className="h-3.5 w-3.5" />
            <span>v1.1.0 Released – Native MCP CLR Intelligence Bridge</span>
          </div>

          <h1 className="text-3xl sm:text-5xl font-extrabold tracking-tight text-neutral-900 dark:text-neutral-100 max-w-3xl mx-auto leading-tight">
            CLR Intelligence & Runtime Forensic Bridge for AI Coding Agents
          </h1>

          <p className="text-sm sm:text-base text-neutral-600 dark:text-neutral-400 max-w-2xl mx-auto leading-relaxed">
            Eliminate runtime blindness. Equip Cursor, Claude Code, and Windsurf with low-level CLR diagnostics, cross-platform memory crash dump root-cause analysis, and surgical CIL bytecode decompilation.
          </p>

          {/* Quick Install Bar */}
          <div className="pt-2 flex flex-col sm:flex-row items-center justify-center gap-3 max-w-md mx-auto">
            <div className="w-full flex items-center justify-between gap-3 bg-white dark:bg-neutral-950 border border-neutral-200 dark:border-neutral-800 rounded-lg px-4 py-2.5 font-mono text-xs text-neutral-800 dark:text-neutral-200 shadow-sm">
              <span className="truncate">dotnet tool install -g Sharphy.Cli</span>
              <button
                onClick={() => copyToClipboard('dotnet tool install -g Sharphy.Cli', 'install')}
                className="text-neutral-500 hover:text-neutral-900 dark:hover:text-neutral-100 transition-colors shrink-0"
                aria-label="Copy installation command"
              >
                {copiedCmd === 'install' ? <Check className="h-4 w-4 text-emerald-500" /> : <Copy className="h-4 w-4" />}
              </button>
            </div>
          </div>
        </section>

        {/* Architectural Pillars */}
        <section id="features" className="mt-20 space-y-6">
          <div className="border-b border-neutral-200 dark:border-neutral-800 pb-4">
            <h2 className="text-lg sm:text-xl font-bold tracking-tight text-neutral-900 dark:text-neutral-100">
              Architectural Pillars
            </h2>
            <p className="text-xs sm:text-sm text-neutral-500 dark:text-neutral-400">
              Deterministic CLR runtime diagnostics exposed directly to LLM semantic reasoning
            </p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="rounded-xl border border-neutral-200 dark:border-neutral-800 bg-white dark:bg-neutral-950 p-5 space-y-3 shadow-sm">
              <div className="h-9 w-9 rounded-lg bg-blue-500/10 text-blue-600 dark:text-blue-400 flex items-center justify-center">
                <Cpu className="h-5 w-5" />
              </div>
              <h3 className="font-semibold text-sm text-neutral-900 dark:text-neutral-100">
                Memory Crash Dump Forensics
              </h3>
              <p className="text-xs text-neutral-600 dark:text-neutral-400 leading-relaxed">
                Powered by Microsoft ClrMD. Automatically inspects Windows and Linux crash dumps, analyzes managed heap fragmentation, and constructs thread synchronization Wait-For Graphs.
              </p>
            </div>

            <div className="rounded-xl border border-neutral-200 dark:border-neutral-800 bg-white dark:bg-neutral-950 p-5 space-y-3 shadow-sm">
              <div className="h-9 w-9 rounded-lg bg-indigo-500/10 text-indigo-600 dark:text-indigo-400 flex items-center justify-center">
                <FileCode className="h-5 w-5" />
              </div>
              <h3 className="font-semibold text-sm text-neutral-900 dark:text-neutral-100">
                High-Fidelity CIL Decompilation
              </h3>
              <p className="text-xs text-neutral-600 dark:text-neutral-400 leading-relaxed">
                Embedded ILSpy decompiler engine transforms raw CIL opcode streams into modern, idiomatic C# source code, complete with async state machine and compiler attribute reconstruction.
              </p>
            </div>

            <div className="rounded-xl border border-neutral-200 dark:border-neutral-800 bg-white dark:bg-neutral-950 p-5 space-y-3 shadow-sm">
              <div className="h-9 w-9 rounded-lg bg-purple-500/10 text-purple-600 dark:text-purple-400 flex items-center justify-center">
                <Binary className="h-5 w-5" />
              </div>
              <h3 className="font-semibold text-sm text-neutral-900 dark:text-neutral-100">
                Bytecode Mutation & Sandbox
              </h3>
              <p className="text-xs text-neutral-600 dark:text-neutral-400 leading-relaxed">
                Mono.Cecil metadata inspection, surgical opcode patching, and dynamic reflection safely encapsulated within collectible AssemblyLoadContext sandboxes.
              </p>
            </div>
          </div>
        </section>

        {/* 10 MCP Tools Catalog */}
        <section id="tools" className="mt-20 space-y-6">
          <div className="border-b border-neutral-200 dark:border-neutral-800 pb-4">
            <h2 className="text-lg sm:text-xl font-bold tracking-tight text-neutral-900 dark:text-neutral-100">
              10 MCP Tools Catalog
            </h2>
            <p className="text-xs sm:text-sm text-neutral-500 dark:text-neutral-400">
              Standardized Model Context Protocol tool endpoints available via JSON-RPC 2.0 Stdio
            </p>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            {tools.map((tool) => (
              <div 
                key={tool.name}
                className="rounded-lg border border-neutral-200 dark:border-neutral-800 bg-white dark:bg-neutral-950 p-4 space-y-2 hover:border-blue-500/40 transition-colors shadow-sm"
              >
                <div className="flex items-center justify-between">
                  <span className="font-mono text-xs font-semibold text-blue-600 dark:text-blue-400">
                    {tool.name}
                  </span>
                  <span className="text-[10px] font-mono uppercase tracking-wider rounded bg-neutral-100 dark:bg-neutral-900 border border-neutral-200 dark:border-neutral-800 px-1.5 py-0.5 text-neutral-600 dark:text-neutral-400">
                    {tool.category}
                  </span>
                </div>
                <p className="text-xs text-neutral-600 dark:text-neutral-400 leading-relaxed">
                  {tool.desc}
                </p>
              </div>
            ))}
          </div>
        </section>

        {/* Agent Setup Guide */}
        <section id="setup" className="mt-20 space-y-6">
          <div className="border-b border-neutral-200 dark:border-neutral-800 pb-4">
            <h2 className="text-lg sm:text-xl font-bold tracking-tight text-neutral-900 dark:text-neutral-100">
              Agent Setup Guide
            </h2>
            <p className="text-xs sm:text-sm text-neutral-500 dark:text-neutral-400">
              Integrate Sharphy with Cursor, Claude Code, Windsurf, or Claude Desktop in seconds
            </p>
          </div>

          <div className="rounded-xl border border-neutral-200 dark:border-neutral-800 bg-white dark:bg-neutral-950 p-5 space-y-4 shadow-sm">
            <p className="text-xs text-neutral-600 dark:text-neutral-400">
              Add the following JSON-RPC configuration to your agent config file:
            </p>

            <div className="relative rounded-lg bg-neutral-900 text-neutral-100 p-4 font-mono text-xs overflow-x-auto">
              <pre>{`{
  "mcpServers": {
    "sharphy": {
      "command": "sharphy",
      "args": ["mcp", "start"]
    }
  }
}`}</pre>
              <button
                onClick={() => copyToClipboard('{\n  "mcpServers": {\n    "sharphy": {\n      "command": "sharphy",\n      "args": ["mcp", "start"]\n    }\n  }\n}', 'config')}
                className="absolute top-3 right-3 text-neutral-400 hover:text-white transition-colors"
                aria-label="Copy configuration"
              >
                {copiedCmd === 'config' ? <Check className="h-4 w-4 text-emerald-400" /> : <Copy className="h-4 w-4" />}
              </button>
            </div>
          </div>
        </section>

        {/* Multi-Targeting Architecture */}
        <section id="workflow" className="mt-20 space-y-4 rounded-xl border border-neutral-200 dark:border-neutral-800 bg-white dark:bg-neutral-950 p-6 shadow-sm">
          <h2 className="text-base sm:text-lg font-bold tracking-tight text-neutral-900 dark:text-neutral-100">
            Multi-Targeting .NET & Automated GitHub Actions
          </h2>
          <p className="text-xs sm:text-sm text-neutral-600 dark:text-neutral-400 leading-relaxed">
            Sharphy delivers native multi-targeting across .NET 8.0 (LTS), .NET 9.0 (STS), and .NET 10.0 (Latest). Build checks, unit test suites, NuGet packaging, and web docs deployments are orchestrated through the Release Orchestrator workflow.
          </p>
        </section>
      </main>

      {/* Minimalist Footer */}
      <footer className="mt-auto border-t border-neutral-200 dark:border-neutral-800 bg-white dark:bg-neutral-950 px-6 py-8 text-xs text-neutral-500 dark:text-neutral-400 transition-colors">
        <div className="mx-auto max-w-5xl flex flex-col sm:flex-row items-center justify-between gap-4">
          <p>© 2026 Moh. Baharudin Yuda Maulidi. Apache License 2.0.</p>
          <div className="flex items-center gap-5">
            <a href="https://github.com/baharmaulidiii/sharphy" className="hover:text-neutral-900 dark:hover:text-neutral-100 transition-colors">GitHub</a>
            <a href="https://github.com/baharmaulidiii/sharphy/blob/main/CHANGELOG.md" className="hover:text-neutral-900 dark:hover:text-neutral-100 transition-colors">Changelog</a>
            <a href="https://github.com/baharmaulidiii/sharphy/blob/main/TRADEMARK.md" className="hover:text-neutral-900 dark:hover:text-neutral-100 transition-colors">Trademark</a>
            <a href="https://github.com/baharmaulidiii/sharphy/blob/main/DISCLAIMER.md" className="hover:text-neutral-900 dark:hover:text-neutral-100 transition-colors">Disclaimer</a>
            <a href="https://github.com/baharmaulidiii/sharphy/blob/main/SECURITY.md" className="hover:text-neutral-900 dark:hover:text-neutral-100 transition-colors">Security</a>
          </div>
        </div>
      </footer>
    </div>
  );
}
