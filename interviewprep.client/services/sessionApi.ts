import { apiFetch } from "./apiClient";
import type {
  AnswerDto,
  CreateSessionRequest,
  InterviewResultDto,
  InterviewSessionDto,
  QuestionDto,
  SubmitAnswerRequest,
} from "../types/session";

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
