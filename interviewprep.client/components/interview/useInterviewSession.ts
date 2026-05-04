import { useCallback, useEffect, useState } from "react";

import {
  completeSession,
  generateQuestions,
  getQuestions,
  getSessionById,
  submitAnswer,
} from "@/services/sessionApi";

import type { InterviewSessionDto, QuestionDto } from "@/types/session";

type UseInterviewSessionProps = {
  sessionId: string;
};

type UseInterviewSessionReturn = {
  session: InterviewSessionDto | null;
  questions: QuestionDto[];
  loading: boolean;
  error: string;

  loadSession: () => Promise<void>;
  submit: (questionId: string, answer: string) => Promise<void>;
  complete: () => Promise<void>;
};

export function useInterviewSession({
  sessionId,
}: UseInterviewSessionProps): UseInterviewSessionReturn {
  const [session, setSession] = useState<InterviewSessionDto | null>(null);

  const [questions, setQuestions] = useState<QuestionDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const loadSession = useCallback(async () => {
    try {
      setLoading(true);
      setError("");

      const sessionData = await getSessionById(sessionId);
      setSession(sessionData);

      let questionData = await getQuestions(sessionId);

      if (!questionData.length) {
        questionData = await generateQuestions(sessionId);
      }

      setQuestions(questionData);
    } catch (err) {
      console.error(err);
      setError("Failed to load interview.");
    } finally {
      setLoading(false);
    }
  }, [sessionId]);

  const submit = useCallback(
    async (questionId: string, answer: string) => {
      await submitAnswer(sessionId, {
        interviewQuestionId: questionId,
        transcript: answer,
      });
    },
    [sessionId],
  );

  const complete = useCallback(async () => {
    await completeSession(sessionId);
  }, [sessionId]);

  useEffect(() => {
    loadSession();
  }, [loadSession]);

  return {
    session,
    questions,
    loading,
    error,
    loadSession,
    submit,
    complete,
  };
}
