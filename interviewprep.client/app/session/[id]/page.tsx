import { InterviewRoom } from "@/components/interview/InterviewRoom";

type SessionPageProps = {
  params: {
    id: string;
  };
};

export default function SessionPage({ params }: SessionPageProps) {
  return <InterviewRoom sessionId={params.id} />;
}
