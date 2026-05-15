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
            ? "highlight-surface shadow-[var(--glow-green)] scale-[1.02]"
            : "",
        ].join(" ")}
      >
        <div className="flex min-h-[110px]  items-center justify-center">
          <div className="flex flex-col items-center text-center">
            {/* 🎙 AI MIC VISUAL */}
            <div
              className={[
                "mb-3 flex h-16 w-16 items-center justify-center rounded-full border text-2xl transition-all duration-300",
                aiSpeaking
                  ? "border-[var(--green-primary)] highlight-surface  animate-pulse scale-110"
                  : "border-[var(--border-soft)] surface scale-100",
              ].join(" ")}
              style={{
                transform: aiSpeaking ? "scale(1.15)" : "scale(1)",
              }}
            >
              🎙️
            </div>

            {/* 🔊 AI SOUND WAVES */}
            {aiSpeaking && (
              <div className="mb-2 flex items-end gap-[3px]">
                <span className="h-2 w-1 animate-pulse bg-[var(--green-primary)]" />
                <span className="delay-75 h-3 w-1 animate-pulse bg-[var(--green-primary)]" />
                <span className="delay-150 h-4 w-1 animate-pulse bg-[var(--green-primary)]" />
                <span className="delay-200 h-3 w-1 animate-pulse bg-[var(--green-primary)]" />
                <span className="delay-300 h-2 w-1 animate-pulse bg-[var(--green-primary)]" />
              </div>
            )}

            <p className="text-base font-semibold text-[var(--text-primary)]">
              AI Interviewer
            </p>

            <p className="mt-2 max-w-sm text-sm leading-6 text-[var(--text-muted)]">
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
        className={[
          "transition-all duration-300",
          userSpeaking
            ? "highlight-surface shadow-[var(--glow-yellow)] scale-[1.02]"
            : userReady
              ? "highlight-surface"
              : "",
        ].join(" ")}
      >
        <div className="flex min-h-[110px] items-center justify-center">
          <div className="flex flex-col items-center text-center">
            {/* 🎤 USER MIC VISUAL */}
            <div
              className={[
                "mb-3 flex h-16 w-16 items-center justify-center rounded-full border text-2xl transition-all duration-300",
                userSpeaking
                  ? "border-[var(--yellow-accent)]  scale-110"
                  : "border-[var(--border-soft)] surface scale-100",
              ].join(" ")}
              style={{
                transform: userSpeaking ? "scale(1.15)" : "scale(1)",
              }}
            >
              👤
            </div>

            {/* 🔊 USER SOUND WAVES */}
            {userSpeaking && (
              <div className="mb-2 flex items-end gap-[3px]">
                <span className="h-2 w-1 animate-pulse bg-[var(--yellow-accent)]" />
                <span className="delay-75 h-3 w-1 animate-pulse bg-[var(--yellow-accent)]" />
                <span className="delay-150 h-4 w-1 animate-pulse bg-[var(--yellow-accent)]" />
                <span className="delay-200 h-3 w-1 animate-pulse bg-[var(--yellow-accent)]" />
                <span className="delay-300 h-2 w-1 animate-pulse bg-[var(--yellow-accent)]" />
              </div>
            )}

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
