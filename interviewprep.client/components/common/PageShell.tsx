import { PropsWithChildren } from "react";

type PageShellProps = PropsWithChildren<{
  className?: string;
  contentClassName?: string;
}>;

export default function PageShell({
  children,
  className = "",
  contentClassName = "",
}: PageShellProps) {
  return (
    <main
      className={`min-h-screen px-4 py-10 sm:px-6 lg:px-8 ${className}`.trim()}
    >
      <div className={`mx-auto w-full max-w-6xl ${contentClassName}`.trim()}>
        {children}
      </div>
    </main>
  );
}
