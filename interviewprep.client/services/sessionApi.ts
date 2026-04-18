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

export type QuestionDto = {
  id: string;
  category: number;
  text: string;
  order: number;
};

export type SubmitAnswerRequest = {
  interviewQuestionId: string;
  transcript: string;
};

export type AnswerDto = {
  id: string;
  interviewSessionId: string;
  interviewQuestionId: string;
  transcript: string;
  score?: number | null;
};

export type InterviewResultDto = {
  sessionId: string;
  overallScore: number;
  feedback: string;
  completedAtUtc: string;
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

export function generateQuestions(sessionId: string) {
  return apiFetch<QuestionDto[]>(`/api/sessions/${sessionId}/questions`, {
    method: "POST",
  });
}

export function getQuestions(sessionId: string) {
  return apiFetch<QuestionDto[]>(`/api/sessions/${sessionId}/questions`);
}

export function submitAnswer(sessionId: string, payload: SubmitAnswerRequest) {
  return apiFetch<AnswerDto>(`/api/sessions/${sessionId}/answers`, {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export function completeSession(sessionId: string) {
  return apiFetch<InterviewResultDto>(`/api/sessions/${sessionId}/complete`, {
    method: "POST",
  });
}
