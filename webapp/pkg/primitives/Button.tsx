import clsx from "clsx";

type Props = {
  size?: "default" | "sm" | "xs";
  variant: "primary" | "secondary" | "danger";
  onClick?: () => void | Promise<void>;
  loading?: boolean;
  disabled?: boolean;
  children: React.ReactNode;
};

const Loading = () => {
  return (
    <div className="flex flex-row items-center justify-center">
      <svg
        className="animate-spin -ml-1 mr-3 h-5 w-5 text-white"
        xmlns="http://www.w3.org/2000/svg"
        fill="none"
        viewBox="0 0 24 24"
      >
        <circle
          className="opacity-25"
          cx="12"
          cy="12"
          r="10"
          stroke="currentColor"
          strokeWidth="4"
        ></circle>
        <path
          className="opacity-75"
          fill="currentColor"
          d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
        ></path>
      </svg>
      <span>Processing...</span>
    </div>
  );
};

const sizes = {
  xs: "px-1 py-0.5 text-xs",
  sm: "px-2 py-1 text-sm",
  default: "px-2 py-1.5 text-base",
};

const classNames = {
  primary: "btn bg-primary text-white hover:opacity-90",
  secondary: "btn bg-subtle text-default hover:opacity-90",
  danger: "btn btn-danger bg-error text-white hover:opacity-90",
};

export function Button(props: Props) {
  const className = classNames[props.variant];
  const sizeClassName = sizes[props.size ?? "default"];

  return (
    <button
      type="submit"
      disabled={props.loading || props.disabled}
      className={clsx(className, sizeClassName)}
      onClick={props.onClick}
    >
      {props.loading ? <Loading /> : props.children}
    </button>
  );
}
