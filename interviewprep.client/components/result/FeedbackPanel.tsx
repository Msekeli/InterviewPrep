type FeedbackPanelProps = {
  feedback: string;
  title?: string;
  className?: string;
};

export default function FeedbackPanel({
  feedback,
  title = "Feedback",
  className = "",
}: FeedbackPanelProps) {
  return (
    <section className={`surface px-6 py-6 ${className}`.trim()}>
      <h3 className="text-lg font-semibold text-[var(--text-primary)]">
        {title}
      </h3>
      <p className="mt-3 whitespace-pre-line text-sm leading-7 text-[var(--text-muted)] sm:text-base">
        {feedback}
      </p>
    </section>
  );
}
