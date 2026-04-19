"use client";

import { useEffect, useState } from "react";

type QuestionBarProps = {
  question: string;
  currentIndex: number;
  totalQuestions: number;
  className?: string;
};

export default function QuestionBar({
  question,
  currentIndex,
  totalQuestions,
  className = "",
}: QuestionBarProps) {
  const safeTotal = totalQuestions > 0 ? totalQuestions : 1;

  const [displayedText, setDisplayedText] = useState("");

  useEffect(() => {
    let index = 0;
    setDisplayedText("");

    const interval = setInterval(() => {
      index++;
      setDisplayedText(question.slice(0, index));

      if (index >= question.length) {
        clearInterval(interval);
      }
    }, 20); // speed (lower = faster)

    return () => clearInterval(interval);
  }, [question]);

  return (
    <section className={`surface px-5 py-5 ${className}`.trim()}>
      <div className="flex items-center justify-between gap-4">
        <p className="text-xs font-semibold uppercase tracking-[0.18em] text-[var(--text-muted)]">
          Current Question
        </p>

        <p className="text-sm font-medium text-[var(--text-primary)]">
          {Math.min(currentIndex + 1, safeTotal)} / {safeTotal}
        </p>
      </div>

      <p className="mt-4 text-base leading-7 text-[var(--text-primary)] sm:text-lg">
        {displayedText}
        <span className="ml-1 animate-pulse">|</span>
      </p>
    </section>
  );
}
