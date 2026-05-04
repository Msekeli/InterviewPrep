import { useEffect, useRef, useState } from "react";

import { useInterviewSession } from "./useInterviewSession";
import { useSpeechController } from "./useSpeechController";
import { useConversationEngine } from "./useConversationEngine";

export function useInterviewController(sessionId: string) {
  const session = useInterviewSession({ sessionId });
  const speech = useSpeechController();
  const convo = useConversationEngine();

  const [hasStarted, setHasStarted] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isCompleting, setIsCompleting] = useState(false);
  const [answerText, setAnswerText] = useState("");
  const [index, setIndex] = useState(0);
  const lastSpokenQuestionId = useRef<string | null>(null);

  const questions = session.questions || [];
  const hasCompletedQuestions = index >= questions.length;
  const currentQuestion = hasCompletedQuestions ? null : questions[index];
  const isLast = hasCompletedQuestions;

  const startInterview = () => {
    setHasStarted(true);
  };

  const finishInterview = async () => {
    try {
      setIsCompleting(true);
      await session.complete();
    } finally {
      setIsCompleting(false);
    }
  };

  // Speak each question once after interview starts.
  useEffect(() => {
    if (!hasStarted) return;
    if (!questions.length) return;
    if (!currentQuestion) return;
    if (lastSpokenQuestionId.current === currentQuestion.id) return;

    lastSpokenQuestionId.current = currentQuestion.id;
    speech.speak(currentQuestion.text);
  }, [hasStarted, questions.length, currentQuestion, speech]);

  // 🎤 SPEAKING
  const handleStartSpeaking = () => {
    speech.startMic(
      (partial) => setAnswerText(partial),
      (final) => setAnswerText(final),
    );
  };

  const handleStopSpeaking = () => {
    speech.stopMic();
  };

  // 📦 SUBMIT FLOW (RESTORED SIMPLE LOGIC)
  const submitAnswer = async () => {
    if (!currentQuestion) return;
    if (!answerText.trim()) return;

    try {
      setIsSubmitting(true);
      await session.submit(currentQuestion.id, answerText.trim());
      setAnswerText("");
      setIndex((i) => i + 1);
    } finally {
      setIsSubmitting(false);
    }
  };

  // reset local state when a new session loads
  useEffect(() => {
    setHasStarted(false);
    setIsSubmitting(false);
    setIsCompleting(false);
    setAnswerText("");
    setIndex(0);
    lastSpokenQuestionId.current = null;
  }, [sessionId]);

  return {
    ...session,
    ...speech,
    ...convo,

    hasStarted,
    startInterview,
    finishInterview,

    isSubmitting,
    isCompleting,

    answerText,
    setAnswerText,

    currentQuestion,
    index,
    isLast,

    handleStartSpeaking,
    handleStopSpeaking,
    submitAnswer,
  };
}
