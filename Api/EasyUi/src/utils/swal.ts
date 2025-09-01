import Swal, { type SweetAlertIcon } from "sweetalert2";

const THEME = {
    background: "#1f2937",
    color: "#f9fafb",
};

export type ToastPosition =
    | "top-end" | "top-start" | "top"
    | "bottom" | "bottom-start" | "bottom-end" | "center";

const BRAND_CLASSES = {
    popup: "bg-gray-800 border border-gray-700 rounded-xl",
    title: "text-gray-100 font-bold",
    htmlContainer: "text-gray-300",
    confirmButton:
        "bg-indigo-600 hover:bg-indigo-700 text-white  py-2 px-5 mr-2 rounded-lg focus:outline-none",
    cancelButton:
        "bg-gray-600 hover:bg-gray-700 text-white  py-2 px-5 rounded-lg focus:outline-none",
};

export const showSuccessToast = (
    title: string,
    position: ToastPosition = "top-end"
) =>
    Swal.fire({
        toast: true,
        position,
        icon: "success",
        title,
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        ...THEME,
    });

export const showErrorToast = (
    title: string,
    position: ToastPosition = "top-end",
    opts?: { text?: string; timer?: number }
) =>
    Swal.fire({
        toast: true,
        position,
        icon: "error",
        title,
        text: opts?.text,
        showConfirmButton: false,
        timer: opts?.timer ?? 4000,
        timerProgressBar: true,
        ...THEME,
    });

export const showErrorModal = (
    title: string,
    text?: string,
    opts?: { confirmText?: string; isRtl?: boolean }
) =>
    Swal.fire({
        icon: "error",
        title,
        text,
        confirmButtonText: opts?.confirmText ?? "OK",
        showConfirmButton: true,
        buttonsStyling: false,
        reverseButtons: !!opts?.isRtl,
        customClass: BRAND_CLASSES,
        ...THEME,
    });

export const showConfirm = (opts: {
    title: string;
    text?: string;
    confirmText: string;
    cancelText: string;
    isRtl?: boolean;
    icon?: SweetAlertIcon;
}) =>
    Swal.fire({
        title: opts.title,
        text: opts.text,
        icon: opts.icon ?? "warning",
        showCancelButton: true,
        confirmButtonText: opts.confirmText,
        cancelButtonText: opts.cancelText,
        reverseButtons: !!opts.isRtl,
        buttonsStyling: false,
        customClass: BRAND_CLASSES,
        ...THEME,
    });

export const extractErrorMessage = (err: unknown, fallback: string) =>
    (err as any)?.message ||
    (err as any)?.response?.data?.message ||
    (typeof err === "string" ? err : "") ||
    fallback;
