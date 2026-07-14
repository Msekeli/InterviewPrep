"use client";

import { useRouter } from "next/navigation";

import AppHeader from "../common/AppHeader";
import PageShell from "../common/PageShell";
import LoadingState from "../common/LoadingState";
import ErrorState from "../common/ErrorState";

import InterviewActions from "./InterviewActions";
import InterviewStagePanels from "./InterviewStagePanels";
import QuestionBar from "./QuestionBar";
import AnswerComposer from "./AnswerComposer";
import StatusPill from "../common/StatusPill";

import { getStageStatus, type InterviewStage } from "./interviewStage";

import { useInterviewController } from "./useInterviewController";

type InterviewRoomProps = {
  sessionId: string;
};

export function InterviewRoom({ sessionId }: InterviewRoomProps) {
  const router = useRouter();

  const {
    session,
    questions,
    loading,
    error,

    currentQuestion,
    index,

    hasStarted,
    startInterview,
    finishInterview,

    answerText,
    setAnswerText,

    submitAnswer,

    handleStartSpeaking,
    handleStopSpeaking,

    isSubmitting,
    isCompleting,

    aiSpeaking,
    userSpeaking,
    userReady,
  } = useInterviewController(sessionId);

  // 🧠 derived state
  const hasCompletedQuestions = index >= questions.length;

  const isLastQuestion = hasCompletedQuestions;

  const userTyping = answerText.trim().length > 0;

  const stage: InterviewStage = isSubmitting
    ? "submitting"
    : isCompleting
      ? "completing"
      : hasStarted
        ? "answering"
        : "asking";

  const stageStatus = getStageStatus(stage);

  // 🚨 loading state
  if (loading) {
    return (
      <>
        <AppHeader title="Interview Session" stage="interview" />

        <PageShell>
          <LoadingState
            title="Loading..."
            message="Preparing your interview..."
          />
        </PageShell>
      </>
    );
  }

  // 🚨 error state
  if (!session || error) {
    return (
      <>
        <AppHeader title="Interview Session" stage="interview" />

        <PageShell>
          <ErrorState message="Unable to load interview session." />
        </PageShell>
      </>
    );
  }

  return (
    <>
      <AppHeader title="Interview Session" stage="interview" />

      <PageShell>
        <div className="flex h-full flex-col gap-4">
          {/* 🟡 STATUS ERROR */}
          {error && (
            <div className="rounded-xl border-l-4 border-[var(--yellow-accent)] bg-[var(--surface)] px-4 py-3 text-sm text-[var(--yellow-soft)]">
              {error}
            </div>
          )}

          {/* 🎭 AI / USER */}
          <InterviewStagePanels
            stage={stage}
            aiSpeaking={aiSpeaking}
            userSpeaking={userSpeaking}
            userReady={userReady}
            userTyping={userTyping}
          />

          {/* 📌 QUESTION */}
          <QuestionBar
            className={!userTyping ? "highlight-surface" : ""}
            question={
              !hasStarted
                ? "Click Start Interview to begin"
                : (currentQuestion?.text ??
                  "All questions answered. Finish interview.")
            }
            currentIndex={Math.min(index, Math.max(questions.length - 1, 0))}
            totalQuestions={questions.length}
          />

          {/* 🧱 MAIN BOX */}
          <div className="surface flex flex-1 flex-col p-5">
            <div className="mb-4 flex justify-between">
              <StatusPill
                label={stageStatus.label}
                variant={stageStatus.variant}
              />
            </div>

            {/* ✍️ ANSWER */}
            <AnswerComposer
              value={answerText}
              onChange={setAnswerText}
              disabled={!hasStarted || aiSpeaking || hasCompletedQuestions}
              rows={5}
              className={userTyping ? "highlight-surface" : ""}
              placeholder={
                !hasStarted
                  ? "Click Start Interview to begin typing..."
                  : hasCompletedQuestions
                    ? "Interview complete. Click Finish Interview."
                    : "Type your answer..."
              }
            />

            {/* 🎮 ACTIONS */}
            <InterviewActions
              hasStarted={hasStarted}
              onStartInterview={startInterview}
              onSubmit={submitAnswer}
              onFinish={async () => {
                await finishInterview();

                router.push(`/session/${sessionId}/result`);
              }}
              onStartSpeaking={handleStartSpeaking}
              onStopSpeaking={handleStopSpeaking}
              isUserSpeaking={userSpeaking}
              isSubmitting={isSubmitting}
              isCompleting={isCompleting}
              isLastQuestion={isLastQuestion}
              disabled={
                hasStarted &&
                (aiSpeaking ||
                  (stage !== "answering" && !hasCompletedQuestions))
              }
            />
          </div>
        </div>
      </PageShell>
    </>
  );
}
