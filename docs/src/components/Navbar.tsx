'use client';

import { ExternalLink } from 'lucide-react';
import { ThemeToggle } from './ThemeToggle';

interface NavbarProps {
  lang: 'id' | 'en';
  setLang: (lang: 'id' | 'en') => void;
  brandPrefix?: string;
}

export function Navbar({ lang, setLang }: NavbarProps) {
  const navItems = [
    { label: lang === 'id' ? 'Arsitektur' : 'Architecture', href: '#features' },
    { label: lang === 'id' ? 'Katalog Tools' : 'Tools Catalog', href: '#tools' },
    { label: lang === 'id' ? 'Setup Agen' : 'Agent Setup', href: '#setup' },
    { label: lang === 'id' ? 'Rilis & CI' : 'CI & Release', href: '#workflow' },
  ];

  return (
    <header className="sticky top-0 z-50 w-full backdrop-blur-md bg-white/75 dark:bg-neutral-950/75 border-b border-neutral-200/70 dark:border-neutral-800/70 transition-colors">
      <div className="mx-auto flex max-w-5xl items-center justify-between px-6 h-14">
        {/* Brand Logo & Name */}
        <a href="#" className="flex items-center gap-2.5 hover:opacity-85 transition-opacity">
          <img 
            src="sharphy.svg" 
            alt="Sharphy Monogram" 
            className="h-7 w-7 rounded object-contain drop-shadow-sm" 
            onError={(e) => {
              // Try absolute path if relative fails
              const target = e.currentTarget;
              if (target.src.endsWith('sharphy.svg') && !target.src.includes('/sharphy/')) {
                target.src = '/sharphy/sharphy.svg';
              }
            }}
          />
          <div className="flex items-center gap-2">
            <span className="font-semibold tracking-tight text-neutral-900 dark:text-neutral-100 text-sm sm:text-base">
              Sharphy
            </span>
            <span className="rounded-full border border-blue-500/30 bg-blue-500/10 px-2 py-0.5 text-[10px] font-mono font-medium text-blue-600 dark:text-blue-400">
              v0.1.1-preview
            </span>
          </div>
        </a>

        {/* Right Navigation */}
        <div className="flex items-center gap-2 sm:gap-4">
          <nav className="hidden md:flex items-center gap-4 text-xs font-medium text-neutral-600 dark:text-neutral-400">
            {navItems.map((item) => (
              <a
                key={item.href}
                href={item.href}
                className="hover:text-neutral-950 dark:hover:text-neutral-100 transition-colors py-1"
              >
                {item.label}
              </a>
            ))}
          </nav>

          <div className="flex items-center gap-2 pl-2 border-l border-neutral-200 dark:border-neutral-800">
            {/* Language Switcher */}
            <div className="flex rounded-lg border border-neutral-200 dark:border-neutral-800 bg-neutral-100 dark:bg-neutral-900 p-0.5 text-xs font-medium">
              <button
                onClick={() => setLang('id')}
                className={`rounded-md px-2 py-1 transition-colors ${
                  lang === 'id'
                    ? 'bg-white dark:bg-neutral-800 font-semibold text-neutral-900 dark:text-neutral-100 shadow-sm'
                    : 'text-neutral-500 dark:text-neutral-400 hover:text-neutral-900 dark:hover:text-neutral-200'
                }`}
              >
                ID
              </button>
              <button
                onClick={() => setLang('en')}
                className={`rounded-md px-2 py-1 transition-colors ${
                  lang === 'en'
                    ? 'bg-white dark:bg-neutral-800 font-semibold text-neutral-900 dark:text-neutral-100 shadow-sm'
                    : 'text-neutral-500 dark:text-neutral-400 hover:text-neutral-900 dark:hover:text-neutral-200'
                }`}
              >
                EN
              </button>
            </div>

            {/* Theme Toggle */}
            <ThemeToggle />

            {/* GitHub Link */}
            <a
              href="https://github.com/baharmaulidiii/sharphy"
              target="_blank"
              rel="noreferrer"
              className="flex items-center gap-1 rounded-lg border border-neutral-200 dark:border-neutral-800 bg-neutral-100 dark:bg-neutral-900 px-2.5 py-1 text-xs font-medium text-neutral-700 dark:text-neutral-300 transition-colors hover:border-neutral-300 dark:hover:border-neutral-700 hover:text-neutral-950 dark:hover:text-white"
            >
              <span>GitHub</span>
              <ExternalLink className="h-3 w-3 text-neutral-400" />
            </a>
          </div>
        </div>
      </div>
    </header>
  );
}
