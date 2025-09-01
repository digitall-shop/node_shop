import {useMutation, useQueryClient} from "@tanstack/react-query";
import {useToasts} from "./useToasts";
import type {ManualTransactionPayload} from "../models/transactions/transactions";
import {manualCreditTransaction, manualDebitTransaction} from "../api/transactions/transactions";
import {showErrorToast, showSuccessToast} from "../utils/swal.ts";

export const useUpdateBalance = (onDone: () => void) => {
    const {t, toastPosition, getErrorMessage} = useToasts();
    const qc = useQueryClient();

    return useMutation({
        mutationFn: (payload: ManualTransactionPayload & { type: "credit" | "debit" }) => {
            const {type, ...rest} = payload;
            return type === "credit" ? manualCreditTransaction(rest) : manualDebitTransaction(rest);
        },
        onSuccess: () => {
            showSuccessToast(t("usersPage.toasts.success"), toastPosition);
            qc.invalidateQueries({queryKey: ["users"]});
            onDone();
        },
        onError: (error: any) => {
            const msg = getErrorMessage(error, t("common.error"));
            showErrorToast(t("usersPage.toasts.errorTitle"), toastPosition, {text: msg});
        },
    });
};
