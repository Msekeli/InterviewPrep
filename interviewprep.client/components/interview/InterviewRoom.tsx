"use client";

import { useEffect, useRef, useState } from "react";
import { useRouter } from "next/navigation";
import {
  completeSession,
  generateQuestions,
  getQuestions,
  getSessionById,
  submitAnswer,
} from "@/services/sessionApi";
import { speakText } from "@/services/speechService";
import { startListening, stopListening } from "@/services/speechToTextService";
import type { InterviewSessionDto, QuestionDto } from "@/types/session";
import AppHeader from "../common/AppHeader";
import ErrorState from "../common/ErrorState";
import LoadingState from "../common/LoadingState";
import PageShell from "../common/PageShell";
import StatusPill from "../common/StatusPill";
import AnswerComposer from "./AnswerComposer";
import InterviewActions from "./InterviewActions";
import InterviewStagePanels from "./InterviewStagePanels";
import QuestionBar from "./QuestionBar";
import { getStageStatus, type InterviewStage } from "./interviewStage";

type InterviewRoomProps = {
  sessionId: string;
};

export function InterviewRoom({ sessionId }: InterviewRoomProps) {
  const router = useRouter();
  const hasLoaded = useRef(false);

  const [session, setSession] = useState<InterviewSessionDto | null>(null);
  const [questions, setQuestions] = useState<QuestionDto[]>([]);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [answerText, setAnswerText] = useState("");
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [completing, setCompleting] = useState(false);
  const [error, setError] = useState("");
  const [stage, setStage] = useState<InterviewStage>("asking");

  // 🔊 Speech states
  const [aiSpeaking, setAiSpeaking] = useState(false);
  const [userSpeaking, setUserSpeaking] = useState(false);
  const [userReady, setUserReady] = useState(false);
  const answerRef = useRef<HTMLTextAreaElement | null>(null);

  useEffect(() => {
    if (hasLoaded.current) return;
    hasLoaded.current = true;

    async function loadRoom() {
      try {
        setLoading(true);
        setError("");

        const sessionData = await getSessionById(sessionId);
        setSession(sessionData);

        let questionData = await getQuestions(sessionId);

        if (!questionData.length) {
          questionData = await generateQuestions(sessionId);
        }

        setQuestions(questionData);
      } catch (err) {
        console.error("Interview room load failed:", err);
        setError("Failed to load the interview room.");
      } finally {
        setLoading(false);
      }
    }

    loadRoom();
  }, [sessionId]);

  const currentQuestion = questions[currentIndex];
  const isLastQuestion = currentIndex === questions.length - 1;
  const stageStatus = getStageStatus(stage);

  // 🔊 AI SPEECH FLOW
  useEffect(() => {
    if (!currentQuestion || loading || submitting || completing) return;

    async function runSpeechFlow() {
      try {
        setAiSpeaking(true);
        setStage("asking");

        await speakText(currentQuestion.text);

        setAiSpeaking(false);
        setStage("answering");

        // 🎯 NEW: immediate readiness signal
        setUserReady(true);

        // 🎯 AUTO FOCUS INPUT
        setTimeout(() => {
          answerRef.current?.focus();
        }, 50);
      } catch (err) {
        console.error(err);

        setAiSpeaking(false);
        setStage("answering");
      }
    }

    runSpeechFlow();
  }, [currentQuestion, loading, submitting, completing]);

  async function handleCompleteSession() {
    try {
      setCompleting(true);
      setStage("completing");
      setError("");

      await completeSession(sessionId);
      router.push(`/session/${sessionId}/result`);
    } catch (err) {
      console.error(err);
      setError(
        err instanceof Error
          ? err.message
          : "Failed to complete the interview.",
      );
      setStage("answering");
    } finally {
      setCompleting(false);
    }
  }

  async function handleSubmitAnswer() {
    if (!currentQuestion) return;

    if (!answerText.trim()) {
      setError("Please enter an answer before continuing.");
      return;
    }

    try {
      setSubmitting(true);
      setStage("submitting");
      setError("");

      await submitAnswer(sessionId, {
        interviewQuestionId: currentQuestion.id,
        transcript: answerText.trim(),
      });

      if (isLastQuestion) {
        await handleCompleteSession();
        return;
      }

      setAnswerText("");
      setCurrentIndex((prev) => prev + 1);
    } catch (err) {
      console.error(err);
      setError(
        err instanceof Error ? err.message : "Failed to submit your answer.",
      );
      setStage("answering");
    } finally {
      setSubmitting(false);
    }
  }

  // 🎤 USER SPEECH
  function handleStartSpeaking() {
    setUserSpeaking(true);
    setUserReady(false);

    startListening(
      (partial) => setAnswerText(partial),
      (finalText) => {
        setAnswerText(finalText);
        setUserReady(true);
      },
    );
  }

  function handleStopSpeaking() {
    stopListening();
    setUserSpeaking(false);
  }

  if (loading) {
    return (
      <>
        <AppHeader title="Interview Session" />
        <PageShell>
          <LoadingState
            title="Loading interview room..."
            message="Preparing your session and questions."
          />
        </PageShell>
      </>
    );
  }

  if (error && !session) {
    return (
      <>
        <AppHeader title="Interview Session"  />
        <PageShell>
          <ErrorState message={error} />
        </PageShell>
      </>
    );
  }

  if (!session) {
    return (
      <>
        <AppHeader title="Interview Session" />
        <PageShell>
          <ErrorState message="Session not found." />
        </PageShell>
      </>
    );
  }

  if (!questions.length) {
    return (
      <>
        <AppHeader title="Interview Session" />
        <PageShell>
          <ErrorState message="No questions are available for this interview yet." />
        </PageShell>
      </>
    );
  }

  return (
    <>
      <AppHeader title="Interview Session" />

      <PageShell>
        <div className="flex h-full flex-col gap-4">
          {error ? (
            <div
              className="shrink-0 rounded-xl bg-[var(--surface)]
border-l-4 border-[var(--yellow-accent)] px-4 py-3 text-sm text-[var(--yellow-soft)]"
            >
              {error}
            </div>
          ) : null}

          <InterviewStagePanels
            stage={stage}
            aiSpeaking={aiSpeaking}
            userSpeaking={userSpeaking}
            userReady={userReady}
          />

          <QuestionBar
            question={currentQuestion.text}
            currentIndex={currentIndex}
            totalQuestions={questions.length}
            className="shrink-0"
          />

          <div className="surface flex flex-1 flex-col p-5">
            <div className="mb-4 flex items-center justify-between gap-3">
              <StatusPill
                label={stageStatus.label}
                variant={stageStatus.variant}
              />
              <StatusPill
                label={`Question ${currentIndex + 1} of ${questions.length}`}
                variant="muted"
              />
            </div>

            <div className="flex-1">
              <AnswerComposer
                ref={answerRef}
                value={answerText}
                onChange={setAnswerText}
                placeholder="Type your answer here..."
                disabled={
                  stage !== "answering" ||
                  aiSpeaking ||
                  submitting ||
                  completing
                }
                rows={5}
              />
            </div>

            <InterviewActions
              className="mt-4"
              onSubmit={handleSubmitAnswer}
              onFinish={handleCompleteSession}
              onStartSpeaking={handleStartSpeaking}
              onStopSpeaking={handleStopSpeaking}
              isUserSpeaking={userSpeaking}
              isSubmitting={submitting}
              isCompleting={completing}
              isLastQuestion={isLastQuestion}
              disabled={stage !== "answering"}
            />
          </div>
        </div>
      </PageShell>
    </>
  );
}
