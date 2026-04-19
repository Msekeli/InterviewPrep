export type InterviewStage =
  | "asking"
  | "answering"
  | "submitting"
  | "completing";

export function getStageStatus(stage: InterviewStage) {
  switch (stage) {
    case "completing":
      return {
        label: "Finishing",
        variant: "warning" as const,
      };

    case "submitting":
      return {
        label: "Submitting",
        variant: "warning" as const,
      };

    case "asking":
      return {
        label: "AI turn",
        variant: "default" as const,
      };

    case "answering":
    default:
      return {
        label: "Your turn",
        variant: "success" as const,
      };
  }
}
