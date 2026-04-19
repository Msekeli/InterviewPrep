type NextStepNoteProps = {
  title?: string;
  message?: string;
  className?: string;
};

export default function NextStepNote({
  title = "Next step",
  message = "Review your feedback, sharpen your examples, and come back for another round stronger than before.",
  className = "",
}: NextStepNoteProps) {
  return (
    <section
      className={`rounded-2xl border border-[rgba(234,179,8,0.25)] bg-[rgba(234,179,8,0.08)] px-5 py-4 ${className}`.trim()}
    >
      <p className="text-sm font-semibold uppercase tracking-[0.18em] text-[var(--yellow-soft)]">
        {title}
      </p>
      <p className="mt-2 text-sm leading-6 text-[var(--text-primary)] sm:text-base">
        {message}
      </p>
    </section>
  );
}
