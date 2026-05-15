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
    <section className={`surface px-6 py-6 ${className}`.trim()}>
      <div className="flex items-center gap-3">
        {icon ? (
          <span className="text-[var(--green-primary)]">{icon}</span>
        ) : null}

        <h3 className="text-base font-semibold tracking-tight text-[var(--text-primary)] sm:text-lg">
          {title}
        </h3>
      </div>

      <p className="mt-4 whitespace-pre-line text-sm leading-7 text-[var(--text-primary)] sm:text-base">
        {content}
      </p>
    </section>
  );
}
