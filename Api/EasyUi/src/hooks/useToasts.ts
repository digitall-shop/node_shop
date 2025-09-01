import {useTranslation} from "react-i18next";
import type {ToastPosition} from "../utils/swal.ts";


export const useToasts = () => {
    const {t, i18n} = useTranslation();
    const isRtl =
        typeof i18n?.dir === "function"
            ? i18n.dir(i18n.language) === "rtl"
            : (typeof document !== "undefined" ? document?.dir === "rtl" : false);

    const toastPosition : ToastPosition = isRtl ? "top-start" : "top-end";
    const getErrorMessage = (err: unknown, fallback: string) =>
        (err as any)?.message || (err as any)?.response?.data?.message || fallback;

    return {t, toastPosition, getErrorMessage};
};
