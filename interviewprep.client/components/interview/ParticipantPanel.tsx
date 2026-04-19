import type { ReactNode } from "react";

type ParticipantPanelProps = {
  title: string;
  subtitle?: string;
  badge?: string;
  children?: ReactNode;
  className?: string;
};

export default function ParticipantPanel({
  title,
  subtitle,
  badge,
  children,
  className = "",
}: ParticipantPanelProps) {
  return (
    <section className={`surface px-5 py-5 ${className}`.trim()}>
      <div className="flex items-start justify-between gap-4">
        <div>
          <h3 className="text-base font-semibold text-[var(--text-primary)] sm:text-lg">
            {title}
          </h3>
          {subtitle ? (
            <p className="mt-1 text-sm text-[var(--text-muted)]">{subtitle}</p>
          ) : null}
        </div>

        {badge ? (
          <span className="rounded-full border border-[var(--border-soft)] bg-[rgba(34,197,94,0.1)] px-3 py-1 text-xs font-medium text-[var(--green-soft)]">
            {badge}
          </span>
        ) : null}
      </div>

      {children ? <div className="mt-4">{children}</div> : null}
    </section>
  );
}
