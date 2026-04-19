"use client";

import { useEffect, useState } from "react";
import { useParams } from "next/navigation";
import { getSessionById } from "@/services/sessionApi";
import type { InterviewSessionDto } from "@/types/session";
import ErrorState from "@/components/common/ErrorState";
import LoadingState from "@/components/common/LoadingState";
import PageShell from "@/components/common/PageShell";
import SectionTitle from "@/components/common/SectionTitle";
import FeedbackPanel from "@/components/result/FeedbackPanel";
import NextStepNote from "@/components/result/NextStepNote";
import ResultSummary from "@/components/result/ResultSummary";
import ScoreDisplay from "@/components/result/ScoreDisplay";

export default function ResultPage() {
  const params = useParams<{ id: string }>();
  const sessionId = params.id;

  const [session, setSession] = useState<InterviewSessionDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    async function loadResult() {
      try {
        setLoading(true);
        setError("");

        const sessionData = await getSessionById(sessionId);
        setSession(sessionData);
      } catch (err) {
        console.error(err);
        setError("Failed to load your interview result.");
      } finally {
        setLoading(false);
      }
    }

    if (sessionId) {
      loadResult();
    }
  }, [sessionId]);

  if (loading) {
    return (
      <PageShell>
        <LoadingState
          title="Loading results..."
          message="Gathering your completed interview feedback."
        />
      </PageShell>
    );
  }

  if (error) {
    return (
      <PageShell>
        <ErrorState message={error} />
      </PageShell>
    );
  }

  if (!session) {
    return (
      <PageShell>
        <ErrorState message="Result not found." />
      </PageShell>
    );
  }

  return (
    <PageShell className="py-4">
      <div className="flex h-full min-h-0 w-full flex-col gap-5">
        <SectionTitle
          title="Your interview result"
          subtitle="A quiet reflection on how the conversation went and where to sharpen next."
        />

        <div className="min-h-0 flex-1 overflow-y-auto pr-1">
          <ResultSummary
            className="gap-5"
            score={
              <ScoreDisplay
                score={session.overallScore ?? 0}
                label="Overall Score"
              />
            }
            feedback={
              <FeedbackPanel
                feedback={
                  session.feedback?.trim() ||
                  "No written feedback is available for this session yet."
                }
              />
            }
            nextStep={<NextStepNote />}
          />
        </div>
      </div>
    </PageShell>
  );
}
