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
  /** Which accent this participant's "active" state renders in.
   * "dawn" = warm coral/amber (used for the AI, so it reads friendly rather
   * than clinical). "indigo" = calm primary (used for the candidate). */
  accent?: "indigo" | "dawn";
};

export default function ParticipantPanel({
  title,
  subtitle,
  badge,
  children,
  className = "",
  isActive = false,
  isSpeaking = false,
  accent = "indigo",
}: ParticipantPanelProps) {
  const stateClasses = [
    "transition-all duration-300 ease-in-out",
    "relative overflow-hidden",

    "opacity-80 scale-100",

    isActive ? "opacity-100 scale-[1.01]" : "",

    isSpeaking ? "opacity-100 scale-[1.03]" : "",
  ].join(" ");

  const surfaceClass = className.includes("highlight-surface") ? "" : "surface";

  const badgeActiveClass =
    accent === "dawn"
      ? "border-transparent bg-gradient-to-br from-[var(--yellow-accent)] to-[var(--yellow-soft)] text-[#4a2410]"
      : "border-transparent bg-gradient-to-br from-[var(--green-primary)] to-[var(--green-soft)] text-[#f3f3ff]";

  return (
    <section
      className={`${surfaceClass} px-3 py-3 sm:px-5 sm:py-5 ${stateClasses} ${className}`.trim()}
    >
      {isSpeaking && (
        <div className="pointer-events-none absolute inset-0 animate-pulse bg-gradient-to-r from-transparent via-white/5 to-transparent" />
      )}

      <div className="relative flex items-start justify-between gap-2 sm:gap-4">
        <div className="min-w-0">
          <h3 className="truncate text-sm font-semibold text-[var(--text-primary)] sm:text-lg">
            {title}
          </h3>

          {subtitle ? (
            <p className="mt-1 hidden text-sm text-[var(--text-muted)] sm:block">
              {subtitle}
            </p>
          ) : null}
        </div>

        {badge ? (
          <span
            className={`shrink-0 rounded-full border px-2 py-0.5 text-[10px] font-semibold transition-all duration-300 sm:px-3 sm:py-1 sm:text-xs ${
              isSpeaking
                ? badgeActiveClass
                : "border-[var(--border-soft)] surface text-[var(--text-muted)]"
            }`}
          >
            {badge}
          </span>
        ) : null}
      </div>

      {children ? <div className="relative mt-2 sm:mt-4">{children}</div> : null}
    </section>
  );
}
