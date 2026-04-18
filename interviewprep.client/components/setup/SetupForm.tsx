"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";
import { createSession } from "@/services/sessionApi";
import { ContextTextArea } from "./ContextTextArea";

export function SetupForm() {
  const router = useRouter();

  const [cvText, setCvText] = useState("");
  const [jobSpecText, setJobSpecText] = useState("");
  const [companyText, setCompanyText] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState("");

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setError("");

    if (!cvText.trim() || !jobSpecText.trim() || !companyText.trim()) {
      setError("Please fill in all fields before starting.");
      return;
    }

    try {
      setIsSubmitting(true);

      const session = await createSession({
        cvText: cvText.trim(),
        jobSpecText: jobSpecText.trim(),
        companyText: companyText.trim(),
      });

      router.push(`/session/${session.id}`);
    } catch (err) {
      console.error(err);
      setError("Something went wrong while starting the interview.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="surface glow-green w-full p-6 sm:p-8">
      <div className="mb-8 space-y-3">
        <p className="text-sm font-medium uppercase tracking-[0.2em] text-[var(--text-muted)]">
          Interview Prep
        </p>

        <h1 className="text-gradient text-3xl font-semibold tracking-tight sm:text-4xl">
          Prepare for the room before you enter it
        </h1>

        <p className="max-w-2xl text-sm leading-6 text-[var(--text-muted)] sm:text-base">
          Paste your CV, job spec, and company context to begin a focused mock
          interview experience.
        </p>
      </div>

      <form className="space-y-6" onSubmit={handleSubmit}>
        <ContextTextArea
          id="cvText"
          label="CV"
          placeholder="Paste your CV here..."
          value={cvText}
          onChange={setCvText}
          rows={6}
        />

        <ContextTextArea
          id="jobSpecText"
          label="Job specification"
          placeholder="Paste the job description here..."
          value={jobSpecText}
          onChange={setJobSpecText}
          rows={6}
        />

        <ContextTextArea
          id="companyText"
          label="Company context"
          placeholder="Paste company information here..."
          value={companyText}
          onChange={setCompanyText}
          rows={5}
        />

        {error ? <p className="text-sm text-red-300">{error}</p> : null}

        <div className="flex items-center justify-between pt-2">
          <p className="text-xs text-[var(--text-muted)]">
            This helps tailor the interview to your real experience.
          </p>

          <button
            type="submit"
            className="btn btn-primary disabled:cursor-not-allowed disabled:opacity-70"
            disabled={isSubmitting}
          >
            {isSubmitting ? "Starting..." : "Start interview"}
          </button>
        </div>
      </form>
    </div>
  );
}
