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
  const progress = Math.min(((currentIndex + 1) / safeTotal) * 100, 100);

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
        {question}
      </p>

      <div className="mt-5 h-2 overflow-hidden rounded-full bg-[rgba(255,255,255,0.06)]">
        <div
          className="h-full rounded-full bg-[linear-gradient(90deg,var(--green-soft),var(--yellow-accent))] transition-all duration-300"
          style={{ width: `${progress}%` }}
        />
      </div>
    </section>
  );
}
