type ContextTextAreaProps = {
  id: string;
  label: string;
  placeholder: string;
  value: string;
  onChange: (value: string) => void;
  rows?: number;
  maxLength?: number;
  optional?: boolean;
  hint?: string;
  /** Stretch to fill the available height instead of a fixed row count —
   * used by the setup wizard so each step fits the viewport exactly. */
  fill?: boolean;
};

export function ContextTextArea({
  id,
  label,
  placeholder,
  value,
  onChange,
  rows = 6,
  maxLength,
  optional = false,
  hint,
  fill = false,
}: ContextTextAreaProps) {
  return (
    <div className={fill ? "flex h-full flex-col gap-2" : "space-y-2"}>
      <div className="flex shrink-0 items-baseline justify-between gap-3">
        <label
          htmlFor={id}
          className="text-sm font-medium text-[var(--text-primary)]"
        >
          {label}
          {optional ? (
            <span className="ml-1.5 text-xs font-normal text-[var(--text-muted)]">
              (optional)
            </span>
          ) : null}
        </label>

        {maxLength ? (
          <span className="shrink-0 text-xs tabular-nums text-[var(--text-muted)]">
            {value.length.toLocaleString()} / {maxLength.toLocaleString()}
          </span>
        ) : null}
      </div>

      <textarea
        id={id}
        rows={fill ? undefined : rows}
        value={value}
        maxLength={maxLength}
        onChange={(e) => onChange(e.target.value)}
        placeholder={placeholder}
        className={[
          "w-full rounded-2xl border border-dashed border-[var(--border-strong)] bg-[var(--bg)] px-4 py-3 text-sm leading-6 text-[var(--text-primary)] outline-none transition placeholder:text-[var(--text-muted)] focus:border-solid focus:border-[var(--yellow-accent)]",
          fill ? "min-h-0 flex-1 resize-none" : "resize-y",
        ].join(" ")}
      />

      {hint ? (
        <p className="shrink-0 text-xs text-[var(--text-muted)]">{hint}</p>
      ) : null}
    </div>
  );
}
