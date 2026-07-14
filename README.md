# InterviewPrep

InterviewPrep is a mock interview platform: paste in a CV, a job description, and (optionally) company context, and it runs a structured, voice-driven interview — then gives you a written reflection on how you did.

Built as a **.NET 9 Clean Architecture** backend with a **Next.js 14** frontend, running fully locally against **SQLite**.

> AI tools were used throughout development — see [AI_USAGE.md](AI_USAGE.md) for what was used and how.

## Status

This is an actively developed personal project, currently running locally (not deployed). A few things are deliberately unfinished:

- **Question generation and scoring are mocked**, not backed by a live AI provider. The app is structured behind `IQuestionService` / `IInterviewEvaluatorService` interfaces so a real LLM integration (Gemini, Claude, OpenAI, etc.) can be dropped in later without touching the rest of the app — see [Swapping in a real AI provider](#swapping-in-a-real-ai-provider).
- **No authentication yet.** Every session is currently attributed to a single hardcoded `TemporaryUserId`. Real auth is planned as a separate service shared across other apps, not something bolted onto this one.
- **No hosting.** It previously ran on Azure App Service; that was torn down to stop burning free-tier credits. Run it locally per the instructions below.

## What it does

1. **Setup** — a 3-step wizard collects your CV, the job spec, and (optionally) company/culture notes.
2. **Interview** — the app generates a set of tailored questions (CV, technical, behavioural, culture-fit) and conducts the interview turn-by-turn: it speaks each question aloud (text-to-speech), listens to your spoken answer (speech-to-text), and transcribes it live.
3. **Reflection** — once all questions are answered, it produces structured feedback: an overall observation, strengths, communication notes, a growth opportunity, an overall impression, and one concrete thing to focus on next.

## Architecture

**Backend** — `.NET 9`, Clean Architecture across four projects:

| Project | Responsibility |
|---|---|
| `InterviewPrep.Domain` | Entities (`InterviewSession`, `InterviewQuestion`, `InterviewAnswer`) and enums (`InterviewLevel`, `QuestionCategory`, `InterviewSessionStatus`) |
| `InterviewPrep.Application` | Use-case handlers (one class per operation, e.g. `CreateSessionHandler`, `SubmitAnswerHandler`), DTOs, and interfaces for external dependencies |
| `InterviewPrep.Infrastructure` | EF Core + SQLite persistence, and the mock question/evaluator services |
| `InterviewPrep.Api` | ASP.NET Core controllers, CORS, exception-handling middleware, DI wiring |

`InterviewPrep.Tests` covers the Domain and Application layers with xUnit + Moq (29 tests).

**Frontend** — `Next.js 14` (App Router) + TypeScript + Tailwind CSS:

- Custom design system ("Daybreak"): a calm indigo for the candidate's own presence, a warm sunrise/amber gradient for the AI interviewer, light and dark themes via `next-themes`.
- Geist for body text, a serif display font for questions and feedback headlines.
- Azure Cognitive Services Speech SDK for speech-to-text and text-to-speech (this is the one piece that does call a live Azure service — it's the interview I/O, not the AI reasoning).
- No page in the setup or interview flow scrolls — the setup wizard, interview room, and question flow are all built to fit the viewport. The Results/Reflection page is the one deliberate exception, since AI feedback is variable-length prose.

## Core flow

```
POST   /api/sessions                        create a session (CV + job spec + company text)
GET    /api/sessions                        list sessions
GET    /api/sessions/{id}                   get one session
POST   /api/sessions/{id}/questions          generate questions for a session
GET    /api/sessions/{id}/questions          list a session's questions
POST   /api/sessions/{id}/answers            submit an answer (transcript)
GET    /api/sessions/{id}/answers            list a session's answers
POST   /api/sessions/{id}/complete           evaluate the session and produce feedback
```

## Tech stack

- **Backend**: .NET 9, ASP.NET Core Web API, EF Core, SQLite, xUnit + Moq
- **Frontend**: Next.js 14 (App Router), TypeScript, Tailwind CSS, `next-themes`, `lucide-react`
- **Voice**: `microsoft-cognitiveservices-speech-sdk` (Azure Speech, STT + TTS)

## Running it locally

Requires the .NET 9 SDK and Node.js.

**Backend** (from the repo root):
```bash
dotnet run --project InterviewPrep.Api
```
Swagger UI is available at `/swagger` in development. SQLite migrations run automatically on startup.

**Frontend**:
```bash
cd interviewprep.client
npm install
npm run dev
```
The client is hardcoded to call the API at `http://localhost:5276` (`interviewprep.client/services/apiClient.ts`), and the API's CORS policy only allows `http://localhost:3000`, so run the backend with the `http` launch profile and the frontend on its default port 3000.

If you want real speech input/output, set Azure Speech credentials as environment variables (check `InterviewPrep.Api/appsettings.Development.json` for the expected keys) — without them, the interview flow still works but voice I/O won't.

## Swapping in a real AI provider

Question generation and evaluation are isolated behind two interfaces in `InterviewPrep.Application/Interfaces`:

- `IQuestionService` — currently implemented by `MockQuestionService`
- `IInterviewEvaluatorService` — currently implemented by `MockInterviewEvaluatorService`

To go live with a real model:

1. Implement `IQuestionService` and `IInterviewEvaluatorService` against your chosen provider — this is the real work: calling the API, prompting it to produce questions/feedback in the expected shape, and mapping the response onto `InterviewQuestion` / `EvaluationResultDto`.
2. Register an `HttpClient` (or the provider's SDK client) and its API key/config in `InterviewPrep.Api/Program.cs` — these were removed when the app moved to mock-only, so they'd need to be added back.
3. Swap the DI registration from the mocks to the new implementations:

```csharp
builder.Services.AddScoped<IQuestionService, MockQuestionService>();
builder.Services.AddScoped<IInterviewEvaluatorService, MockInterviewEvaluatorService>();
```

Controllers, handlers, and the frontend call these services only through the interfaces, so none of them need to change — the work is confined to the new implementation plus its DI/config wiring.
