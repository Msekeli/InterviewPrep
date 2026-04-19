import PrimaryButton from "../common/PrimaryButton";
import SecondaryButton from "../common/SecondaryButton";

type InterviewActionsProps = {
  onSubmit: () => void;
  onFinish?: () => void;
  isSubmitting?: boolean;
  isCompleting?: boolean;
  isLastQuestion?: boolean;
  disabled?: boolean;
  className?: string;
};

export default function InterviewActions({
  onSubmit,
  onFinish,
  isSubmitting = false,
  isCompleting = false,
  isLastQuestion = false,
  disabled = false,
  className = "",
}: InterviewActionsProps) {
  const isBusy = disabled || isSubmitting || isCompleting;

  return (
    <div
      className={`flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between ${className}`.trim()}
    >
      <p className="text-xs leading-5 text-[var(--text-muted)] sm:text-sm">
        Keep your answer grounded in real experience.
      </p>

      <div className="flex flex-col gap-3 sm:flex-row">
        {isLastQuestion && onFinish ? (
          <SecondaryButton
            type="button"
            onClick={onFinish}
            disabled={isBusy}
            className="w-full disabled:cursor-not-allowed disabled:opacity-60 sm:w-auto"
          >
            {isCompleting ? "Finishing..." : "Finish interview"}
          </SecondaryButton>
        ) : null}

        <PrimaryButton
          type="button"
          onClick={onSubmit}
          disabled={isBusy}
          className="w-full disabled:cursor-not-allowed disabled:opacity-60 sm:w-auto"
        >
          {isCompleting
            ? "Finishing..."
            : isSubmitting
              ? "Submitting..."
              : isLastQuestion
                ? "Submit answer"
                : "Next question"}
        </PrimaryButton>
      </div>
    </div>
  );
}
