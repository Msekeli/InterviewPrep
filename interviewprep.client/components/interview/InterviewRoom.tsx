"use client";

import { useEffect, useMemo, useRef, useState } from "react";
import { useRouter } from "next/navigation";
import {
  completeSession,
  generateQuestions,
  getQuestions,
  getSessionById,
  submitAnswer,
} from "@/services/sessionApi";
import type { InterviewSessionDto, QuestionDto } from "@/types/session";
import ErrorState from "../common/ErrorState";
import LoadingState from "../common/LoadingState";
import PageShell from "../common/PageShell";
import StatusPill from "../common/StatusPill";
import AnswerComposer from "./AnswerComposer";
import InterviewActions from "./InterviewActions";
import ParticipantPanel from "./ParticipantPanel";
import QuestionBar from "./QuestionBar";
import StageHeader from "./StageHeader";

type InterviewRoomProps = {
  sessionId: string;
};

type InterviewStage = "asking" | "answering" | "submitting" | "completing";

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

  useEffect(() => {
    if (!currentQuestion || loading || submitting || completing) return;

    setStage("asking");

    const timer = window.setTimeout(() => {
      setStage("answering");
    }, 2500);

    return () => window.clearTimeout(timer);
  }, [currentQuestion, loading, submitting, completing]);

  const stageStatus = useMemo(() => {
    if (completing) {
      return {
        label: "Finishing",
        variant: "warning" as const,
      };
    }

    if (submitting) {
      return {
        label: "Submitting",
        variant: "warning" as const,
      };
    }

    if (stage === "asking") {
      return {
        label: "AI turn",
        variant: "default" as const,
      };
    }

    return {
      label: "Your turn",
      variant: "success" as const,
    };
  }, [stage, submitting, completing]);

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

  if (loading) {
    return (
      <PageShell contentClassName="max-w-6xl">
        <LoadingState
          title="Loading interview room..."
          message="Preparing your session, questions, and interview stage."
        />
      </PageShell>
    );
  }

  if (error && !session) {
    return (
      <PageShell contentClassName="max-w-6xl">
        <ErrorState message={error} />
      </PageShell>
    );
  }

  if (!session) {
    return (
      <PageShell contentClassName="max-w-6xl">
        <ErrorState message="Session not found." />
      </PageShell>
    );
  }

  if (!questions.length) {
    return (
      <PageShell contentClassName="max-w-6xl">
        <ErrorState message="No questions are available for this interview yet." />
      </PageShell>
    );
  }

  return (
    <PageShell className="py-4 sm:py-6" contentClassName="max-w-7xl">
      <div className="flex min-h-[calc(100dvh-2rem)] flex-col gap-4 sm:gap-5">
        <StageHeader
          title="Interview Session"
          subtitle="Stay calm. Answer with clarity. Let your real experience speak."
          statusLabel={stageStatus.label}
          statusVariant={stageStatus.variant}
        />

        <div className="grid gap-4 lg:grid-cols-2">
          <ParticipantPanel
            title="AI Interviewer"
            subtitle={
              stage === "asking"
                ? "The interviewer is asking the current question."
                : "Waiting for your response."
            }
            badge={stage === "asking" ? "Asking" : "Listening"}
            className={[
              "min-h-[220px] transition-all duration-300",
              stage === "asking"
                ? "ring-1 ring-[rgba(34,197,94,0.45)] shadow-[var(--glow-green)]"
                : "",
            ].join(" ")}
          >
            <div className="flex h-full min-h-[140px] items-center justify-center">
              <div className="flex flex-col items-center text-center">
                <div className="mb-4 flex h-20 w-20 items-center justify-center rounded-full border border-[var(--border-soft)] bg-[rgba(255,255,255,0.05)] text-3xl">
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
              "min-h-[220px] transition-all duration-300",
              stage === "answering"
                ? "ring-1 ring-[rgba(234,179,8,0.45)] shadow-[var(--glow-yellow)]"
                : "",
            ].join(" ")}
          >
            <div className="flex h-full min-h-[140px] items-center justify-center">
              <div className="flex flex-col items-center text-center">
                <div className="mb-4 flex h-20 w-20 items-center justify-center rounded-full border border-[rgba(234,179,8,0.25)] bg-[rgba(234,179,8,0.08)] text-3xl">
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

        <QuestionBar
          question={currentQuestion.text}
          currentIndex={currentIndex}
          totalQuestions={questions.length}
          className="shrink-0"
        />

        <div className="grid gap-4 xl:grid-cols-[minmax(0,1fr)_280px]">
          <div className="surface p-4 sm:p-5">
            <div className="mb-4 flex flex-wrap items-center gap-3">
              <StatusPill
                label={stage === "asking" ? "Question on screen" : "Answer now"}
                variant={stage === "asking" ? "default" : "success"}
              />
              <StatusPill
                label={`Question ${currentIndex + 1} of ${questions.length}`}
                variant="muted"
              />
            </div>

            <AnswerComposer
              value={answerText}
              onChange={setAnswerText}
              placeholder="Type your answer here..."
              disabled={stage !== "answering" || submitting || completing}
              rows={7}
            />

            {error ? (
              <p className="mt-4 text-sm text-red-300">{error}</p>
            ) : null}

            <InterviewActions
              className="mt-5"
              onSubmit={handleSubmitAnswer}
              onFinish={handleCompleteSession}
              isSubmitting={submitting}
              isCompleting={completing}
              isLastQuestion={isLastQuestion}
              disabled={stage !== "answering"}
            />
          </div>

          <aside className="surface p-4 sm:p-5">
            <p className="text-xs font-semibold uppercase tracking-[0.18em] text-[var(--text-muted)]">
              Interview notes
            </p>

            <div className="mt-4 space-y-4 text-sm leading-6 text-[var(--text-muted)]">
              <p>
                Keep your answers clear, specific, and tied to real work you
                have done.
              </p>
              <p>Structure helps: situation, task, action, result.</p>
              <p>
                The question remains visible below the stage so the flow stays
                natural even before speech is added.
              </p>
            </div>
          </aside>
        </div>
      </div>
    </PageShell>
  );
}
