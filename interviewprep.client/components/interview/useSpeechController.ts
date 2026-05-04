import { useState, useCallback } from "react";
import { speakText } from "@/services/speechService";
import { startListening, stopListening } from "@/services/speechToTextService";

type SpeechController = {
  aiSpeaking: boolean;
  userSpeaking: boolean;

  speak: (text: string) => Promise<void>;

  startMic: (
    onPartial: (text: string) => void,
    onFinal: (text: string) => void,
  ) => void;

  stopMic: () => void;
};

export function useSpeechController(): SpeechController {
  const [aiSpeaking, setAiSpeaking] = useState(false);
  const [userSpeaking, setUserSpeaking] = useState(false);

  const speak = useCallback(async (text: string) => {
    setAiSpeaking(true);

    try {
      await speakText(text);
    } finally {
      setAiSpeaking(false);
    }
  }, []);

  const startMic = useCallback(
    (onPartial: (text: string) => void, onFinal: (text: string) => void) => {
      setUserSpeaking(true);

      startListening(onPartial, (finalText) => {
        onFinal(finalText);
        setUserSpeaking(false);
      });
    },
    [],
  );

  const stopMic = useCallback(() => {
    stopListening();
    setUserSpeaking(false);
  }, []);

  return {
    aiSpeaking,
    userSpeaking,
    speak,
    startMic,
    stopMic,
  };
}
