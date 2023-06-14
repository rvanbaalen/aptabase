import { Button, ErrorState, LoadingState, api } from "@app/primitives";
import { useQuery } from "@tanstack/react-query";
import { BillingState } from "./billing.types";
import { SubscriptionPlanSelector } from "./SubscriptionPlanSelector";

export function ManageBillingView() {
  const { isLoading, isError, data } = useQuery(["billing-state"], () =>
    api.get<BillingState>(`/_billing`)
  );

  if (isLoading) return <LoadingState />;
  if (isError) return <ErrorState />;

  return (
    <div className="mt-8 max-w-xl">
      <div className="bg-subtle p-4 border border-default flex justify-between">
        <div>
          <span className="text-xl font-semibold">{data.current.name}</span>
          <p className="text-sm">
            {data.current.monthlyEvents}/events per month
          </p>
        </div>
        <div className="flex items-center gap-2">
          <Button variant="primary" size="sm">
            Change Plan
          </Button>
        </div>
      </div>
      {data.manage_payment_url && (
        <Button variant="danger" size="sm">
          Cancel
        </Button>
      )}
      <SubscriptionPlanSelector plans={data.plans} />
      {data.manage_payment_url && (
        <a href={data.manage_payment_url}>Manage Payment Details</a>
      )}
    </div>
  );
}
