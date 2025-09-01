import React from "react";
import {useMutation, useQuery, useQueryClient} from "@tanstack/react-query";
import {useTranslation} from "react-i18next";
import type {PagedResult, PaymentRequest} from "../../models/peyment/payment.ts";
import {isActionable} from "../../models/paymentStatus.ts";
import {
    approvePaymentRequest,
    getAllPaymentRequestsForAdmin,
    rejectPaymentRequest,
} from "../../api/peyment/payment.ts";
import {isSuperAdmin} from "../../models/auth.utils.ts";
import {buildReceiptUrl} from "../../models/receiptUrl.ts";

const PaymentMethod = {CardToCard: 1, Plisio: 2} as const;
const hasReceipt = (p: any) => Boolean((p as any)?.receiptImage || (p as any)?.receiptImageUrl);
const isPlisio = (p: any) => Number(p?.method) === PaymentMethod.Plisio;


const WaitingForPayment: React.FC = () => {
    const {t} = useTranslation();
    const superAdmin = isSuperAdmin();
    const queryClient = useQueryClient();

    const [busyId, setBusyId] = React.useState<number | null>(null);
    const [removed, setRemoved] = React.useState<Set<number>>(new Set());
    const [toast, setToast] = React.useState("");
    const [rejectModal, setRejectModal] = React.useState<{ open: boolean; id: number | null }>({
        open: false,
        id: null,
    });
    const [receiptModal, setReceiptModal] = React.useState<{ open: boolean; url: string | null }>({
        open: false,
        url: null,
    });

    const {data, isLoading, isError} = useQuery({
        queryKey: ["admin-payment-requests", "pending-only"],
        queryFn: () => getAllPaymentRequestsForAdmin(),
        enabled: superAdmin,
        staleTime: 30_000,
    });

    const pending: PaymentRequest[] =
        ((data as PagedResult<PaymentRequest> | undefined)?.items ?? [])
            .filter((p) => isActionable(Number(p.status)))
            .filter((p) => !removed.has(p.id as number));

    const plisio = pending.filter(isPlisio);
    const c2cWithReceipt = pending.filter((p) => !isPlisio(p) && hasReceipt(p));
    const c2cNoReceipt = pending.filter((p) => !isPlisio(p) && !hasReceipt(p));

    const approveMut = useMutation({
        mutationFn: (id: number) => approvePaymentRequest(id),
        onMutate: (id) => setBusyId(id),
        onSettled: () => setBusyId(null),
        onSuccess: (_res, id) => {
            setRemoved((s) => new Set(s).add(id));
            setToast(t("adminPayments.toast.approveSuccess"));
            queryClient.invalidateQueries({queryKey: ["admin-payment-requests"]});
        },
    });

    const rejectMut = useMutation({
        mutationFn: ({id, description}: { id: number; description: string }) =>
            rejectPaymentRequest(id, {description} as any),
        onMutate: ({id}) => setBusyId(id),
        onSettled: () => setBusyId(null),
        onSuccess: (_res, {id}) => {
            setRemoved((s) => new Set(s).add(id));
            setToast(t("adminPayments.toast.rejectSuccess"));
            setRejectModal({open: false, id: null});
            queryClient.invalidateQueries({queryKey: ["admin-payment-requests"]});
        },
    });

    if (!superAdmin) {
        return (
            <div className="container mx-auto px-4 py-8">
                <p className="text-center text-red-500 font-semibold">
                    {t("adminPayments.accessDenied")}
                </p>
            </div>
        );
    }

    return (
        <div className="container mx-auto px-4 py-8">
            <h1 className="text-2xl font-bold mb-6">{t("adminPayments.pageTitle")}</h1>

            {toast && <Toast text={toast} onClose={() => setToast("")}/>}

            {isLoading && <div
                className="self-start sm:self-auto w-5 h-5 border-2 border-cyan-400 border-t-transparent justify-self-center rounded-full animate-spin"/>}
            {isError && <p className="text-red-500">{t("adminPayments.error")}</p>}

            <div className="grid gap-4">
                {plisio.map((p) => (
                    <Row key={p.id as number}>
                        <Info p={p}/>
                        <span className="text-gray-400 text-sm">{t("adminPayments.plisioNoAction")}</span>
                    </Row>
                ))}

                {c2cNoReceipt.map((p) => {
                    const id = p.id as number;
                    const busy = busyId === id;
                    return (
                        <Row key={id}>
                            <Info p={p} extra={t("adminPayments.incompleteNoReceipt")}/>
                            <button
                                disabled={busy}
                                onClick={() => setRejectModal({open: true, id})}
                                className="px-3 py-2 rounded-lg bg-rose-600 hover:bg-rose-700 text-white disabled:opacity-60"
                            >
                                {busy ? t("adminPayments.buttons.rejecting") : t("adminPayments.buttons.reject")}
                            </button>
                        </Row>
                    );
                })}

                {c2cWithReceipt.map((p) => {
                    const id = p.id as number;
                    const busy = busyId === id;
                    const receiptPath = (p as any).receiptImageUrl || (p as any).receiptImage || null;
                    const receiptUrl = buildReceiptUrl(receiptPath);

                    return (
                        <Row key={id}>
                            <Info p={p}/>
                            <div className="flex items-center gap-2 flex-wrap">
                                {receiptUrl && (
                                    <button
                                        className="px-3 py-2 rounded-lg bg-cyan-600 hover:bg-cyan-700 text-white"
                                        onClick={() => setReceiptModal({open: true, url: receiptUrl})}
                                    >
                                        {t("adminPayments.buttons.viewReceipt")}
                                    </button>
                                )}

                                <button
                                    disabled={busy}
                                    onClick={() => approveMut.mutate(id)}
                                    className="px-3 py-2 rounded-lg bg-emerald-600 hover:bg-emerald-700 text-white disabled:opacity-60"
                                >
                                    {busy ? t("adminPayments.buttons.approving") : t("adminPayments.buttons.approve")}
                                </button>
                                <button
                                    disabled={busy}
                                    onClick={() => setRejectModal({open: true, id})}
                                    className="px-3 py-2 rounded-lg bg-rose-600 hover:bg-rose-700 text-white disabled:opacity-60"
                                >
                                    {busy ? t("adminPayments.buttons.rejecting") : t("adminPayments.buttons.reject")}
                                </button>
                            </div>
                        </Row>
                    );
                })}
            </div>

            {rejectModal.open && rejectModal.id != null && (
                <RejectModal
                    busy={rejectMut.isPending}
                    onClose={() => setRejectModal({open: false, id: null})}
                    onConfirm={(reason) => rejectMut.mutate({id: rejectModal.id!, description: reason})}
                />
            )}

            {receiptModal.open && receiptModal.url && (
                <ImageModal url={receiptModal.url} onClose={() => setReceiptModal({open: false, url: null})}/>
            )}
        </div>
    );
};

