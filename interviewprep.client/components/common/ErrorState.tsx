type ErrorStateProps = {
  title?: string;
  message: string;
  className?: string;
};

export default function ErrorState({
  title = "Something went wrong",
  message,
  className = "",
}: ErrorStateProps) {
  return (
    <div
      className={`surface flex min-h-[220px] flex-col items-center justify-center px-6 py-10 text-center ${className}`.trim()}
    >
      <div className="mb-4 rounded-full border border-[rgba(234,179,8,0.35)] bg-[rgba(234,179,8,0.12)] px-3 py-1 text-xs font-medium text-[var(--yellow-soft)]">
        Error
      </div>
      <h3 className="text-lg font-semibold text-[var(--text-primary)]">
        {title}
      </h3>
      <p className="mt-2 max-w-md text-sm text-[var(--text-muted)]">
        {message}
      </p>
    </div>
  );
}
