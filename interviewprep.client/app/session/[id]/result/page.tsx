"use client";

import { useEffect, useState } from "react";
import { useParams } from "next/navigation";

import { Code2, MessageSquare, TrendingUp, Sparkles } from "lucide-react";

import { getSessionById } from "@/services/sessionApi";

import type { InterviewSessionDto } from "@/types/session";

import AppHeader from "@/components/common/AppHeader";
import ErrorState from "@/components/common/ErrorState";
import LoadingState from "@/components/common/LoadingState";
import PageShell from "@/components/common/PageShell";
import SectionTitle from "@/components/common/SectionTitle";

import CoachingSection from "@/components/result/CoachingSection";
import CoachHighlight from "@/components/result/CoachHighlight";

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

        const result = await getSessionById(sessionId);

        setSession(result);
      } catch (err) {
        console.error(err);

        setError("Failed to load your interview reflection.");
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
      <>
        <AppHeader title="Interview Reflection" stage="result" />
        <PageShell>
          <LoadingState
            title="Loading reflection..."
            message="Gathering your coaching insights."
          />
        </PageShell>
      </>
    );
  }

  if (error) {
    return (
      <>
        <AppHeader title="Interview Reflection" stage="result" />
        <PageShell>
          <ErrorState message={error} />
        </PageShell>
      </>
    );
  }

  if (!session) {
    return (
      <>
        <AppHeader title="Interview Reflection" stage="result" />
        <PageShell>
          <ErrorState message="Reflection not found." />
        </PageShell>
      </>
    );
  }

  return (
    <>
      <AppHeader title="Interview Reflection" stage="result" />
      <PageShell className="py-4">
      <div className="space-y-6">
        <SectionTitle
          title="Interview Reflection"
          subtitle="A quiet reflection on your experience, your communication, and where to sharpen next."
        />

        <CoachHighlight
          variant="open"
          title="Observation"
          message={session.observation || "Your interview reflection is ready."}
        />

        <div className="grid gap-4 md:grid-cols-2">
          <CoachingSection
            title="Strengths"
            icon={<Code2 size={18} />}
            content={
              session.strengths ||
              "Strong technical foundations were demonstrated."
            }
          />

          <CoachingSection
            title="Communication"
            icon={<MessageSquare size={18} />}
            content={
              session.communication || "Your answers were clear and structured."
            }
          />

          <CoachingSection
            title="Growth opportunity"
            icon={<TrendingUp size={18} />}
            content={
              session.growthOpportunity ||
              "Keep building on the areas above with more specific examples."
            }
          />

          <CoachingSection
            title="Overall impression"
            icon={<Sparkles size={18} />}
            content={
              session.overallImpression ||
              "A solid, composed interview overall."
            }
          />
        </div>

        <CoachHighlight
          variant="close"
          title="Next Focus"
          message={
            session.nextFocus || "Continue practicing real-world storytelling."
          }
        />
      </div>
      </PageShell>
    </>
  );
}
