// src/queries/bankAccount.ts
import {
    useQuery,
    useMutation,
    useQueryClient,
    type UseQueryOptions,
    type UseMutationOptions,
} from "@tanstack/react-query";

import type {
    BankAccount,
    CreateBankAccountDto,
    UpdateBankAccountDto,
} from "../models/bankAccounts/bankAccounts";
import {
    createBankAccountAdmin,
    deleteBankAccountAdmin,
    getAllBankAccountsAdmin,
    getBankAccountByIdAdmin,
    updateBankAccountAdmin,
} from "../api/bankAccounts/bankAccounts";
import {isSuperAdmin} from "../models/auth.utils";

export const bankAccountKeys = {
    all: ["bank-account", "admin"] as const,
    list: () => [...bankAccountKeys.all, "list"] as const,
    detail: (id: number) => [...bankAccountKeys.all, "detail", id] as const,
} as const;

export function useBankAccountsAdmin(
    options?: Omit<
        UseQueryOptions<BankAccount[], Error, BankAccount[], ReturnType<typeof bankAccountKeys.list>>,
        "queryKey" | "queryFn" | "enabled"
    >
) {
    return useQuery({
        queryKey: bankAccountKeys.list(),
        queryFn: getAllBankAccountsAdmin,
        staleTime: 30_000,
        enabled: isSuperAdmin(),
        ...options,
    });
}

export function useBankAccountAdmin(
    id: number | null | undefined,
    options?: Omit<
        UseQueryOptions<BankAccount, Error, BankAccount, ReturnType<typeof bankAccountKeys.detail>>,
        "queryKey" | "queryFn" | "enabled"
    >
) {
    const enabled = isSuperAdmin() && typeof id === "number";
    return useQuery({
        queryKey: bankAccountKeys.detail(id ?? -1),
        queryFn: () => getBankAccountByIdAdmin(id as number),
        staleTime: 30_000,
        enabled,
        ...options,
    });
}

export function useCreateBankAccountAdmin(
    options?: UseMutationOptions<BankAccount, Error, CreateBankAccountDto>
) {
    const qc = useQueryClient();
    return useMutation<BankAccount, Error, CreateBankAccountDto>({
        mutationFn: (payload) => createBankAccountAdmin(payload),
        onSuccess: (created, vars, ctx) => {
            qc.invalidateQueries({queryKey: bankAccountKeys.list()});
            if (created?.id) {
                qc.setQueryData(bankAccountKeys.detail(created.id), created);
            }
            options?.onSuccess?.(created, vars, ctx as any);
        },
        ...options,
    });
}

type UpdateVars = { id: number; data: UpdateBankAccountDto };
type UpdateCtx = { prevList?: BankAccount[]; prevDetail?: BankAccount };

export function useUpdateBankAccountAdmin(
    options?: UseMutationOptions<BankAccount, Error, UpdateVars, UpdateCtx>
) {
    const qc = useQueryClient();
    return useMutation<BankAccount, Error, UpdateVars, UpdateCtx>({
        mutationFn: ({id, data}) => updateBankAccountAdmin(id, data),

        // optimistic update
        onMutate: async (vars) => {
            await qc.cancelQueries({queryKey: bankAccountKeys.list()});

            const prevList = qc.getQueryData<BankAccount[]>(bankAccountKeys.list());
            const prevDetail = qc.getQueryData<BankAccount>(bankAccountKeys.detail(vars.id));

            if (prevList) {
                const nextList = prevList.map((a) => (a.id === vars.id ? ({...a, ...vars.data} as BankAccount) : a));
                qc.setQueryData(bankAccountKeys.list(), nextList);
            }
            if (prevDetail) {
                qc.setQueryData(bankAccountKeys.detail(vars.id), {...prevDetail, ...vars.data});
            }

            return {prevList, prevDetail};
        },

        onError: (err, vars, ctx) => {
            if (ctx?.prevList) qc.setQueryData(bankAccountKeys.list(), ctx.prevList);
            if (ctx?.prevDetail) qc.setQueryData(bankAccountKeys.detail(vars.id), ctx.prevDetail);
            options?.onError?.(err, vars, ctx);
        },

        onSuccess: (updated, vars, ctx) => {
            if (updated) {
                qc.setQueryData(bankAccountKeys.detail(updated.id), updated);
                const list = qc.getQueryData<BankAccount[]>(bankAccountKeys.list()) ?? [];
                qc.setQueryData(
                    bankAccountKeys.list(),
                    list.map((a) => (a.id === updated.id ? updated : a))
                );
            }
            options?.onSuccess?.(updated, vars, ctx);
        },

        // بک‌اند الان غیرفعال‌ها رو هم برمی‌گردونه → sync با سرور
        onSettled: () => {
            qc.invalidateQueries({queryKey: bankAccountKeys.list()});
        },
    });
}

export function useDeleteBankAccountAdmin(
    options?: UseMutationOptions<boolean, Error, number>
) {
    const qc = useQueryClient();
    return useMutation<boolean, Error, number>({
        mutationFn: (id) => deleteBankAccountAdmin(id),
        onSuccess: (_ok, id, ctx) => {
            qc.removeQueries({queryKey: bankAccountKeys.detail(id)});
            qc.invalidateQueries({queryKey: bankAccountKeys.list()});
            options?.onSuccess?.(_ok, id, ctx as any);
        },
        ...options,
    });
}
