type StatusVariant = "default" | "success" | "warning" | "muted";

type StatusPillProps = {
  label: string;
  variant?: StatusVariant;
  className?: string;
};

export default function StatusPill({
  label,
  variant = "default",
  className = "",
}: StatusPillProps) {
  const variantStyles: Record<StatusVariant, string> = {
    default:
      "border-[var(--border-soft)] bg-[var(--surface)] text-[var(--text-primary)]",

    success:
      "border-[var(--border-soft)] bg-[rgba(34,197,94,0.12)] text-[var(--text-primary)]",

    warning:
      "border-[var(--border-soft)] bg-[rgba(234,179,8,0.15)] text-[var(--text-primary)]",

    muted:
      "border-[var(--border-soft)] bg-[var(--surface)] text-[var(--text-muted)]",
  };

  return (
    <span
      className={`inline-flex items-center rounded-full border px-3 py-1 text-xs font-medium ${variantStyles[variant]} ${className}`.trim()}
    >
      {label}
    </span>
  );
}
