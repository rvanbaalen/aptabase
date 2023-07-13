import { api } from "../primitives/api";
import { SubscriptionPlan } from "./billing.types";

type Props = {
  plans: SubscriptionPlan[];
};

export function SubscriptionPlanSelector(props: Props) {
  const openCheckout = (plan: SubscriptionPlan) => {
    api
      .post("/_billing/checkout", { variantId: plan.variantId })
      .then(console.log);
  };

  return (
    <div className="space-y-1">
      {props.plans.map((plan) => (
        <button
          className="bg-subtle p-4 w-full border border-default flex justify-between cursor-pointer hover:bg-emphasis"
          key={plan.variantId}
          onClick={() => openCheckout(plan)}
        >
          <div>
            <span className="text-xl font-semibold">{plan.name}</span>
            <p className="text-sm">{plan.monthlyEvents}/events per month</p>
          </div>
          <div>
            <span className="font-semibold">
              <span className="text-2xl">${plan.monthlyPrice}</span>
              <span className="text-xs text-subtle"> /mo</span>
            </span>
          </div>
        </button>
      ))}
      <p className="text-center text-sm text-subtle">
        No contract. You can switch or cancel your plan at any time.
      </p>
      <script src="https://app.lemonsqueezy.com/js/lemon.js" defer></script>
    </div>
  );
}
