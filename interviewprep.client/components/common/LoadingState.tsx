type LoadingStateProps = {
  title?: string;
  message?: string;
  className?: string;
};

export default function LoadingState({
  title = "Loading...",
  message = "Please wait while we prepare your experience.",
  className = "",
}: LoadingStateProps) {
  return (
    <div
      className={`surface flex min-h-[220px] flex-col items-center justify-center px-6 py-10 text-center ${className}`.trim()}
    >
      <div className="mb-4 h-10 w-10 animate-spin rounded-full border-4 border-[var(--border-soft)] border-t-[var(--green-primary)]" />
      <h3 className="text-lg font-semibold text-[var(--text-primary)]">
        {title}
      </h3>
      <p className="mt-2 max-w-md text-sm text-[var(--text-muted)]">
        {message}
      </p>
    </div>
  );
}
