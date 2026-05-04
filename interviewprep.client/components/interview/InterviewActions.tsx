import PrimaryButton from "../common/PrimaryButton";
import SecondaryButton from "../common/SecondaryButton";

type InterviewActionsProps = {
  hasStarted: boolean;
  onStartInterview: () => void;
  onSubmit: () => void;
  onFinish?: () => void;
  isSubmitting?: boolean;
  isCompleting?: boolean;
  isLastQuestion?: boolean;
  disabled?: boolean;
  className?: string;
  onStartSpeaking: () => void;
  onStopSpeaking: () => void;
  isUserSpeaking?: boolean;
};

export default function InterviewActions({
  hasStarted,
  onStartInterview,
  onSubmit,
  onFinish,
  isSubmitting = false,
  isCompleting = false,
  isLastQuestion = false,
  disabled = false,
  onStartSpeaking,
  onStopSpeaking,
  isUserSpeaking = false,
}: InterviewActionsProps) {
  const isBusy = disabled || isSubmitting || isCompleting;

  return (
    <div className="flex flex-col gap-3 sm:flex-row sm:items-center">
      {/* 🎬 BEFORE START */}
      {!hasStarted && (
        <SecondaryButton
          type="button"
          onClick={onStartInterview}
          className="w-full sm:w-auto"
        >
          Start Interview
        </SecondaryButton>
      )}

      {/* 🎤 DURING INTERVIEW */}
      {hasStarted && !isLastQuestion && (
        <>
          {!isUserSpeaking ? (
            <button
              type="button"
              onClick={onStartSpeaking}
              disabled={isBusy}
              className="w-full rounded-lg border border-[var(--border-soft)] px-4 py-2 text-sm transition hover:bg-[rgba(255,255,255,0.05)] disabled:opacity-60 sm:w-auto"
            >
              🎙️ Speak
            </button>
          ) : (
            <button
              type="button"
              onClick={onStopSpeaking}
              className="w-full rounded-lg border border-red-400 px-4 py-2 text-sm text-red-400 transition hover:bg-[rgba(239,68,68,0.10)] sm:w-auto"
            >
              ⏹ Stop
            </button>
          )}

          <PrimaryButton
            type="button"
            onClick={onSubmit}
            disabled={isBusy}
            className="w-full sm:w-auto"
          >
            {isSubmitting ? "Submitting..." : "Next Question"}
          </PrimaryButton>
        </>
      )}

      {/* 🏁 AFTER LAST QUESTION */}
      {hasStarted && isLastQuestion && (
        <SecondaryButton
          type="button"
          onClick={onFinish}
          disabled={isBusy}
          className="w-full sm:w-auto"
        >
          {isCompleting ? "Finishing..." : "Finish Interview"}
        </SecondaryButton>
      )}
    </div>
  );
}
