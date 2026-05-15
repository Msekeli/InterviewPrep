type CoachHighlightProps = {
  title?: string;
  message: string;
  className?: string;
};

export default function CoachHighlight({
  title = "Insight",
  message,
  className = "",
}: CoachHighlightProps) {
  return (
    <section className={`highlight-surface px-6 py-5 ${className}`.trim()}>
      <p className="text-sm font-semibold uppercase tracking-[0.18em] text-[var(--yellow-accent)]">
        {title}
      </p>

      <p className="mt-3 text-base leading-8 text-[var(--text-primary)] sm:text-lg">
        {message}
      </p>
    </section>
  );
}
