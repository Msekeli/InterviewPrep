type SessionPageProps = {
  params: {
    id: string;
  };
};

export default function SessionPage({ params }: SessionPageProps) {
  return (
    <section className="space-y-4">
      <h1 className="text-3xl font-semibold">Session</h1>
      <p className="text-sm opacity-80">Session ID: {params.id}</p>
    </section>
  );
}
