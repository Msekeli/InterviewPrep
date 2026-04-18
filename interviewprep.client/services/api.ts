const API_BASE_URL = "http://localhost:5276";

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

export async function createSession(
  payload: CreateSessionRequest,
): Promise<InterviewSessionDto> {
  const response = await fetch(`${API_BASE_URL}/api/sessions`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(payload),
  });

  if (!response.ok) {
    throw new Error("Failed to create session.");
  }

  return response.json();
}

export async function getSessionById(
  sessionId: string,
): Promise<InterviewSessionDto> {
  const response = await fetch(`${API_BASE_URL}/api/sessions/${sessionId}`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
  });

  if (!response.ok) {
    throw new Error("Failed to load session.");
  }

  return response.json();
}
