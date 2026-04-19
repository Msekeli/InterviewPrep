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
      "border-[var(--border-soft)] bg-[rgba(34,197,94,0.1)] text-[var(--green-soft)]",
    success:
      "border-[rgba(34,197,94,0.35)] bg-[rgba(34,197,94,0.15)] text-[var(--green-soft)]",
    warning:
      "border-[rgba(234,179,8,0.35)] bg-[rgba(234,179,8,0.15)] text-[var(--yellow-soft)]",
    muted:
      "border-[rgba(255,255,255,0.1)] bg-[rgba(255,255,255,0.05)] text-[var(--text-muted)]",
  };

  return (
    <span
      className={`inline-flex items-center rounded-full border px-3 py-1 text-xs font-medium ${variantStyles[variant]} ${className}`.trim()}
    >
      {label}
    </span>
  );
}
