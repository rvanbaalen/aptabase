import { ErrorState, LoadingState, api } from "@app/primitives";
import { useQuery } from "@tanstack/react-query";
import { Link } from "react-router-dom";

type CurrentUsageItem = {
  count: number;
  quota: number;
};

export function CurrentUsage() {
  const { isLoading, isError, data } = useQuery(["billing-usage"], () =>
    api.get<CurrentUsageItem>(`/_billing/usage`)
  );

  if (isLoading) return <LoadingState />;
  if (isError) return <ErrorState />;

  const perc = (data.count / data.quota) * 100;

  return (
    <div className="p-2 flex flex-col space-y-1">
      <div className="text-sm font-medium mb-1 flex justify-between items-center">
        <div>Current Usage</div>
        <Link to="/billing" className="hover:text-primary-500 underline">
          Billing
        </Link>
      </div>
      <div className="text-sm text-subtle">
        {data.count?.toLocaleString()} / {data.quota?.toLocaleString()} events (
        {perc.toPrecision(2)}%)
      </div>
      <div className="overflow-hidden rounded bg-subtle">
        <div className="h-2 rounded bg-primary" style={{ width: `${perc}%` }} />
      </div>
    </div>
  );
}
