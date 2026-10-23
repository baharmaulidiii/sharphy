import type { Metadata } from 'next';
import './globals.css';

export const metadata: Metadata = {
  title: 'Sharphy Docs - CLR Intelligence & Runtime Forensic Bridge',
  description: 'Native Model Context Protocol (MCP) server for deep .NET runtime diagnostics and binary forensics.',
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="id">
      <body className="min-h-screen bg-[#0d1117] text-[#c9d1d9] antialiased selection:bg-blue-600 selection:text-white">
        {children}
      </body>
    </html>
  );
}
