type CoachHighlightProps = {
  title?: string;
  message: string;
  className?: string;
  /** "open" = soft warm-tinted card (the opening Observation).
   *  "close" = solid warm gradient card (the closing Next Focus). */
  variant?: "open" | "close";
};

export default function CoachHighlight({
  title = "Insight",
  message,
  className = "",
  variant = "open",
}: CoachHighlightProps) {
  if (variant === "close") {
    return (
      <section
        className={`rounded-2xl bg-[linear-gradient(135deg,var(--yellow-accent),var(--yellow-soft))] px-6 py-7 sm:px-8 sm:py-8 ${className}`.trim()}
      >
        <p className="text-xs font-semibold uppercase tracking-[0.18em] text-[#4a2410] opacity-70">
          {title}
        </p>

        <p className="font-display mt-3 text-lg font-medium leading-8 text-[#4a2410] sm:text-xl">
          {message}
        </p>
      </section>
    );
  }

  return (
    <section className={`highlight-surface px-6 py-7 sm:px-8 sm:py-8 ${className}`.trim()}>
      <p className="text-xs font-semibold uppercase tracking-[0.18em] text-[var(--yellow-accent)]">
        {title}
      </p>

      <p className="font-display mt-3 text-lg italic leading-8 text-[var(--text-primary)] sm:text-xl">
        {message}
      </p>
    </section>
  );
}
