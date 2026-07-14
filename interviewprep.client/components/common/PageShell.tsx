import { PropsWithChildren } from "react";

type PageShellProps = PropsWithChildren<{
  className?: string;
}>;

export default function PageShell({
  children,
  className = "",
}: PageShellProps) {
  return (
    <main
      className={`min-h-[calc(100dvh-64px)] w-full px-4 py-4 sm:px-8 lg:px-[10%] ${className}`}
    >
      <div className="flex h-full w-full flex-col">{children}</div>
    </main>
  );
}
