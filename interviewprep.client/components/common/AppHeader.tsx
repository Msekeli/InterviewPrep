"use client";

import { useRouter } from "next/navigation";
import ThemeToggle from "./ThemeToggle";

type AppHeaderProps = {
  title?: string;
  stage?: "setup" | "interview" | "result";
};

export default function AppHeader({
  title = "Interview Session",
  stage = "setup",
}: AppHeaderProps) {
  const router = useRouter();

  const showBack = stage !== "setup";

  return (
    <header
      className="
        w-full
        border-b border-[var(--border-soft)]
        bg-[var(--bg)]
      "
    >
      {/* 🧱 CONTAINER (fixes left/right issue) */}
      <div
        className="
          h-[64px]
          max-w-6xl
          mx-auto
          px-6
          flex
          items-center
          justify-between
        "
      >
        {/* LEFT */}
        <div className="w-[120px]">
          {showBack ? (
            <button
              onClick={() => router.back()}
              className="
                text-sm
                text-[var(--text-muted)]
                hover:text-[var(--text-primary)]
                transition
              "
            >
              ← Back
            </button>
          ) : null}
        </div>

        {/* CENTER */}
        <h1 className="text-sm font-medium text-[var(--text-primary)]">
          {title}
        </h1>

        {/* RIGHT */}
        <div className="w-[120px] flex justify-end">
          <ThemeToggle />
        </div>
      </div>
    </header>
  );
}
