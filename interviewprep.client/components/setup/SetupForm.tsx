"use client";

import { useRouter } from "next/navigation";
import { useEffect, useRef, useState } from "react";
import { createSession } from "@/services/sessionApi";
import PageShell from "../common/PageShell";
import AppHeader from "../common/AppHeader";
import SecondaryButton from "../common/SecondaryButton";
import PrimaryButton from "../common/PrimaryButton";
import StartInterviewAction from "./StartInterviewAction";
import { ContextTextArea } from "./ContextTextArea";

type Step = 0 | 1 | 2;

const STEP_COUNT = 3;

const STEP_META: { title: string; description: string }[] = [
  {
    title: "Paste your CV",
    description:
      "This anchors the interview to your real background and experience.",
  },
  {
    title: "Paste the job spec",
    description:
      "The closer this is to the real posting, the more accurate the interview.",
  },
  {
    title: "Company context",
    description:
      "Optional — helps the questions reference the company by name.",
  },
];

export function SetupForm() {
  const router = useRouter();
  const errorTimeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  const [step, setStep] = useState<Step>(0);
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

  function goNext() {
    if (step === 0 && !cvText.trim()) {
      showTemporaryError("Please paste your CV before continuing.");
      return;
    }

    if (step === 1 && !jobSpecText.trim()) {
      showTemporaryError("Please paste the job description before continuing.");
      return;
    }

    setError("");
    setStep((s) => (s < STEP_COUNT - 1 ? ((s + 1) as Step) : s));
  }

  function goBack() {
    setError("");
    setStep((s) => (s > 0 ? ((s - 1) as Step) : s));
  }

  async function handleStart() {
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

  const meta = STEP_META[step];

  return (
    <>
      <AppHeader title="Setup Interview" stage="setup" />

      <PageShell className="py-4">
        <div className="mx-auto flex h-[calc(100dvh-96px)] w-full max-w-4xl flex-col">
          <div className="surface flex h-full flex-col rounded-2xl border border-[var(--border-soft)] p-6 shadow-[0_0_25px_rgba(84,87,214,0.12)] sm:p-8">
            {/* ERROR */}
            {error ? (
              <div className="mb-4 shrink-0 rounded-xl border border-red-400/30 bg-red-400/10 px-4 py-3 text-sm text-red-200">
                {error}
              </div>
            ) : null}

            {/* PROGRESS + HEADER */}
            <div className="mb-4 shrink-0 space-y-3">
              <div className="flex items-center gap-1.5">
                {Array.from({ length: STEP_COUNT }).map((_, i) => (
                  <span
                    key={i}
                    className={[
                      "h-1.5 rounded-full transition-all duration-500 ease-out",
                      i === step
                        ? "w-6 bg-[linear-gradient(135deg,var(--yellow-accent),var(--yellow-soft))]"
                        : "w-1.5",
                      i < step ? "bg-[var(--green-primary)]" : "",
                      i > step ? "bg-[var(--border-soft)]" : "",
                    ]
                      .join(" ")
                      .trim()}
                  />
                ))}

                <span className="ml-2 text-xs text-[var(--text-muted)]">
                  Step {step + 1} of {STEP_COUNT}
                </span>
              </div>

              <div>
                <h1 className="font-display text-xl font-medium tracking-tight text-[var(--text-primary)] sm:text-2xl">
                  {meta.title}
                </h1>
                <p className="mt-1 text-sm leading-relaxed text-[var(--text-muted)]">
                  {meta.description}
                </p>
              </div>
            </div>

            {/* FIELD — fills whatever height is left, never overflows */}
            <div className="min-h-0 flex-1">
              {step === 0 ? (
                <ContextTextArea
                  id="cvText"
                  label="CV"
                  placeholder="Paste your CV here..."
                  value={cvText}
                  onChange={(value) => {
                    setCvText(value);
                    if (error) setError("");
                  }}
                  maxLength={20000}
                  fill
                />
              ) : null}

              {step === 1 ? (
                <ContextTextArea
                  id="jobSpecText"
                  label="Job description"
                  placeholder="Paste the job description here..."
                  value={jobSpecText}
                  onChange={(value) => {
                    setJobSpecText(value);
                    if (error) setError("");
                  }}
                  maxLength={10000}
                  fill
                />
              ) : null}

              {step === 2 ? (
                <ContextTextArea
                  id="companyText"
                  label="Company overview"
                  placeholder="Paste company information here..."
                  value={companyText}
                  onChange={(value) => {
                    setCompanyText(value);
                    if (error) setError("");
                  }}
                  maxLength={5000}
                  optional
                  fill
                />
              ) : null}
            </div>

            {/* FOOTER NAV */}
            <div className="mt-4 flex shrink-0 items-center justify-between border-t border-[var(--border-soft)] pt-4">
              {step > 0 ? (
                <SecondaryButton onClick={goBack} disabled={isSubmitting}>
                  Back
                </SecondaryButton>
              ) : (
                <p className="text-sm text-[var(--text-muted)]">
                  Everything here stays on this device until you start.
                </p>
              )}

              {step < STEP_COUNT - 1 ? (
                <PrimaryButton onClick={goNext}>Continue</PrimaryButton>
              ) : (
                <StartInterviewAction
                  isSubmitting={isSubmitting}
                  disabled={isSubmitting}
                  onClick={handleStart}
                />
              )}
            </div>
          </div>
        </div>
      </PageShell>
    </>
  );
}
