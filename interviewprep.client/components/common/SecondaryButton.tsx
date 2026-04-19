import { ButtonHTMLAttributes, PropsWithChildren } from "react";

type SecondaryButtonProps = PropsWithChildren<
  ButtonHTMLAttributes<HTMLButtonElement>
>;

export default function SecondaryButton({
  children,
  className = "",
  type = "button",
  ...props
}: SecondaryButtonProps) {
  return (
    <button
      type={type}
      className={`btn btn-secondary ${className}`.trim()}
      {...props}
    >
      {children}
    </button>
  );
}
