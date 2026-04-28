import type * as SpeechSDK from "microsoft-cognitiveservices-speech-sdk";

let recognizer: SpeechSDK.SpeechRecognizer | null = null;

export async function startListening(
  onRecognizing: (text: string) => void,
  onFinal: (text: string) => void,
) {
  const sdk = await import("microsoft-cognitiveservices-speech-sdk");

  const speechConfig = sdk.SpeechConfig.fromSubscription(
    process.env.NEXT_PUBLIC_AZURE_SPEECH_KEY!,
    process.env.NEXT_PUBLIC_AZURE_SPEECH_REGION!,
  );

  speechConfig.speechRecognitionLanguage = "en-US";

  const audioConfig = sdk.AudioConfig.fromDefaultMicrophoneInput();

  recognizer = new sdk.SpeechRecognizer(speechConfig, audioConfig);

  recognizer.recognizing = (_sender, event) => {
    if (event.result.text) {
      onRecognizing(event.result.text); // live typing
    }
  };

  recognizer.recognized = (_sender, event) => {
    if (event.result.text) {
      onFinal(event.result.text); // final sentence
    }
  };

  recognizer.startContinuousRecognitionAsync();
}

export function stopListening() {
  const currentRecognizer = recognizer;

  if (currentRecognizer) {
    currentRecognizer.stopContinuousRecognitionAsync(() => {
      currentRecognizer.close();
      recognizer = null;
    });
  }
}
