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
      className={`min-h-[calc(100dvh-64px)] w-full px-[10%] py-4 ${className}`}
    >
      <div className="flex h-full w-full flex-col">{children}</div>
    </main>
  );
}
