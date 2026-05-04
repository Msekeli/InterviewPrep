import { useEffect, useState } from "react";

import { useInterviewSession } from "./useInterviewSession";
import { useSpeechController } from "./useSpeechController";
import { useConversationEngine } from "./useConversationEngine";

export function useInterviewController(sessionId: string) {
  const session = useInterviewSession({ sessionId });
  const speech = useSpeechController();
  const convo = useConversationEngine();

  const [answerText, setAnswerText] = useState("");
  const [index, setIndex] = useState(0);

  const questions = session.questions || [];
  const currentQuestion = questions[index];
  const isLast = index >= questions.length - 1;

  // 🔊 RESTORE OLD SIMPLE AI SPEECH FLOW
  useEffect(() => {
    if (!questions.length) return;
    if (!currentQuestion) return;

    speech.speak(currentQuestion.text);
  }, [currentQuestion?.text]);

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

    await session.submit(currentQuestion.id, answerText);

    setAnswerText("");

    if (isLast) {
      await session.complete();
      return;
    }

    setIndex((i) => i + 1);
  };

  return {
    ...session,
    ...speech,
    ...convo,

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
