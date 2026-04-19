"use client";

import { useRouter } from "next/navigation";

type AppHeaderProps = {
  title?: string;
  candidate?: string;
};

export default function AppHeader({
  title = "Interview",
  candidate = "Candidate",
}: AppHeaderProps) {
  const router = useRouter();

  return (
    <header className="h-[64px] w-full px-[5%] shadow-md bg-[var(--surface)] flex items-center justify-between">
      {/* Left */}
      <button
        onClick={() => router.back()}
        className="text-sm text-[var(--text-muted)] hover:text-[var(--text-primary)] transition"
      >
        ← Back
      </button>

      {/* Center */}
      <h1 className="text-sm font-medium text-[var(--text-primary)]">
        {title}
      </h1>

      {/* Right */}
      <div className="text-sm text-[var(--text-muted)]">{candidate}</div>
    </header>
  );
}
