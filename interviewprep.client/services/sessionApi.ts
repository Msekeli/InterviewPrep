import { apiFetch } from "./apiClient";

export type CreateSessionRequest = {
  cvText: string;
  jobSpecText: string;
  companyText: string;
  targetLevel?: number;
};

export type InterviewSessionDto = {
  id: string;
  cvText: string;
  jobSpecText: string;
  companyText: string;
  targetLevel: number;
  status: number;
  createdAtUtc: string;
  completedAtUtc?: string | null;
  overallScore?: number | null;
  feedback?: string | null;
};

export function createSession(payload: CreateSessionRequest) {
  return apiFetch<InterviewSessionDto>("/api/sessions", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export function getSessionById(sessionId: string) {
  return apiFetch<InterviewSessionDto>(`/api/sessions/${sessionId}`);
}

export type QuestionDto = {
  id: string;
  category: number;
  text: string;
  order: number;
};

export async function generateQuestions(
  sessionId: string,
): Promise<QuestionDto[]> {
  return apiFetch<QuestionDto[]>(`/api/sessions/${sessionId}/questions`, {
    method: "POST",
  });
}

export async function getQuestions(sessionId: string): Promise<QuestionDto[]> {
  return apiFetch<QuestionDto[]>(`/api/sessions/${sessionId}/questions`);
}
export type SubmitAnswerRequest = {
  questionId: string;
  transcript: string;
};

export type AnswerDto = {
  id: string;
  interviewQuestionId: string;
  transcript: string;
  score?: number | null;
};

export async function submitAnswer(
  sessionId: string,
  payload: SubmitAnswerRequest,
): Promise<AnswerDto> {
  return apiFetch<AnswerDto>(`/api/sessions/${sessionId}/answers`, {
    method: "POST",
    body: JSON.stringify(payload),
  });
}
