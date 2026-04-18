type ContextTextAreaProps = {
  id: string;
  label: string;
  placeholder: string;
  value: string;
  onChange: (value: string) => void;
  rows?: number;
};

export function ContextTextArea({
  id,
  label,
  placeholder,
  value,
  onChange,
  rows = 6,
}: ContextTextAreaProps) {
  return (
    <div className="space-y-2">
      <label
        htmlFor={id}
        className="text-sm font-medium text-[var(--text-primary)]"
      >
        {label}
      </label>

      <textarea
        id={id}
        rows={rows}
        value={value}
        onChange={(e) => onChange(e.target.value)}
        placeholder={placeholder}
        className="w-full rounded-xl border border-[var(--border-soft)] bg-transparent px-4 py-3 text-sm text-[var(--text-primary)] outline-none transition placeholder:text-white/30 focus:border-[var(--green-soft)]"
      />
    </div>
  );
}
