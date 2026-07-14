import type * as SpeechSDK from "microsoft-cognitiveservices-speech-sdk";

let recognizer: SpeechSDK.SpeechRecognizer | null = null;
let micStream: MediaStream | null = null;

export async function startListening(
  onRecognizing: (text: string) => void,
  onFinal: (text: string) => void,
) {
  // Never let two recognizers run at once — stop whatever's already active
  // first, otherwise the old one is orphaned and keeps listening forever.
  if (recognizer) {
    await stopListeningAsync();
  }

  const sdk = await import("microsoft-cognitiveservices-speech-sdk");

  const speechConfig = sdk.SpeechConfig.fromSubscription(
    process.env.NEXT_PUBLIC_AZURE_SPEECH_KEY!,
    process.env.NEXT_PUBLIC_AZURE_SPEECH_REGION!,
  );

  speechConfig.speechRecognitionLanguage = "en-US";

  // Acquire the microphone stream ourselves instead of letting
  // AudioConfig.fromDefaultMicrophoneInput() manage it internally, so we can
  // force-stop the hardware track directly on cleanup rather than trusting
  // the SDK to fully release it.
  micStream = await navigator.mediaDevices.getUserMedia({ audio: true });
  const audioConfig = sdk.AudioConfig.fromStreamInput(micStream);

  recognizer = new sdk.SpeechRecognizer(speechConfig, audioConfig);

  recognizer.recognizing = (_sender, event) => {
    if (event.result.text) {
      onRecognizing(event.result.text); // live typing
    }
  };

  recognizer.recognized = (_sender, event) => {
    if (event.result.text) {
      onFinal(event.result.text); // one finalized phrase — not "done speaking"
    }
  };

  recognizer.startContinuousRecognitionAsync();
}

export function stopListening() {
  void stopListeningAsync();
}

function stopListeningAsync(): Promise<void> {
  return new Promise((resolve) => {
    const currentRecognizer = recognizer;
    const currentStream = micStream;
    recognizer = null;
    micStream = null;

    const releaseStream = () => {
      currentStream?.getTracks().forEach((track) => track.stop());
    };

    if (!currentRecognizer) {
      releaseStream();
      resolve();
      return;
    }

    currentRecognizer.stopContinuousRecognitionAsync(
      () => {
        currentRecognizer.close();
        releaseStream();
        resolve();
      },
      () => {
        // Even if the SDK reports an error stopping, force the hardware
        // mic off — a stuck "listening" indicator is worse than a
        // swallowed error here.
        currentRecognizer.close();
        releaseStream();
        resolve();
      },
    );
  });
}
