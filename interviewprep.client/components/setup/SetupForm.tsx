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
      {/* HEADER */}
      <AppHeader title="Setup Interview" stage="setup" />

      <PageShell className="py-8">
        <div className="w-full max-w-5xl mx-auto">
          {/* MAIN SURFACE */}
          <div className="surface border border-[var(--border-soft)] rounded-2xl p-8 shadow-[0_0_25px_rgba(34,197,94,0.10)]">
            {/* ERROR */}
            {error ? (
              <div className="mb-6 rounded-xl border border-red-400/30 bg-red-400/10 px-4 py-3 text-sm text-red-200">
                {error}
              </div>
            ) : null}

            {/* HERO + TIP (BALANCED LAYOUT) */}
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
              {/* LEFT: TITLE + DESCRIPTION */}
              <div className="lg:col-span-2 space-y-3">
                <h1 className="text-4xl font-semibold tracking-tight text-gradient leading-tight">
                  Build your interview context
                </h1>

                <p className="text-base text-[var(--text-muted)] leading-relaxed max-w-2xl">
                  Paste your CV, job description, and company details. This
                  defines the realism and depth of your interview experience.
                </p>
              </div>

              {/* RIGHT: TIP CARD */}
              <div className="surface border border-[var(--border-soft)] rounded-2xl p-5">
                <p className="text-sm font-semibold text-[var(--text-primary)] mb-2">
                  Tip
                </p>

                <p className="text-sm text-[var(--text-muted)] leading-relaxed">
                  Use real job descriptions and your actual experience. The
                  closer the input, the more accurate your interview becomes.
                </p>
              </div>
            </div>

            {/* FORM */}
            <form className="flex flex-col gap-6" onSubmit={handleSubmit}>
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

              {/* ACTION BAR */}
              <div className="flex items-center justify-between border-t border-[var(--border-soft)] pt-6">
                <p className="text-sm text-[var(--text-muted)]">
                  All fields must be completed before starting.
                </p>

                <StartInterviewAction
                  isSubmitting={isSubmitting}
                  disabled={isSubmitting}
                />
              </div>
            </form>
          </div>
        </div>
      </PageShell>
    </>
  );
}
