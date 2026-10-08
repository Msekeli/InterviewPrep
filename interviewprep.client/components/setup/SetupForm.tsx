"use client";

import { useRouter } from "next/navigation";
import { useEffect, useRef, useState } from "react";
import { createSession } from "@/services/sessionApi";
import PageShell from "../common/PageShell";
import AppHeader from "../common/AppHeader";
import StartInterviewAction from "./StartInterviewAction";
import { ContextTextArea } from "./ContextTextArea";

export function SetupForm() {
  const router = useRouter();
  const errorTimeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  const [cvText, setCvText] = useState("");
  const [jobSpecText, setJobSpecText] = useState("");
  const [companyText, setCompanyText] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    return () => {
      if (errorTimeoutRef.current) clearTimeout(errorTimeoutRef.current);
    };
  }, []);

  function showTemporaryError(message: string) {
    setError(message);
    if (errorTimeoutRef.current) clearTimeout(errorTimeoutRef.current);
    errorTimeoutRef.current = setTimeout(() => setError(""), 3000);
  }

  function update(setter: (value: string) => void) {
    return (value: string) => {
      setter(value);
      if (error) setError("");
    };
  }

  async function handleStart() {
    if (!cvText.trim()) {
      showTemporaryError("Please paste your CV before starting.");
      return;
    }
    if (!jobSpecText.trim()) {
      showTemporaryError("Please paste the job description before starting.");
      return;
    }

    try {
      setError("");
      setIsSubmitting(true);

      const session = await createSession({
        cvText: cvText.trim(),
        jobSpecText: jobSpecText.trim(),
        companyText: companyText.trim(),
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
    <>
      <AppHeader title="Setup Interview" stage="setup" />

      <PageShell className="py-4">
        <div className="mx-auto flex w-full max-w-6xl flex-col lg:h-[calc(100dvh-96px)]">
          <div className="surface flex flex-col rounded-2xl border border-[var(--border-soft)] p-6 shadow-[0_0_25px_rgba(94,234,212,0.08)] sm:p-8 lg:h-full">
            {error ? (
              <div className="mb-4 shrink-0 rounded-xl border border-red-400/30 bg-red-400/10 px-4 py-3 text-sm text-red-200">
                {error}
              </div>
            ) : null}

            <div className="mb-5 shrink-0">
              <h1 className="font-display text-xl font-bold tracking-tight text-[var(--text-primary)] sm:text-2xl">
                Set up your interview
              </h1>
              <p className="mt-1 text-sm leading-relaxed text-[var(--text-muted)]">
                Paste your CV and the job spec. The closer they are to the real
                thing, the more accurate the interview.
              </p>
            </div>

            <div className="flex min-h-0 flex-1 flex-col gap-5">
              <div className="grid min-h-0 flex-1 grid-cols-1 gap-5 lg:grid-cols-2">
                <div className="min-h-[220px] lg:min-h-0">
                  <ContextTextArea
                    id="cvText"
                    label="CV"
                    placeholder="Paste your CV here..."
                    value={cvText}
                    onChange={update(setCvText)}
                    maxLength={20000}
                    fill
                  />
                </div>
                <div className="min-h-[220px] lg:min-h-0">
                  <ContextTextArea
                    id="jobSpecText"
                    label="Job description"
                    placeholder="Paste the job description here..."
                    value={jobSpecText}
                    onChange={update(setJobSpecText)}
                    maxLength={10000}
                    fill
                  />
                </div>
              </div>

              <div className="shrink-0">
                <ContextTextArea
                  id="companyText"
                  label="Company overview"
                  placeholder="Optional: paste company information so questions can mention the company by name."
                  value={companyText}
                  onChange={update(setCompanyText)}
                  maxLength={5000}
                  rows={3}
                  optional
                />
              </div>
            </div>

            <div className="mt-5 flex shrink-0 flex-col gap-3 border-t border-[var(--border-soft)] pt-4 sm:flex-row sm:items-center sm:justify-between">
              <p className="text-sm text-[var(--text-muted)]">
                Everything here stays on this device until you start.
              </p>
              <StartInterviewAction
                isSubmitting={isSubmitting}
                disabled={isSubmitting}
                onClick={handleStart}
              />
            </div>
          </div>
        </div>
      </PageShell>
    </>
  );
}
