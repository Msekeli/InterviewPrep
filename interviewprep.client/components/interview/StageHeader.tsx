import StatusPill from "../common/StatusPill";

type StageHeaderProps = {
  title?: string;
  subtitle?: string;
  statusLabel?: string;
  statusVariant?: "default" | "success" | "warning" | "muted";
  className?: string;
};

export default function StageHeader({
  title = "Interview Session",
  subtitle = "Stay calm, answer clearly, and let your experience speak.",
  statusLabel,
  statusVariant = "default",
  className = "",
}: StageHeaderProps) {
  return (
    <header
      className={`flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between ${className}`.trim()}
    >
      <div className="space-y-2">
        <h1 className="text-3xl font-semibold tracking-tight text-gradient sm:text-4xl">
          {title}
        </h1>
        <p className="max-w-2xl text-sm leading-6 text-[var(--text-muted)] sm:text-base">
          {subtitle}
        </p>
      </div>

      {statusLabel ? (
        <div className="shrink-0">
          <StatusPill label={statusLabel} variant={statusVariant} />
        </div>
      ) : null}
    </header>
  );
}
