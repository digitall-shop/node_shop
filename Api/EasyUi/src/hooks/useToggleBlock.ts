import {useMutation, useQueryClient} from "@tanstack/react-query";
import {useState} from "react";
import {useToasts} from "./useToasts";
import {apiBlockUser, apiUnblockUser} from "../api/user/user.ts";
import {showErrorToast, showSuccessToast} from "../utils/swal.ts";

type ToggleVars = { userId: number; block: boolean; username?: string };

export const useToggleBlock = () => {
    const {t, toastPosition, getErrorMessage} = useToasts();
    const queryClient = useQueryClient();
    const [busyUserId, setBusyUserId] = useState<number | null>(null);

    const {mutate, isPending} = useMutation<unknown, unknown, ToggleVars>({
        mutationFn: ({userId, block}) =>
            block ? apiBlockUser(userId) : apiUnblockUser(userId),
        onMutate: ({userId}) => setBusyUserId(userId),
        onSuccess: (_data, vars) => {
            const key = vars.block
                ? "usersPage.toasts.successBlock"
                : "usersPage.toasts.successUnBlock";
            showSuccessToast(t(key, { username: vars.username }), toastPosition);
            queryClient.invalidateQueries({queryKey: ["users"]});
        },
        onError: (error: any) => {
            const msg = getErrorMessage(error, t("common.error"));
            showErrorToast(t("usersPage.toasts.errorTitle"), toastPosition, {text: msg});
        },
        onSettled: () => setBusyUserId(null),
    });

    return {toggleBlock: mutate, isToggling: isPending, busyUserId};
};
