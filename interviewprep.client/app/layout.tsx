import type { Metadata } from "next";
import localFont from "next/font/local";
import "./globals.css";
import "../styles/theme.css";
import "../styles/base.css";
import "../styles/ui.css";
import { ThemeProvider } from "next-themes";

const geistSans = localFont({
  src: "./fonts/GeistVF.woff",
  variable: "--font-geist-sans",
  weight: "100 900",
});

const geistMono = localFont({
  src: "./fonts/GeistMonoVF.woff",
  variable: "--font-geist-mono",
  weight: "100 900",
});

export const metadata: Metadata = {
  title: "Interview Prep",
  description: "A minimal AI-powered interview practice experience.",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html
      lang="en"
      suppressHydrationWarning
      className={`${geistSans.variable} ${geistMono.variable}`}
    >
      <body>
        <ThemeProvider attribute="class" defaultTheme="light">
          <div className="min-h-screen w-full">{children}</div>
        </ThemeProvider>
      </body>
    </html>
  );
}
