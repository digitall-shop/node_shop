export interface Transaction {
    id: number;
    amount: number;
    type: string;
    reason: string;
    description?: string | null;
    balanceBefore: number;
    balanceAfter: number;
    timestamp: string;
}

export interface GetTransactionsParams {
    DateFrom?: string;
    DateTo?: string;
    Type?: string;
    Reason?: string;
    FromAmount?: number;
    ToAmount?: number;
    OrderBy?: string;
    OrderDir?: string;
    Skip?: number;
    Take?: number;
}

export interface PagedResult<T> {
    page: number;
    take: number;
    totalCount: number;
    items: T[];
}

export interface ManualTransactionPayload {
    userId: number;
    amount: number;
    description: string;
}