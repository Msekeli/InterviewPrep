type Turn = "ai" | "user" | "idle";

export type ConversationState = {
  turn: Turn;
  aiSpeaking: boolean;
  userSpeaking: boolean;
  userReady: boolean;
};

type Listener = (state: ConversationState) => void;

let state: ConversationState = {
  turn: "idle",
  aiSpeaking: false,
  userSpeaking: false,
  userReady: false,
};

const listeners = new Set<Listener>();

function emit() {
  for (const l of listeners) l(state);
}

export function subscribe(listener: Listener) {
  listeners.add(listener);
  listener(state);
  return () => {
    listeners.delete(listener);
  };
}

/* ---------------- CORE STATE UPDATE ---------------- */

function setState(partial: Partial<ConversationState>) {
  state = { ...state, ...partial };
  emit();
}

/* ---------------- AI FLOW ---------------- */

function delay(ms: number) {
  return new Promise((r) => setTimeout(r, ms));
}

export async function aiSpeak(speakFn: () => Promise<void>) {
  setState({
    turn: "ai",
    aiSpeaking: false,
    userSpeaking: false,
  });

  await delay(700);

  setState({ aiSpeaking: true });

  await speakFn();

  setState({ aiSpeaking: false });

  await delay(500);

  setState({ turn: "user" });
}

/* ---------------- USER FLOW ---------------- */

export function userStartSpeaking() {
  setState({
    userSpeaking: true,
    aiSpeaking: false,
  });
}

export function userStopSpeaking() {
  setState({
    userSpeaking: false,
    userReady: true,
  });

  setTimeout(() => {
    setState({ userReady: true });
  }, 300);
}
