import ParticipantPanel from "./ParticipantPanel";
import type { InterviewStage } from "./interviewStage";

type InterviewStagePanelsProps = {
  stage: InterviewStage;
};

export default function InterviewStagePanels({
  stage,
}: InterviewStagePanelsProps) {
  return (
    <div className="grid gap-3 lg:grid-cols-2">
      <ParticipantPanel
        title="AI Interviewer"
        subtitle={
          stage === "asking"
            ? "The interviewer is asking the current question."
            : "Waiting for your response."
        }
        badge={stage === "asking" ? "Asking" : "Listening"}
        className={[
          "transition-all duration-300",
          stage === "asking"
            ? "ring-1 ring-[rgba(34,197,94,0.45)] shadow-[var(--glow-green)]"
            : "",
        ].join(" ")}
      >
        <div className="flex min-h-[110px] items-center justify-center">
          <div className="flex flex-col items-center text-center">
            <div className="mb-3 flex h-16 w-16 items-center justify-center rounded-full border border-[var(--border-soft)] bg-[rgba(255,255,255,0.05)] text-2xl">
              🎙️
            </div>
            <p className="text-base font-semibold text-[var(--text-primary)]">
              AI Interviewer
            </p>
            <p className="mt-2 max-w-sm text-sm leading-6 text-[var(--text-muted)]">
              {stage === "asking"
                ? "Presenting the next interview question."
                : "Ready for your answer."}
            </p>
          </div>
        </div>
      </ParticipantPanel>

      <ParticipantPanel
        title="Candidate"
        subtitle={
          stage === "answering"
            ? "This is your turn to answer."
            : "Get ready to respond."
        }
        badge={stage === "answering" ? "Responding" : "Waiting"}
        className={[
          "transition-all duration-300",
          stage === "answering"
            ? "ring-1 ring-[rgba(234,179,8,0.45)] shadow-[var(--glow-yellow)]"
            : "",
        ].join(" ")}
      >
        <div className="flex min-h-[110px] items-center justify-center">
          <div className="flex flex-col items-center text-center">
            <div className="mb-3 flex h-16 w-16 items-center justify-center rounded-full border border-[rgba(234,179,8,0.25)] bg-[rgba(234,179,8,0.08)] text-2xl">
              👤
            </div>
            <p className="text-base font-semibold text-[var(--text-primary)]">
              You
            </p>
            <p className="mt-2 max-w-sm text-sm leading-6 text-[var(--text-muted)]">
              {stage === "answering"
                ? "Answer with specific examples and real experience."
                : "Listen carefully to the question before responding."}
            </p>
          </div>
        </div>
      </ParticipantPanel>
    </div>
  );
}
