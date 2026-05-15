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

  observation: string;
  strengths: string;
  communication: string;
  growthOpportunity: string;
  overallImpression: string;
  nextFocus: string;
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

  observation: string;
  strengths: string;
  communication: string;
  growthOpportunity: string;
  overallImpression: string;
  nextFocus: string;

  completedAtUtc: string;
};
