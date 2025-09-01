export interface NewPaymentRequest {
    amount: number;
    method: number;
}


export interface PaymentDisplayData {
    bankName: string;
    cardNumber: string;
    holderName: string;
}

export interface NewPaymentResponseData {
    paymentRequestId: number;
    isSuccess: boolean;
    requiresRedirect: boolean;
    redirectUrl: string | null;
    displayData: PaymentDisplayData | null;
    errorMessage: string | null;
}

export interface AcceptPaymentPayload {
    paymentRequestId: number;
    receipt: File;
}

export interface AcceptPaymentResponse {
    success: boolean;
    message: string;
}

export interface PaymentRequest {
    id: number | null;
    userId: number | null;
    amount: number;
    method: number;
    status: number;
    createDate: string | null;
    gatewayTransactionId: string | null;
    receiptImage: string | null;
    bankAccountId: number | null;
}

export interface GetAllPaymentsResponse {
    data: PaymentRequest[];
    isSuccess: boolean;
    statusCode: number;
    jsonValidationMessage: string | null;
    message: string;
}

export interface RejectPaymentPayload {
    description: string;
}

export interface PagedResult<T> {
    page: number;
    take: number;
    totalCount: number;
    items: T[];
}

export interface GetPaymentRequestsParams {
    DateFrom?: string;
    DateTo?: string;
    Method?: number;
    Status?: number;
    FromAmount?: number;
    ToAmount?: number;
    OrderBy?: string;
    OrderDir?: "asc" | "desc";
    Skip?: number;
    Take?: number;
}