type LevelOption = {
  value: number;
  label: string;
  description?: string;
};

type LevelSelectorProps = {
  value?: number;
  onChange: (value: number) => void;
  options?: LevelOption[];
  className?: string;
};

const defaultOptions: LevelOption[] = [
  {
    value: 1,
    label: "Junior",
    description: "For early-career roles and foundational interview practice.",
  },
  {
    value: 2,
    label: "Intermediate",
    description:
      "For mid-level roles with deeper technical and project questions.",
  },
  {
    value: 3,
    label: "Senior",
    description: "For advanced roles with architecture, leadership, and depth.",
  },
];

export default function LevelSelector({
  value,
  onChange,
  options = defaultOptions,
  className = "",
}: LevelSelectorProps) {
  return (
    <div className={`space-y-3 ${className}`.trim()}>
      <label className="block text-sm font-medium text-[var(--text-primary)]">
        Interview level
      </label>

      <div className="grid gap-3 sm:grid-cols-3">
        {options.map((option) => {
          const isActive = value === option.value;

          return (
            <button
              key={option.value}
              type="button"
              onClick={() => onChange(option.value)}
              className={[
                "surface text-left transition-all duration-200 px-4 py-4",
                isActive
                  ? "border-[rgba(34,197,94,0.45)] shadow-[var(--glow-green)]"
                  : "hover:border-[rgba(34,197,94,0.35)]",
              ].join(" ")}
            >
              <div className="flex items-center justify-between gap-3">
                <span className="text-sm font-semibold text-[var(--text-primary)]">
                  {option.label}
                </span>

                {isActive ? (
                  <span className="rounded-full bg-[rgba(34,197,94,0.18)] px-2 py-1 text-[10px] font-semibold uppercase tracking-[0.16em] text-[var(--green-soft)]">
                    Selected
                  </span>
                ) : null}
              </div>

              {option.description ? (
                <p className="mt-2 text-sm leading-6 text-[var(--text-muted)]">
                  {option.description}
                </p>
              ) : null}
            </button>
          );
        })}
      </div>
    </div>
  );
}