export default WaitingForPayment;

const Row: React.FC<{ children: React.ReactNode }> = ({children}) => (
    <div
        className="bg-gray-800/60 border border-gray-700 rounded-xl p-4 flex flex-col sm:flex-row sm:items-center justify-between gap-3">
        {children}
    </div>
);

const Info: React.FC<{ p: PaymentRequest; extra?: string }> = ({p, extra}) => {
    const {t} = useTranslation();
    return (
        <div className="min-w-0">
            <p className="font-semibold truncate">#{p.id} • {t("adminPayments.card.amountLabel")}: {p.amount}</p>
            <p className="text-sm text-gray-400 truncate">{t("adminPayments.card.userLabel")}: {p.userId}</p>
            {extra && <span className="inline-flex items-center gap-2 text-amber-400 text-xs mt-1">{extra}</span>}
        </div>
    );
};

const Toast: React.FC<{ text: string; onClose: () => void }> = ({text, onClose}) => {
    React.useEffect(() => {
        const t = setTimeout(onClose, 2500);
        return () => clearTimeout(t);
    }, [onClose]);
    return (
        <div className="mb-4 rounded-lg border border-emerald-700 bg-emerald-900/40 px-4 py-2 text-emerald-200">
            {text}
        </div>
    );
};

const RejectModal: React.FC<{
    busy?: boolean;
    onClose: () => void;
    onConfirm: (reason: string) => void;
}> = ({busy, onClose, onConfirm}) => {
    const {t} = useTranslation();
    const [reason, setReason] = React.useState("");
    return (
        <div className="fixed inset-0 bg-black/60 flex items-center justify-center p-4 z-50" onClick={onClose}>
            <div className="bg-gray-900 border border-gray-700 rounded-xl p-5 w-full max-w-md"
                 onClick={(e) => e.stopPropagation()}>
                <h3 className="text-lg font-bold mb-3">{t("adminPayments.rejectModal.title")}</h3>
                <textarea
                    rows={4}
                    className="w-full bg-gray-800 border border-gray-700 rounded-lg p-2 mb-4"
                    placeholder={t("adminPayments.rejectModal.placeholder")}
                    value={reason}
                    onChange={(e) => setReason(e.target.value)}
                />
                <div className="flex items-center justify-end gap-2">
                    <button onClick={onClose}
                            className="px-3 py-2 rounded-lg bg-gray-700 text-white">{t("buttons.cancel")}</button>
                    <button
                        disabled={busy || !reason.trim()}
                        onClick={() => onConfirm(reason.trim())}
                        className="px-3 py-2 rounded-lg bg-rose-600 hover:bg-rose-700 text-white disabled:opacity-60"
                    >
                        {busy ? t("adminPayments.buttons.rejecting") : t("adminPayments.rejectModal.confirmButton")}
                    </button>
                </div>
            </div>
        </div>
    );
};

const ImageModal: React.FC<{ url: string; onClose: () => void; }> = ({url, onClose}) => {
    const {t} = useTranslation();
    return (
        <div className="fixed inset-0 bg-black/75 flex items-center justify-center p-4 z-50" onClick={onClose}>
            <div className="bg-gray-900 border border-gray-700 rounded-xl p-3 w-full max-w-2xl"
                 onClick={(e) => e.stopPropagation()}>
                <img src={url} alt={t("common.receiptAlt")} className="w-full max-h-[80vh] object-contain rounded"/>
                <div className="mt-3 flex justify-end">
                    <button onClick={onClose}
                            className="px-3 py-2 rounded-lg bg-gray-700 text-white">{t("buttons.close")}</button>
                </div>
            </div>
        </div>
    );
};
