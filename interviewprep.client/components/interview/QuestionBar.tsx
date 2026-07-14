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
    }, 20);

    return () => clearInterval(interval);
  }, [question]);

  const surfaceClass = className.includes("highlight-surface") ? "" : "surface";

  return (
    <section
      className={`${surfaceClass} px-5 py-5 transition-all duration-300 ${className}`.trim()}
    >
      <div className="flex items-center justify-between gap-4">
        <p className="text-xs font-semibold uppercase tracking-[0.18em] text-[var(--text-muted)]">
          Current Question
        </p>

        <p className="text-sm font-medium text-[var(--text-primary)]">
          {Math.min(currentIndex + 1, safeTotal)} / {safeTotal}
        </p>
      </div>

      <div className="mt-3 flex items-center gap-1.5">
        {Array.from({ length: safeTotal }).map((_, i) => {
          const isCurrent = i === Math.min(currentIndex, safeTotal - 1);
          const isDone = i < currentIndex;

          return (
            <span
              key={i}
              className={[
                "h-1.5 rounded-full transition-all duration-500 ease-out",
                isCurrent
                  ? "w-6 bg-[linear-gradient(135deg,var(--yellow-accent),var(--yellow-soft))]"
                  : "w-1.5",
                !isCurrent && isDone ? "bg-[var(--green-primary)]" : "",
                !isCurrent && !isDone ? "bg-[var(--border-soft)]" : "",
              ]
                .join(" ")
                .trim()}
            />
          );
        })}
      </div>

      <p className="font-display mt-3 text-base italic leading-7 text-[var(--text-primary)] sm:text-lg">
        {displayedText}

        <span className="ml-1 animate-pulse not-italic">|</span>
      </p>
    </section>
  );
}
