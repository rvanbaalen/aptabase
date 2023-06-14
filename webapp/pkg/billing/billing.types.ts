export type SubscriptionPlan = {
  name: string;
  monthlyPrice: string;
  monthlyEvents: string;
  variantId: string;
};

export type BillingState = {
  current: SubscriptionPlan;
  manage_payment_url?: string;
  plans: SubscriptionPlan[];
};
