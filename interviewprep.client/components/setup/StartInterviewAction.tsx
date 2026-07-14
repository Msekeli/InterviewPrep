import AccentButton from "../common/AccentButton";

type StartInterviewActionProps = {
  isSubmitting?: boolean;
  disabled?: boolean;
  label?: string;
  loadingLabel?: string;
  className?: string;
  onClick?: () => void;
};

export default function StartInterviewAction({
  isSubmitting = false,
  disabled = false,
  label = "Start interview",
  loadingLabel = "Starting...",
  className = "",
  onClick,
}: StartInterviewActionProps) {
  return (
    <div className={className}>
      <AccentButton
        type="button"
        onClick={onClick}
        disabled={disabled || isSubmitting}
        className="w-full sm:w-auto disabled:cursor-not-allowed disabled:opacity-60"
      >
        {isSubmitting ? loadingLabel : label}
      </AccentButton>
    </div>
  );
}
