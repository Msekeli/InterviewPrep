import type { ReactNode } from "react";

type ParticipantPanelProps = {
  title: string;
  subtitle?: string;
  badge?: string;
  children?: ReactNode;
  className?: string;

  isActive?: boolean;
  isSpeaking?: boolean;
  isThinking?: boolean;
};

export default function ParticipantPanel({
  title,
  subtitle,
  badge,
  children,
  className = "",
  isActive = false,
  isSpeaking = false,
}: ParticipantPanelProps) {
  const stateClasses = [
    "transition-all duration-300 ease-in-out",
    "relative overflow-hidden",

    "opacity-80 scale-100",

    isActive ? "opacity-100 scale-[1.01]" : "",

    isSpeaking ? "opacity-100 scale-[1.03] shadow-[var(--glow-green)]" : "",
  ].join(" ");

  return (
    <section
      className={`surface px-5 py-5 ${stateClasses} ${className}`.trim()}
    >
      {/* 🔥 animated glow layer */}
      {isSpeaking && (
        <div className="pointer-events-none absolute inset-0 animate-pulse bg-gradient-to-r from-transparent via-white/5 to-transparent" />
      )}

      <div className="relative flex items-start justify-between gap-4">
        <div>
          <h3 className="text-base font-semibold text-[var(--text-primary)] sm:text-lg">
            {title}
          </h3>

          {subtitle ? (
            <p className="mt-1 text-sm text-[var(--text-muted)]">{subtitle}</p>
          ) : null}
        </div>

        {badge ? (
          <span
            className={`rounded-full border px-3 py-1 text-xs font-medium transition-all duration-300
              ${
                isSpeaking
                  ? "border-[var(--green-primary)] bg-[var(--green-tint-strong)] text-[var(--text-primary)]"
                  : "border-[var(--border-soft)] bg-[var(--surface)] text-[var(--text-muted)]"
              }
            `}
          >
            {badge}
          </span>
        ) : null}
      </div>

      {children ? <div className="relative mt-4">{children}</div> : null}
    </section>
  );
}
