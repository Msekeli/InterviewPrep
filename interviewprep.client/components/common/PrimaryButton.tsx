import { ButtonHTMLAttributes, PropsWithChildren } from "react";

type PrimaryButtonProps = PropsWithChildren<
  ButtonHTMLAttributes<HTMLButtonElement>
>;

export default function PrimaryButton({
  children,
  className = "",
  type = "button",
  ...props
}: PrimaryButtonProps) {
  return (
    <button
      type={type}
      className={`btn btn-primary ${className}`.trim()}
      {...props}
    >
      {children}
    </button>
  );
}
