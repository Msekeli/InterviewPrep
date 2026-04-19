import type { ReactNode } from "react";

type ResultSummaryProps = {
  score: ReactNode;
  feedback: ReactNode;
  nextStep?: ReactNode;
  className?: string;
};

export default function ResultSummary({
  score,
  feedback,
  nextStep,
  className = "",
}: ResultSummaryProps) {
  return (
    <div className={`grid gap-6 ${className}`.trim()}>
      {score}
      {feedback}
      {nextStep ? nextStep : null}
    </div>
  );
}
