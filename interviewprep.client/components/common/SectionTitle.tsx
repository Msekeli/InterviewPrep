import { PropsWithChildren } from "react";

type SectionTitleProps = PropsWithChildren<{
  title?: string;
  subtitle?: string;
  className?: string;
  titleClassName?: string;
  subtitleClassName?: string;
}>;

export default function SectionTitle({
  title,
  subtitle,
  className = "",
  titleClassName = "",
  subtitleClassName = "",
  children,
}: SectionTitleProps) {
  return (
    <div className={`space-y-2 ${className}`.trim()}>
      {title ? (
        <h2
          className={`text-2xl font-semibold tracking-tight text-gradient sm:text-3xl ${titleClassName}`.trim()}
        >
          {title}
        </h2>
      ) : null}

      {subtitle ? (
        <p
          className={`max-w-2xl text-sm leading-6 text-[var(--text-muted)] sm:text-base ${subtitleClassName}`.trim()}
        >
          {subtitle}
        </p>
      ) : null}

      {children}
    </div>
  );
}
