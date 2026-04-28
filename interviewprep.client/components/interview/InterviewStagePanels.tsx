import ParticipantPanel from "./ParticipantPanel";
import type { InterviewStage } from "./interviewStage";

type InterviewStagePanelsProps = {
  stage: InterviewStage;
  aiSpeaking: boolean;
  userSpeaking: boolean;
  userReady: boolean;
};

export default function InterviewStagePanels({
  stage,
  aiSpeaking,
  userSpeaking,
  userReady,
}: InterviewStagePanelsProps) {
  return (
    <div className="grid gap-3 lg:grid-cols-2">
      {/* 🔵 AI INTERVIEWER */}
      <ParticipantPanel
        title="AI Interviewer"
        subtitle={aiSpeaking ? "Speaking..." : "Waiting for your response."}
        badge={aiSpeaking ? "Speaking" : "Listening"}
        className={[
          "transition-all duration-300",
          aiSpeaking
            ? "ring-2 ring-[rgba(34,197,94,0.65)] shadow-[var(--glow-green)] scale-[1.02]"
            : "",
        ].join(" ")}
      >
        <div className="flex min-h-[110px] items-center justify-center">
          <div className="flex flex-col items-center text-center">
            <div
              className={[
                "mb-3 flex h-16 w-16 items-center justify-center rounded-full border text-2xl transition-all duration-300",
                aiSpeaking
                  ? "border-green-400 bg-[rgba(34,197,94,0.15)] animate-pulse"
                  : "border-[var(--border-soft)] bg-[rgba(255,255,255,0.05)]",
              ].join(" ")}
            >
              🎙️
            </div>

            <p className="text-base font-semibold text-[var(--text-primary)]">
              AI Interviewer
            </p>

            <p className="mt-2 max-w-sm text-sm leading-6 text-[var(--text-muted)]">
              {aiSpeaking
                ? "Asking the interview question."
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
        className={[
          "transition-all duration-300",
          userSpeaking
            ? "ring-2 ring-[rgba(234,179,8,0.65)] shadow-[var(--glow-yellow)] scale-[1.02]"
            : userReady
              ? "ring-1 ring-[rgba(234,179,8,0.35)]"
              : "",
        ].join(" ")}
      >
        <div className="flex min-h-[110px] items-center justify-center">
          <div className="flex flex-col items-center text-center">
            <div
              className={[
                "mb-3 flex h-16 w-16 items-center justify-center rounded-full border text-2xl transition-all duration-300",
                userSpeaking
                  ? "border-yellow-400 bg-[rgba(234,179,8,0.15)] animate-pulse"
                  : "border-[rgba(234,179,8,0.25)] bg-[rgba(234,179,8,0.08)]",
              ].join(" ")}
            >
              👤
            </div>

            <p className="text-base font-semibold text-[var(--text-primary)]">
              You
            </p>

            <p className="mt-2 max-w-sm text-sm leading-6 text-[var(--text-muted)]">
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
