import api from "../common/axiosInstance";
import type { BaseResponse } from "../../models/common/common";
import {isSuperAdmin} from "../../models/auth.utils.ts";
import type {BankAccount, CreateBankAccountDto, UpdateBankAccountDto} from "../../models/bankAccounts/bankAccounts.ts";


const assertSuperAdmin = () => {
    if (!isSuperAdmin()) {
        throw new Error("Forbidden: این عملیات فقط برای سوپرادمین مجاز است.");
    }
};

export const getAllBankAccountsAdmin = async (): Promise<BankAccount[]> => {
    assertSuperAdmin();
    const { data } = await api.get<BaseResponse<BankAccount[]>>("/bankAccount");
    return data?.data ?? [];
};

export const getBankAccountByIdAdmin = async (id: number): Promise<BankAccount> => {
    assertSuperAdmin();
    const { data } = await api.get<BaseResponse<BankAccount>>(`/bankAccount/${id}`);
    if (!data?.data) throw new Error("حساب بانکی یافت نشد.");
    return data.data;
};

export const createBankAccountAdmin = async (
    payload: CreateBankAccountDto
): Promise<BankAccount> => {
    assertSuperAdmin();
    const { data } = await api.post<BaseResponse<BankAccount>>("/bankAccount", payload, {
        headers: { "Content-Type": "application/json" },
    });
    if (!data?.data) throw new Error("ایجاد حساب بانکی ناموفق بود.");
    return data.data;
};

export const updateBankAccountAdmin = async (
    id: number,
    payload: UpdateBankAccountDto
): Promise<BankAccount> => {
    assertSuperAdmin();
    const { data } = await api.patch<BaseResponse<BankAccount>>(
        `/bankAccount/${id}`,
        payload,
        { headers: { "Content-Type": "application/json" } }
    );
    if (!data?.data) throw new Error("ویرایش حساب بانکی ناموفق بود.");
    return data.data;
};

export const deleteBankAccountAdmin = async (id: number): Promise<boolean> => {
    assertSuperAdmin();
    const { data } = await api.delete<BaseResponse<unknown>>(`/bankAccount/${id}`);
    return (data as any)?.isSuccess ?? true;
};
