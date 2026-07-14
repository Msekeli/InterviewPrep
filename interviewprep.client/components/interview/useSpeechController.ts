import { useState, useCallback, useEffect } from "react";
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

      // Continuous recognition fires `recognized` after every natural pause,
      // not just when the user is actually done — so don't flip userSpeaking
      // off here. It only turns off when the user explicitly calls stopMic.
      startListening(onPartial, onFinal);
    },
    [],
  );

  const stopMic = useCallback(() => {
    stopListening();
    setUserSpeaking(false);
  }, []);

  // Hard guarantee: if the user navigates away (e.g. the back button) while
  // the mic is on, without ever clicking "Stop Speaking", force it off
  // instead of leaving it running with no UI left to stop it from.
  useEffect(() => {
    return () => {
      stopListening();
    };
  }, []);

  return {
    aiSpeaking,
    userSpeaking,
    speak,
    startMic,
    stopMic,
  };
}
