import { SetupForm } from "@/components/setup/SetupForm";

export default function HomePage() {
  return (
    <section className="flex min-h-[calc(100vh-3rem)] items-center justify-center">
      <div className="w-full max-w-3xl">
        <SetupForm />
      </div>
    </section>
  );
}
