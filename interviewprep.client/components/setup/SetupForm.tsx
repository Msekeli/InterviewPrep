"use client";

import { useRouter } from "next/navigation";
import { useEffect, useRef, useState } from "react";
import { createSession } from "@/services/sessionApi";
import PageShell from "../common/PageShell";
import LevelSelector from "./LevelSelector";
import StartInterviewAction from "./StartInterviewAction";
import { ContextTextArea } from "./ContextTextArea";

export function SetupForm() {
  const router = useRouter();
  const errorTimeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  const [cvText, setCvText] = useState("");
  const [jobSpecText, setJobSpecText] = useState("");
  const [companyText, setCompanyText] = useState("");
  const [targetLevel, setTargetLevel] = useState<number>(2);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    return () => {
      if (errorTimeoutRef.current) {
        clearTimeout(errorTimeoutRef.current);
      }
    };
  }, []);

  function showTemporaryError(message: string) {
    setError(message);

    if (errorTimeoutRef.current) {
      clearTimeout(errorTimeoutRef.current);
    }

    errorTimeoutRef.current = setTimeout(() => {
      setError("");
    }, 3000);
  }

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();

    const missingFields: string[] = [];

    if (!cvText.trim()) missingFields.push("CV");
    if (!jobSpecText.trim()) missingFields.push("job description");
    if (!companyText.trim()) missingFields.push("company overview");

    if (missingFields.length > 0) {
      showTemporaryError(`Please complete: ${missingFields.join(", ")}.`);
      return;
    }

    try {
      setError("");
      setIsSubmitting(true);

      const session = await createSession({
        cvText: cvText.trim(),
        jobSpecText: jobSpecText.trim(),
        companyText: companyText.trim(),
        targetLevel,
      });

      router.push(`/session/${session.id}`);
    } catch (err) {
      console.error(err);
      showTemporaryError("Something went wrong while starting the interview.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <PageShell className="py-6">
      <div className="surface glow-green w-full p-6">
        {error ? (
          <div className="pointer-events-none fixed left-1/2 top-[88px] z-50 -translate-x-1/2 rounded-xl border border-red-400/25 bg-red-400/10 px-4 py-3 shadow-lg backdrop-blur-md">
            <p className="text-sm font-medium text-red-200">{error}</p>
          </div>
        ) : null}

        <div className="grid w-full grid-cols-[1fr_auto] items-start gap-8">
          <div className="space-y-3">
            <h1 className="text-3xl font-semibold tracking-tight text-gradient">
              Build your interview context
            </h1>

            <p className="max-w-xl text-base leading-7 text-[var(--text-muted)]">
              Paste your CV, the job description, and a short company overview.
              The more relevant your context, the more realistic and useful your
              interview will be.
            </p>
          </div>

          <div className="inline-flex w-fit max-w-sm flex-col rounded-2xl border border-[rgba(34,197,94,0.18)] bg-[rgba(34,197,94,0.08)] px-4 py-3">
            <p className="text-sm font-semibold text-[var(--text-primary)]">
              💡 Tip
            </p>
            <p className="mt-1 text-sm leading-6 text-[var(--text-muted)]">
              Use the actual role, your real experience, and a little company
              context so the interview feels sharper and more tailored.
            </p>
          </div>
        </div>

        <form className="mt-6 flex flex-col gap-5" onSubmit={handleSubmit}>
          <ContextTextArea
            id="cvText"
            label="CV"
            placeholder="Paste your CV here..."
            value={cvText}
            onChange={(value) => {
              setCvText(value);
              if (error) setError("");
            }}
            rows={3}
          />

          <ContextTextArea
            id="jobSpecText"
            label="Job description"
            placeholder="Paste the job description here..."
            value={jobSpecText}
            onChange={(value) => {
              setJobSpecText(value);
              if (error) setError("");
            }}
            rows={3}
          />

          <ContextTextArea
            id="companyText"
            label="Company overview"
            placeholder="Paste company information here..."
            value={companyText}
            onChange={(value) => {
              setCompanyText(value);
              if (error) setError("");
            }}
            rows={3}
          />

          <LevelSelector value={targetLevel} onChange={setTargetLevel} />

          <div className="flex items-center justify-between gap-3 border-t border-[var(--border-soft)] pt-4">
            <p className="text-sm leading-5 text-[var(--text-muted)]">
              Fill in all areas before starting your interview.
            </p>

            <StartInterviewAction
              isSubmitting={isSubmitting}
              disabled={isSubmitting}
            />
          </div>
        </form>
      </div>
    </PageShell>
  );
}
