import type {BaseResponse} from "../../models/common/common.ts";
import api from "../common/axiosInstance.ts";
import type {
    GetTransactionsParams,
    ManualTransactionPayload,
    PagedResult,
    Transaction
} from "../../models/transactions/transactions.ts";
import {isSuperAdmin} from "../../models/auth.utils.ts";

export const getAllTransaction = async (
    filters: GetTransactionsParams = {}
): Promise<PagedResult<Transaction>> => {
    const {data} = await api.get<BaseResponse<PagedResult<Transaction>>>(
        "/transaction",
        {params: filters}
    );

    if (!data?.data) {
        throw new Error("Transactions response was null");
    }

    return data.data;
};

export const getAllTransactionUser = async (
    filters: GetTransactionsParams = {}
): Promise<PagedResult<Transaction>> => {
    const {data} = await api.get<BaseResponse<PagedResult<Transaction>>>(
        "/transaction/user",
        {params: filters}
    );

    if (!data?.data) {
        throw new Error("Transactions response was null");
    }

    return data.data;
};

export const getTransactions = async (
    filters: GetTransactionsParams = {}
): Promise<PagedResult<Transaction>> => {
    if (isSuperAdmin()) {
        return getAllTransaction(filters);
    }
    return getAllTransactionUser(filters);
};

export const manualCreditTransaction = async (
    payload: ManualTransactionPayload
) => {
    const { data } = await api.post<BaseResponse<Transaction>>(
        "/transaction/balance/credit",
        payload
    );
    if (!data?.data) throw new Error("Manual credit transaction response was null");
    return data.data;
};


export const manualDebitTransaction = async (
    payload: ManualTransactionPayload
) => {
    const { data } = await api.post<BaseResponse<Transaction>>(
        "/transaction/balance/debit",
        payload
    );
    if (!data?.data) throw new Error("Manual debit transaction response was null");
    return data.data;
};