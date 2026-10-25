'use client';

import { ExternalLink } from 'lucide-react';
import { ThemeToggle } from './ThemeToggle';

export function Navbar() {
  const navItems = [
    { label: 'Architecture', href: '#features' },
    { label: 'Tools Catalog', href: '#tools' },
    { label: 'Agent Setup', href: '#setup' },
    { label: 'CI & Release', href: '#workflow' },
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
              v1.0.4
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
                className="hover:text-neutral-900 dark:hover:text-neutral-100 transition-colors"
              >
                {item.label}
              </a>
            ))}
          </nav>

          <div className="h-4 w-px bg-neutral-200 dark:bg-neutral-800 hidden md:block" />

          {/* GitHub Link */}
          <a
            href="https://github.com/baharmaulidiii/sharphy"
            target="_blank"
            rel="noopener noreferrer"
            className="flex items-center gap-1.5 rounded-md px-2.5 py-1 text-xs font-medium text-neutral-700 dark:text-neutral-300 hover:bg-neutral-100 dark:hover:bg-neutral-900 border border-neutral-200 dark:border-neutral-800 transition-colors"
          >
            <span>GitHub</span>
            <ExternalLink className="h-3 w-3 opacity-60" />
          </a>

          {/* Theme Toggle */}
          <ThemeToggle />
        </div>
      </div>
    </header>
  );
}
