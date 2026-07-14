import * as SpeechSDK from "microsoft-cognitiveservices-speech-sdk";

function escapeSsml(text: string): string {
  return text
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");
}

export function speakText(text: string): Promise<void> {
  return new Promise((resolve, reject) => {
    const speechConfig = SpeechSDK.SpeechConfig.fromSubscription(
      process.env.NEXT_PUBLIC_AZURE_SPEECH_KEY!,
      process.env.NEXT_PUBLIC_AZURE_SPEECH_REGION!,
    );

    const audioConfig = SpeechSDK.AudioConfig.fromDefaultSpeakerOutput();
    const synthesizer = new SpeechSDK.SpeechSynthesizer(
      speechConfig,
      audioConfig,
    );

    // A "chat" style conversational delivery, a touch quieter than the
    // default neural reading — meant to sound like an interviewer talking
    // to you, not a narrator reading a script.
    const ssml = `
      <speak version="1.0" xmlns="http://www.w3.org/2001/10/synthesis" xmlns:mstts="https://www.w3.org/2001/mstts" xml:lang="en-US">
        <voice name="en-US-JennyNeural">
          <mstts:express-as style="chat" styledegree="1">
            <prosody volume="-15%" rate="-2%">${escapeSsml(text)}</prosody>
          </mstts:express-as>
        </voice>
      </speak>
    `.trim();

    synthesizer.speakSsmlAsync(
      ssml,
      () => {
        synthesizer.close();
        resolve();
      },
      (err) => {
        synthesizer.close();
        reject(err);
      },
    );
  });
}
