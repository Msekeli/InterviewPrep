import type { Metadata } from "next";
import "./globals.css";
import "../styles/theme.css";
import "../styles/base.css";
import "../styles/ui.css";
import { ThemeProvider } from "next-themes";

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
    <html lang="en" suppressHydrationWarning>
      <body>
        <ThemeProvider attribute="class" defaultTheme="light">
          <div className="min-h-screen w-full">{children}</div>
        </ThemeProvider>
      </body>
    </html>
  );
}
