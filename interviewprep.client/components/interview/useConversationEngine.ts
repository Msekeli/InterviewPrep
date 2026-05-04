import { useEffect, useState } from "react";

import {
  subscribe,
  ConversationState,
  aiSpeak,
  userStartSpeaking,
  userStopSpeaking,
} from "@/services/conversationEngine";

export function useConversationEngine() {
  const [state, setState] = useState<ConversationState>(() => ({
    turn: "idle",
    aiSpeaking: false,
    userSpeaking: false,
    userReady: false,
  }));

  useEffect(() => {
    const unsubscribe = subscribe(setState);
    return unsubscribe;
  }, []);

  return {
    ...state,

    // actions (direct service bridge)
    aiSpeak,
    userStartSpeaking,
    userStopSpeaking,
  };
}
