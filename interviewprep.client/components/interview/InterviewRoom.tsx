"use client";

import { useRef } from "react";

import AppHeader from "../common/AppHeader";
import PageShell from "../common/PageShell";
import LoadingState from "../common/LoadingState";
import ErrorState from "../common/ErrorState";
import StatusPill from "../common/StatusPill";

import AnswerComposer from "./AnswerComposer";
import InterviewActions from "./InterviewActions";
import InterviewStagePanels from "./InterviewStagePanels";
import QuestionBar from "./QuestionBar";

import { useInterviewController } from "./useInterviewController";
import { getStageStatus } from "./interviewStage";

type Props = {
  sessionId: string;
};

export function InterviewRoom({ sessionId }: Props) {
  const answerRef = useRef<HTMLTextAreaElement | null>(null);

  const {
    session,
    questions,
    loading,
    turn,
    userReady,
    aiSpeaking,
    userSpeaking,

    answerText,
    setAnswerText,

    currentQuestion,
    index,
    isLast,

    handleStartSpeaking,
    handleStopSpeaking,
    submitAnswer,
  } = useInterviewController(sessionId);

  if (loading) {
    return (
      <>
        <AppHeader title="Interview Session" />
        <PageShell>
          <LoadingState title="Loading..." message="Preparing interview..." />
        </PageShell>
      </>
    );
  }

  if (!session || !questions.length) {
    return (
      <>
        <AppHeader title="Interview Session" />
        <PageShell>
          <ErrorState message="Unable to load interview." />
        </PageShell>
      </>
    );
  }

  const stage = turn === "idle" ? "asking" : "answering";
  const stageStatus = getStageStatus(stage);

  return (
    <>
      <AppHeader title="Interview Session" />

      <PageShell>
        <div className="flex flex-col gap-4 h-full">
          <InterviewStagePanels
            stage={turn === "idle" ? "asking" : "answering"}
            aiSpeaking={aiSpeaking}
            userSpeaking={userSpeaking}
            userReady={userReady}
          />

          <QuestionBar
            question={currentQuestion?.text ?? ""}
            currentIndex={index}
            totalQuestions={questions.length}
          />

          <div className="surface flex flex-col flex-1 p-5">
            <div className="mb-4 flex justify-between">
              <StatusPill
                label={stageStatus.label}
                variant={stageStatus.variant}
              />
            </div>

            <AnswerComposer
              ref={answerRef}
              value={answerText}
              onChange={setAnswerText}
              rows={5}
            />

            <InterviewActions
              className="mt-4"
              hasStarted={true}
              onStartInterview={() => {}}
              onSubmit={submitAnswer}
              onFinish={() => {}}
              onStartSpeaking={handleStartSpeaking}
              onStopSpeaking={handleStopSpeaking}
              isUserSpeaking={userSpeaking}
              isSubmitting={false}
              isCompleting={false}
              isLastQuestion={isLast}
              disabled={false}
            />
          </div>
        </div>
      </PageShell>
    </>
  );
}
