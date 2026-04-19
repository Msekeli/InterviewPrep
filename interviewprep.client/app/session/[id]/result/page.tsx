"use client";

import { useEffect, useState } from "react";
import { getSessionById } from "@/services/sessionApi";
import { InterviewSessionDto } from "@/types/session";

type ResultPageProps = {
  params: {
    id: string;
  };
};

export default function ResultPage({ params }: ResultPageProps) {
  const [session, setSession] = useState<InterviewSessionDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    async function loadResult() {
      try {
        setLoading(true);
        setError("");

        const sessionData = await getSessionById(params.id);
        setSession(sessionData);
      } catch (err) {
        console.error(err);
        setError("Failed to load interview results.");
      } finally {
        setLoading(false);
      }
    }

    loadResult();
  }, [params.id]);

  return (
    <section className="flex min-h-[calc(100vh-3rem)] items-center justify-center">
      <div className="surface w-full max-w-3xl p-6 sm:p-8">
        <div className="space-y-3">
          <p className="text-sm uppercase tracking-[0.2em] text-[var(--text-muted)]">
            Results
          </p>

          <h1 className="text-gradient text-3xl font-semibold tracking-tight">
            Interview complete
          </h1>

          <p className="text-sm text-[var(--text-muted)]">
            Here is how your session went.
          </p>
        </div>

        {loading ? (
          <div className="mt-6 rounded-2xl border border-[var(--border-soft)] p-6 text-sm text-[var(--text-muted)]">
            Loading results...
          </div>
        ) : error ? (
          <div className="mt-6 rounded-2xl border border-red-400/30 p-6 text-sm text-red-300">
            {error}
          </div>
        ) : !session ? (
          <div className="mt-6 rounded-2xl border border-red-400/30 p-6 text-sm text-red-300">
            Result not found.
          </div>
        ) : (
          <div className="mt-6 space-y-6">
            <div className="rounded-2xl border border-[var(--border-soft)] p-6">
              <p className="text-xs uppercase tracking-[0.2em] text-[var(--text-muted)]">
                Overall score
              </p>

              <div className="mt-3 flex items-end gap-2">
                <span className="text-4xl font-semibold text-white sm:text-5xl">
                  {session.overallScore ?? 0}
                </span>
                <span className="pb-1 text-sm text-[var(--text-muted)]">
                  / 100
                </span>
              </div>
            </div>

            <div className="rounded-2xl border border-[var(--border-soft)] p-6">
              <p className="text-xs uppercase tracking-[0.2em] text-[var(--text-muted)]">
                Feedback
              </p>

              <p className="mt-3 whitespace-pre-line text-sm leading-7 text-white/90 sm:text-base">
                {session.feedback?.trim()
                  ? session.feedback
                  : "No feedback is available yet."}
              </p>
            </div>

            {session.completedAtUtc ? (
              <div className="rounded-2xl border border-[var(--border-soft)] p-6">
                <p className="text-xs uppercase tracking-[0.2em] text-[var(--text-muted)]">
                  Completed
                </p>

                <p className="mt-3 text-sm text-white/90">
                  {new Date(session.completedAtUtc).toLocaleString()}
                </p>
              </div>
            ) : null}
          </div>
        )}
      </div>
    </section>
  );
}
