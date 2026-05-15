"use client";

import { useEffect, useState } from "react";
import { useParams } from "next/navigation";

import { Code2, MessageSquare } from "lucide-react";

import { getSessionById } from "@/services/sessionApi";

import type { InterviewSessionDto } from "@/types/session";

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
      <PageShell>
        <LoadingState
          title="Loading reflection..."
          message="Gathering your coaching insights."
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
        <ErrorState message="Reflection not found." />
      </PageShell>
    );
  }

  return (
    <PageShell className="py-4">
      <div className="space-y-6">
        <SectionTitle
          title="Interview Reflection"
          subtitle="A quiet reflection on your experience, your communication, and where to sharpen next."
        />

        <CoachHighlight
          title="Observation"
          message={session.observation || "Your interview reflection is ready."}
        />

        <div className="grid gap-5 md:grid-cols-2">
          <CoachingSection
            title="Technical Strength"
            icon={<Code2 size={20} />}
            content={`
${session.strengths || "Strong technical foundations were demonstrated."}

${session.overallImpression || ""}
            `.trim()}
          />

          <CoachingSection
            title="Communication & Growth"
            icon={<MessageSquare size={20} />}
            content={`
${session.communication || "Your answers were clear and structured."}

${session.growthOpportunity || ""}
            `.trim()}
          />
        </div>

        <CoachHighlight
          title="Next Focus"
          message={
            session.nextFocus || "Continue practicing real-world storytelling."
          }
        />
      </div>
    </PageShell>
  );
}
