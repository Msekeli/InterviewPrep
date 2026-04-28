export async function speakText(text: string): Promise<void> {
  if (typeof window === "undefined") return;

  // Lazy load SDK (important for Next.js)
  const sdk = await import("microsoft-cognitiveservices-speech-sdk");

  const speechConfig = sdk.SpeechConfig.fromSubscription(
    process.env.NEXT_PUBLIC_AZURE_SPEECH_KEY!,
    process.env.NEXT_PUBLIC_AZURE_SPEECH_REGION!,
  );

  speechConfig.speechSynthesisVoiceName = "en-US-JennyNeural";

  const audioConfig = sdk.AudioConfig.fromDefaultSpeakerOutput();

  const currentSynthesizer = new sdk.SpeechSynthesizer(
    speechConfig,
    audioConfig,
  );

  return new Promise((resolve, reject) => {
    currentSynthesizer.speakTextAsync(
      text,
      () => {
        currentSynthesizer.close();
        resolve();
      },
      (error: string) => {
        currentSynthesizer.close();
        reject(error);
      },
    );
  });
}
