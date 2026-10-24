import type { Metadata } from 'next';
import './globals.css';

export const metadata: Metadata = {
  metadataBase: new URL('https://baharmaulidiii.github.io/sharphy/'),
  title: 'Sharphy Docs - CLR Intelligence & Runtime Forensic Bridge',
  description: 'Native Model Context Protocol (MCP) server for deep .NET runtime diagnostics, memory forensics, and binary reverse engineering.',
  openGraph: {
    title: 'Sharphy - CLR Intelligence & Runtime Forensic Bridge',
    description: 'Empowering AI coding agents with deep .NET runtime diagnostics, crash dump root cause analysis, CIL decompilation, and isolated binary mutations.',
    url: 'https://baharmaulidiii.github.io/sharphy/',
    siteName: 'Sharphy Docs',
    images: [
      {
        url: 'social-preview.jpeg',
        width: 1280,
        height: 640,
        alt: 'Sharphy Social Preview Banner',
      },
    ],
    locale: 'id_ID',
    type: 'website',
  },
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
