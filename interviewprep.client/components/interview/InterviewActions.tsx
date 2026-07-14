import AccentButton from "../common/AccentButton";
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
      {/* 🟡 BEFORE START */}
      {!hasStarted && (
        <AccentButton
          onClick={onStartInterview}
          disabled={isBusy}
          className="w-full sm:w-auto"
        >
          Start Interview
        </AccentButton>
      )}

      {/* 🟢 DURING INTERVIEW (ALL QUESTIONS EXCEPT LAST) */}
      {hasStarted && !isLastQuestion && (
        <>
          <SecondaryButton
            onClick={isUserSpeaking ? onStopSpeaking : onStartSpeaking}
            disabled={isBusy}
            className="w-full sm:w-auto"
          >
            {isUserSpeaking ? "⏹ Stop Speaking" : "🎙 Speak"}
          </SecondaryButton>

          <AccentButton
            onClick={onSubmit}
            disabled={isBusy}
            className="w-full sm:w-auto"
          >
            {isSubmitting ? "Submitting..." : "Next Question"}
          </AccentButton>
        </>
      )}

      {/* 🔴 LAST QUESTION (FINISH ONLY) */}
      {hasStarted && isLastQuestion && (
        <SecondaryButton
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
