export const PaymentRequestStatus = {
    Pending: 0,
    Submitted: 1,
    Completed: 2,
    Failed: 3,
} as const;


export const statusLabel = (s: number): string => {
    switch (s) {
        case PaymentRequestStatus.Pending:
            return "paymentStatus.pending";
        case PaymentRequestStatus.Submitted:
            return "paymentStatus.submitted";
        case PaymentRequestStatus.Completed:
            return "paymentStatus.completed";
        case PaymentRequestStatus.Failed:
            return "paymentStatus.failed";
        default:
            return "paymentStatus.unknown";
    }
};

export const isActionable = (s: number) =>
    s === PaymentRequestStatus.Pending || s === PaymentRequestStatus.Submitted;