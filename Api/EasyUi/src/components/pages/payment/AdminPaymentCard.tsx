import React from "react";
import {useTranslation} from "react-i18next";
import {buildReceiptUrl} from "../../../models/receiptUrl";
import {formatInTehran} from "../../../utils/dates";
import {isActionable} from "../../../models/paymentStatus";
import {PaymentMethod} from "../../../models/peyment/paymentMethod";
import StatusBadge from "../../../components/ui/StatusBadge";
import type {PaymentRequest as AppPaymentRequest} from "../../../models/peyment/payment";

const isPlisio = (p: AppPaymentRequest) => Number(p?.method) === PaymentMethod.Plisio;
const hasReceipt = (p: AppPaymentRequest) =>
    Boolean((p as any)?.receiptImage || (p as any)?.receiptImageUrl);
const canApprove = (p: AppPaymentRequest) =>
    !isPlisio(p) && isActionable(Number(p.status)) && hasReceipt(p);
const isIncomplete = (p: AppPaymentRequest) =>
    !isPlisio(p) && isActionable(Number(p.status)) && !hasReceipt(p);

type Props = {
    item: AppPaymentRequest;
    lang?: string;
    holderName?: string | null;
    busy?: boolean;
    onShowReceipt?: (url: string) => void;
    onApprove?: (id: number) => void;
    onReject?: (id: number) => void;
};

export const AdminPaymentCard: React.FC<Props> = ({
                                                      item, lang, holderName, busy,
                                                      onShowReceipt, onApprove, onReject
                                                  }) => {
    const {t} = useTranslation();
    const receiptUrl = !isPlisio(item)
        ? buildReceiptUrl((item as any).receiptImageUrl || (item as any).receiptImage)
        : null;

    const methodText = isPlisio(item) ? t("paymentMethods.plisio") : t("paymentMethods.cardToCard");
    const created = (item as any).createDate ?? (item as any).createdAt;
    const createdAtTehran = formatInTehran(created, lang);

    return (
        <div className="bg-gray-800/60 border border-gray-700 rounded-xl p-3 sm:p-4 flex flex-col gap-3 sm:gap-4">
            <div className="flex items-start justify-between gap-3">
                <div className="min-w-0 space-y-1.5">
                    <div className="flex items-center gap-2">
            <span
                className="inline-flex items-center gap-1 rounded-lg bg-white/5 ring-1 ring-white/10 px-2 py-1 text-[11px] sm:text-xs font-semibold">
              <span className="opacity-70">ID</span>
              <span className="font-mono">#{item.id}</span>
            </span>
                        <span
                            className="inline-flex items-center rounded-full bg-cyan-500/15 text-cyan-300 ring-1 ring-cyan-400/20 px-2 py-0.5 text-[10px] sm:text-xs">
              {methodText}
            </span>
                    </div>

                    <div className="flex items-baseline gap-2 text-white">
                        <span className="text-xs sm:text-sm opacity-70">{t("common.amount")}:</span>
                        <span
                            className="font-bold text-sm sm:text-base font-mono tabular-nums">{item.amount.toLocaleString()}</span>
                    </div>

                    <dl className="grid grid-cols-1 sm:grid-cols-2 gap-x-4 gap-y-1 text-[11px] sm:text-[13px] text-gray-400">
                        <div className="flex items-center gap-1" title={`${t("common.userId")}: ${item.userId}`}>
                            <dt className="opacity-70">{t("common.userId")}:</dt>
                            <dd className="truncate font-mono">{item.userId}</dd>
                        </div>
                        <div className="flex items-center gap-1">
                            <dt className="opacity-70">{t("common.createDate")}:</dt>
                            <dd className="font-mono">{createdAtTehran}</dd>
                        </div>
                        {!isPlisio(item) && (
                            <div className="flex items-center gap-1 sm:col-span-2" title={holderName ?? ""}>
                                <dt className="opacity-70">{t("bankAccount.holderName")}:</dt>
                                <dd className="truncate">{holderName ?? "-"}</dd>
                            </div>
                        )}
                    </dl>
                </div>

                <div className="translate-y-0.5">
                    <StatusBadge status={Number(item.status)}/>
                </div>
            </div>

            <div className="flex items-end justify-between gap-3">
                {!isPlisio(item) && (
                    receiptUrl ? (
                        <button
                            onClick={() => onShowReceipt?.(receiptUrl)}
                            className="w-20 h-14 sm:w-24 sm:h-16 bg-black/20 rounded-lg overflow-hidden flex-shrink-0 ring-1 ring-gray-700/70"
                            title={t("allPaymentsPage.card.receiptThumbnailAlt") as string}
                        >
                            <img src={receiptUrl} alt="" className="w-full h-full object-cover"/>
                        </button>
                    ) : (
                        <div
                            className="w-20 h-14 sm:w-24 sm:h-16 flex items-center justify-center text-[10px] sm:text-xs text-gray-500 border border-dashed border-gray-700 rounded-lg">
                            {t("allPaymentsPage.card.noReceipt")}
                        </div>
                    )
                )}

                <div className="flex items-center gap-2 flex-wrap justify-end">
                    {canApprove(item) && (
                        <>
                            <button disabled={!!busy} onClick={() => onApprove?.(item.id as number)}
                                    className="px-3 py-2 rounded-lg bg-emerald-600 hover:bg-emerald-700 font-semibold disabled:opacity-60 text-sm">
                                {t("buttons.approve")}
                            </button>
                            <button disabled={!!busy} onClick={() => onReject?.(item.id as number)}
                                    className="px-3 py-2 rounded-lg bg-rose-600 hover:bg-rose-700 font-semibold disabled:opacity-60 text-sm">
                                {t("buttons.reject")}
                            </button>
                        </>
                    )}
                    {isIncomplete(item) && (
                        <button disabled={!!busy} onClick={() => onReject?.(item.id as number)}
                                className="px-3 py-2 rounded-lg bg-rose-600 hover:bg-rose-700 font-semibold disabled:opacity-60 text-sm">
                            {t("buttons.reject")}
                        </button>
                    )}
                </div>
            </div>
        </div>
    );
};
