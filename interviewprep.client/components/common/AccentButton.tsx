import { ButtonHTMLAttributes, PropsWithChildren } from "react";

type AccentButtonProps = PropsWithChildren<
  ButtonHTMLAttributes<HTMLButtonElement>
>;

export default function AccentButton({
  children,
  className = "",
  type = "button",
  ...props
}: AccentButtonProps) {
  return (
    <button
      type={type}
      className={`btn btn-accent ${className}`.trim()}
      {...props}
    >
      {children}
    </button>
  );
}
