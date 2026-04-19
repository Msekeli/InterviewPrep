type ScoreDisplayProps = {
  score: number;
  label?: string;
  className?: string;
};

export default function ScoreDisplay({
  score,
  label = "Overall Score",
  className = "",
}: ScoreDisplayProps) {
  const roundedScore = Math.round(score);

  return (
    <div className={`surface px-6 py-8 text-center ${className}`.trim()}>
      <p className="text-sm font-medium uppercase tracking-[0.2em] text-[var(--text-muted)]">
        {label}
      </p>

      <div className="mt-4">
        <span className="text-5xl font-bold tracking-tight text-gradient sm:text-6xl">
          {roundedScore}
        </span>
        <span className="ml-1 text-2xl font-semibold text-[var(--text-muted)]">
          /100
        </span>
      </div>
    </div>
  );
}
