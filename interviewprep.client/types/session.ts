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
