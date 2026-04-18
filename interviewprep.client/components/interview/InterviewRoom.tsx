"use client";

import { useEffect, useState } from "react";
import {
  getSessionById,
  generateQuestions,
  getQuestions,
  submitAnswer,
  type InterviewSessionDto,
  type QuestionDto,
} from "@/services/sessionApi";

type InterviewRoomProps = {
  sessionId: string;
};

export function InterviewRoom({ sessionId }: InterviewRoomProps) {
  const [session, setSession] = useState<InterviewSessionDto | null>(null);
  const [questions, setQuestions] = useState<QuestionDto[]>([]);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [answerText, setAnswerText] = useState("");
  const [loading, setLoading] = useState(true);
  const [submittingAnswer, setSubmittingAnswer] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
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
      setSubmittingAnswer(true);
      setError("");

      await submitAnswer(sessionId, {
        questionId: currentQuestion.id,
        transcript: answerText.trim(),
      });

      setAnswerText("");

      if (!isLastQuestion) {
        setCurrentIndex((prev) => prev + 1);
      }
    } catch (err) {
      console.error(err);
      setError("Failed to submit your answer.");
    } finally {
      setSubmittingAnswer(false);
    }
  }

  return (
    <section className="flex min-h-[calc(100vh-3rem)] items-center justify-center">
      <div className="surface w-full max-w-5xl p-6 sm:p-8">
        <div className="mb-6 space-y-2">
          <p className="text-sm uppercase tracking-[0.2em] text-[var(--text-muted)]">
            Session
          </p>

          <h1 className="text-gradient text-3xl font-semibold tracking-tight">
            Interview Room
          </h1>

          <p className="text-sm text-[var(--text-muted)]">
            Session ID: {sessionId}
          </p>
        </div>

        {loading ? (
          <div className="rounded-2xl border border-[var(--border-soft)] p-6 text-sm text-[var(--text-muted)]">
            Preparing your interview...
          </div>
        ) : error && !currentQuestion ? (
          <div className="rounded-2xl border border-red-400/30 p-6 text-sm text-red-300">
            {error}
          </div>
        ) : (
          <div className="space-y-6">
            <div className="rounded-2xl border border-[var(--border-soft)] p-6">
              <div className="mb-3 flex items-center justify-between gap-4">
                <p className="text-xs uppercase tracking-[0.2em] text-[var(--text-muted)]">
                  Current question
                </p>

                <p className="text-xs text-[var(--text-muted)]">
                  {questions.length > 0
                    ? `Question ${currentIndex + 1} of ${questions.length}`
                    : "No questions"}
                </p>
              </div>

              <p className="text-lg leading-8 text-[var(--text-primary)]">
                {currentQuestion?.text ?? "No questions available yet."}
              </p>
            </div>

            <div className="rounded-2xl border border-[var(--border-soft)] p-6">
              <label
                htmlFor="answerText"
                className="mb-3 block text-xs uppercase tracking-[0.2em] text-[var(--text-muted)]"
              >
                Your answer
              </label>

              <textarea
                id="answerText"
                rows={8}
                value={answerText}
                onChange={(e) => setAnswerText(e.target.value)}
                placeholder="Write your answer here..."
                className="w-full rounded-xl border border-[var(--border-soft)] bg-transparent px-4 py-3 text-sm text-[var(--text-primary)] outline-none transition placeholder:text-white/30 focus:border-[var(--green-soft)]"
              />

              {error ? (
                <p className="mt-3 text-sm text-red-300">{error}</p>
              ) : null}

              <div className="mt-4 flex justify-end">
                <button
                  type="button"
                  onClick={handleSubmitAnswer}
                  disabled={submittingAnswer || !currentQuestion}
                  className="btn btn-primary disabled:cursor-not-allowed disabled:opacity-70"
                >
                  {submittingAnswer
                    ? "Submitting..."
                    : isLastQuestion
                      ? "Submit final answer"
                      : "Next question"}
                </button>
              </div>
            </div>

            <div className="rounded-2xl border border-[var(--border-soft)] p-6">
              <p className="mb-2 text-xs uppercase tracking-[0.2em] text-[var(--text-muted)]">
                Session status
              </p>

              <p className="text-sm text-[var(--text-primary)]">
                Status: {session?.status}
              </p>
            </div>
          </div>
        )}
      </div>
    </section>
  );
}
