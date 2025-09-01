import {useMemo} from "react";
import {useBankAccountsAdmin} from "./useBankAccount.ts";

export function useHolderMap() {
    const {data: bankAccounts} = useBankAccountsAdmin?.() ?? {data: undefined};
    return useMemo(() => {
        const m: Record<number, string | null> = {};
        (bankAccounts ?? []).forEach((acc: any) => {
            if (acc) m[Number(acc.id)] = acc.holderName ?? null;
        });
        return m;
    }, [bankAccounts]);
}
