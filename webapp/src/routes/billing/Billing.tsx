import { Head, PageHeading, api } from "@app/primitives";
import { ManageBillingView } from "@app/billing";

Component.displayName = "Billing";
export function Component() {
  return (
    <>
      <Head title="Billing" />
      <PageHeading title="Billing" subtitle="Manage your subscription" />

      <ManageBillingView />
    </>
  );
}
