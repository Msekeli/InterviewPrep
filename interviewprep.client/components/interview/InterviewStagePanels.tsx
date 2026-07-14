import ParticipantPanel from "./ParticipantPanel";
import type { InterviewStage } from "./interviewStage";

type InterviewStagePanelsProps = {
  stage: InterviewStage;
  aiSpeaking: boolean;
  userSpeaking: boolean;
  userReady: boolean;
  userTyping: boolean;
};

export default function InterviewStagePanels({
  stage,
  aiSpeaking,
  userSpeaking,
  userReady,
  userTyping,
}: InterviewStagePanelsProps) {
  return (
    <div className="grid grid-cols-2 gap-2 sm:gap-3">
      {/* 🔵 AI */}
      <ParticipantPanel
        title="AI Interviewer"
        subtitle={aiSpeaking ? "Speaking..." : "Waiting for your response."}
        badge={aiSpeaking ? "Speaking" : "Listening"}
        isSpeaking={aiSpeaking}
        accent="dawn"
        className={[
          "transition-all duration-300",

          !userTyping ? "highlight-surface" : "",

          aiSpeaking ? "shadow-[var(--glow-yellow)] scale-[1.02]" : "",
        ].join(" ")}
      >
        <div className="flex items-center justify-center">
          <div className="flex flex-col items-center text-center">
            <div
              className={[
                "flex h-9 w-9 items-center justify-center rounded-full border text-base transition-all duration-300 sm:mb-3 sm:h-16 sm:w-16 sm:text-2xl",

                aiSpeaking
                  ? "border-[var(--yellow-accent)] surface-strong"
                  : "border-[var(--border-soft)] surface",
              ].join(" ")}
            >
              🎙️
            </div>

            <p className="mt-1 line-clamp-1 max-w-[9rem] text-[11px] leading-tight text-[var(--text-muted)] sm:mt-2 sm:max-w-sm sm:text-sm sm:leading-6">
              {aiSpeaking
                ? "Asking the interview question..."
                : "Ready for your answer."}
            </p>
          </div>
        </div>
      </ParticipantPanel>

      {/* 🟡 USER */}
      <ParticipantPanel
        title="Candidate"
        subtitle={
          userSpeaking
            ? "Speaking..."
            : userReady
              ? "Review your answer before submitting."
              : "Waiting for your turn."
        }
        badge={
          userSpeaking
            ? "Speaking"
            : stage === "answering"
              ? "Your Turn"
              : "Waiting"
        }
        isSpeaking={userSpeaking}
        accent="indigo"
        className={[
          "transition-all duration-300",

          userTyping ? "highlight-surface" : "",

          userSpeaking ? "shadow-[var(--glow-green)] scale-[1.02]" : "",
        ].join(" ")}
      >
        <div className="flex items-center justify-center">
          <div className="flex flex-col items-center text-center">
            <div
              className={[
                "flex h-9 w-9 items-center justify-center rounded-full border text-base transition-all duration-300 sm:mb-3 sm:h-16 sm:w-16 sm:text-2xl",

                userSpeaking
                  ? "border-[var(--green-primary)] surface-strong"
                  : "border-[var(--border-soft)] surface",
              ].join(" ")}
            >
              👤
            </div>

            <p className="mt-1 line-clamp-1 max-w-[9rem] text-[11px] leading-tight text-[var(--text-muted)] sm:mt-2 sm:max-w-sm sm:text-sm sm:leading-6">
              {userSpeaking
                ? "Speak clearly and confidently."
                : userReady
                  ? "You can edit or submit your response."
                  : "Listen carefully before responding."}
            </p>
          </div>
        </div>
      </ParticipantPanel>
    </div>
  );
}
