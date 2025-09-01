import { useEffect, useMemo, useState } from "react";
import { useQuery, keepPreviousData } from "@tanstack/react-query";
import { getAllUserPaymentRequests } from "../api/peyment/payment";
import type {
    PagedResult,
    PaymentRequest as AppPaymentRequest,
} from "../models/peyment/payment";

export type OrderDir = "asc" | "desc";
export type GetPaymentsParams = {
    DateFrom?: string; DateTo?: string;
    Method?: string; Status?: string;
    FromAmount?: string; ToAmount?: string;
    OrderBy?: string; OrderDir?: OrderDir;
    Skip?: number; Take?: number;
};

const useDebounced = <T,>(value: T, delay = 300) => {
    const [v, setV] = useState(value);
    useEffect(() => { const id = setTimeout(() => setV(value), delay); return () => clearTimeout(id); }, [value, delay]);
    return v;
};

export const ITEMS_PER_PAGE = 12;

export function useMyPayments() {
    const [page, setPage] = useState(1);
    const [filters, setFilters] = useState<Partial<GetPaymentsParams>>({
        OrderBy: "id",
        OrderDir: "desc",
    });

    const rawQueryFilters: GetPaymentsParams = useMemo(() => ({
        ...filters,
        Skip: (page - 1) * ITEMS_PER_PAGE,
        Take: ITEMS_PER_PAGE,
    }), [filters, page]);

    const queryFilters = useDebounced(rawQueryFilters, 300);
    const key = ["user-payment-requests", queryFilters] as const;

    const query = useQuery<
        PagedResult<AppPaymentRequest>, // TQueryFnData
        Error,                          // TError
        PagedResult<AppPaymentRequest>, // TData
        typeof key                      // TQueryKey
    >({
        queryKey: key,
        queryFn: () =>
            getAllUserPaymentRequests(queryFilters as any) as Promise<PagedResult<AppPaymentRequest>>,
        staleTime: 30_000,
        refetchOnWindowFocus: false,
        placeholderData: keepPreviousData,   // ⬅️ جایگزین v5
    });

    const totalCount: number = query.data?.totalCount ?? 0;
    const totalPages: number = Math.ceil(totalCount / ITEMS_PER_PAGE);
    const list: AppPaymentRequest[] = query.data?.items ?? [];

    const handleFilterChange = (field: keyof GetPaymentsParams, value?: string) => {
        setFilters(prev => ({ ...prev, [field]: value || undefined }));
        setPage(1);
    };

    const resetFilters = () => {
        setFilters({ OrderBy: "id", OrderDir: "desc" });
        setPage(1);
    };

    return {
        page, setPage,
        filters, setFilters, handleFilterChange, resetFilters,
        totalPages, totalCount, list,
        query,
    };
}
