import { useMutation } from "@tanstack/react-query";
import { useToasts } from "./useToasts";
import {apiSendMessageToUser} from "../api/sendMessage/SendMessage.ts";
import {showErrorToast, showSuccessToast} from "../utils/swal.ts";

export const useSendMessage = (onDone: () => void) => {
    const { t, toastPosition, getErrorMessage } = useToasts();

    return useMutation({
        mutationFn: ({ userId, text }: { userId: number; text: string }) =>
            apiSendMessageToUser(userId, { text, disableWebPagePreview: true }),
        onSuccess: () => {
            showSuccessToast(t("usersPage.toasts.success"), toastPosition);
            onDone();
        },
        onError: (error: any) => {
            const msg = getErrorMessage(error, t("common.error"));
            showErrorToast(t("usersPage.toasts.errorTitle"), toastPosition, { text: msg });
        },
    });
};
