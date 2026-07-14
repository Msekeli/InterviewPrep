import type { ReactNode } from "react";

type CoachingSectionProps = {
  title?: string;
  content: string;
  icon?: ReactNode;
  className?: string;
};

export default function CoachingSection({
  title = "Insight",
  content,
  icon,
  className = "",
}: CoachingSectionProps) {
  return (
    <section className={`surface px-5 py-5 ${className}`.trim()}>
      <div className="flex items-center gap-2.5">
        {icon ? (
          <span className="text-[var(--green-primary)]">{icon}</span>
        ) : null}

        <h3 className="text-sm font-semibold tracking-tight text-[var(--green-primary)] sm:text-base">
          {title}
        </h3>
      </div>

      <p className="mt-3 whitespace-pre-line text-sm leading-7 text-[var(--text-primary)] sm:text-base">
        {content}
      </p>
    </section>
  );
}
