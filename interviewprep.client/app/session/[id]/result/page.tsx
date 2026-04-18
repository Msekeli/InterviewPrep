type SessionResultPageProps = {
  params: {
    id: string;
  };
};

export default function SessionResultPage({ params }: SessionResultPageProps) {
  return (
    <section className="space-y-4">
      <h1 className="text-3xl font-semibold">Interview Result</h1>
      <p className="text-sm opacity-80">Session ID: {params.id}</p>
    </section>
  );
}
