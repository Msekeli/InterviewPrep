import { forwardRef, ChangeEvent } from "react";

type AnswerComposerProps = {
  value: string;
  onChange: (value: string) => void;
  placeholder?: string;
  disabled?: boolean;
  rows?: number;
  className?: string;
};

const AnswerComposer = forwardRef<HTMLTextAreaElement, AnswerComposerProps>(
  (
    {
      value,
      onChange,
      placeholder = "Write your answer here...",
      disabled = false,
      rows = 8,
      className = "",
    },
    ref,
  ) => {
    function handleChange(event: ChangeEvent<HTMLTextAreaElement>) {
      onChange(event.target.value);
    }

    return (
      <section className={`space-y-3 ${className}`.trim()}>
        <label
          htmlFor="answer-composer"
          className="block text-sm font-medium text-[var(--text-primary)]"
        >
          Your answer
        </label>

        <textarea
          ref={ref}
          id="answer-composer"
          value={value}
          onChange={handleChange}
          placeholder={placeholder}
          disabled={disabled}
          rows={rows}
          className="surface min-h-[180px] w-full resize-y px-4 py-3 text-sm leading-7 text-[var(--text-primary)] outline-none placeholder:text-[var(--text-muted)] disabled:cursor-not-allowed disabled:opacity-60"
        />
      </section>
    );
  },
);

AnswerComposer.displayName = "AnswerComposer";

export default AnswerComposer;
