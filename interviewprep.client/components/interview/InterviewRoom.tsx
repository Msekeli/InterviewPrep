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
import type { InterviewSessionDto, QuestionDto } from "@/types/session";

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

  useEffect(() => {
    if (hasLoaded.current) return;
    hasLoaded.current = true;

    async function loadRoom() {
      try {
        setLoading(true);
        setError("");

        console.log("Loading session...");
        const sessionData = await getSessionById(sessionId);
        console.log("Session loaded:", sessionData);
        setSession(sessionData);

        console.log("Loading questions...");
        let questionData = await getQuestions(sessionId);
        console.log("Questions response:", questionData);

        if (!questionData.length) {
          console.log("No questions found. Generating...");
          questionData = await generateQuestions(sessionId);
          console.log("Generated questions:", questionData);
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

  async function handleSubmitAnswer() {
    if (!currentQuestion) return;

    if (!answerText.trim()) {
      setError("Please enter an answer before continuing.");
      return;
    }

    try {
      setSubmitting(true);
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
    } finally {
      setSubmitting(false);
    }
  }

  async function handleCompleteSession() {
    try {
      setCompleting(true);
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
    } finally {
      setCompleting(false);
    }
  }

  if (loading) {
    return (
      <section className="surface glow-green w-full p-6 sm:p-8">
        <p className="text-sm text-[var(--text-muted)]">
          Loading interview room...
        </p>
      </section>
    );
  }

  if (error && !session) {
    return (
      <section className="surface glow-green w-full p-6 sm:p-8">
        <p className="text-sm text-red-300">{error}</p>
      </section>
    );
  }

  if (!session) {
    return (
      <section className="surface glow-green w-full p-6 sm:p-8">
        <p className="text-sm text-red-300">Session not found.</p>
      </section>
    );
  }

  if (!questions.length) {
    return (
      <section className="surface glow-green w-full p-6 sm:p-8">
        <p className="text-sm text-red-300">
          No questions are available for this interview yet.
        </p>
      </section>
    );
  }

  return (
    <section className="surface glow-green w-full p-6 sm:p-8">
      <div className="mb-8 space-y-3">
        <p className="text-sm font-medium uppercase tracking-[0.2em] text-[var(--text-muted)]">
          Interview Session
        </p>

        <h1 className="text-gradient text-3xl font-semibold tracking-tight sm:text-4xl">
          Stay calm. Answer with clarity.
        </h1>

        <p className="text-sm leading-6 text-[var(--text-muted)] sm:text-base">
          Question {currentIndex + 1} of {questions.length}
        </p>
      </div>

      <div className="mb-6 rounded-2xl border border-white/10 bg-white/5 p-5">
        <p className="mb-2 text-xs uppercase tracking-[0.2em] text-[var(--text-muted)]">
          Current question
        </p>
        <p className="text-base leading-7 text-white sm:text-lg">
          {currentQuestion.text}
        </p>
      </div>

      <div className="space-y-3">
        <label
          htmlFor="answer"
          className="text-sm font-medium text-[var(--text-muted)]"
        >
          Your answer
        </label>

        <textarea
          id="answer"
          value={answerText}
          onChange={(e) => setAnswerText(e.target.value)}
          placeholder="Type your answer here..."
          rows={8}
          className="w-full rounded-2xl border border-white/10 bg-white/5 px-4 py-3 text-sm text-white outline-none transition placeholder:text-[var(--text-muted)] focus:border-[var(--accent)]"
          disabled={submitting || completing}
        />
      </div>

      {error ? <p className="mt-4 text-sm text-red-300">{error}</p> : null}

      <div className="mt-6 flex items-center justify-between gap-3">
        <p className="text-xs text-[var(--text-muted)]">
          Keep your answer grounded in real experience.
        </p>

        <button
          type="button"
          onClick={handleSubmitAnswer}
          disabled={submitting || completing}
          className="btn btn-primary disabled:cursor-not-allowed disabled:opacity-70"
        >
          {completing
            ? "Finishing..."
            : submitting
              ? "Submitting..."
              : isLastQuestion
                ? "Finish interview"
                : "Next question"}
        </button>
      </div>
    </section>
  );
}
