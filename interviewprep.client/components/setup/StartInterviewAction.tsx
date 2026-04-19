import PrimaryButton from "../common/PrimaryButton";

type StartInterviewActionProps = {
  isSubmitting?: boolean;
  disabled?: boolean;
  label?: string;
  loadingLabel?: string;
  className?: string;
};

export default function StartInterviewAction({
  isSubmitting = false,
  disabled = false,
  label = "Start Interview",
  loadingLabel = "Starting...",
  className = "",
}: StartInterviewActionProps) {
  return (
    <div className={className}>
      <PrimaryButton
        type="submit"
        disabled={disabled || isSubmitting}
        className="w-full sm:w-auto disabled:cursor-not-allowed disabled:opacity-60"
      >
        {isSubmitting ? loadingLabel : label}
      </PrimaryButton>
    </div>
  );
}
