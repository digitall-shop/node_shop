import React from "react";
import {useTranslation} from "react-i18next";
import {PaymentMethod} from "../../../models/peyment/paymentMethod.ts";
import { buildReceiptUrl } from "../../../models/receiptUrl.ts";
import { formatInTehran } from "../../../utils/dates.ts";
import {isActionable, statusLabel} from "../../../models/paymentStatus.ts";
import StatusBadge from "../../ui/StatusBadge.tsx";


const isPlisio = (p: any) => Number(p?.method) === PaymentMethod.Plisio;
const hasReceipt = (p: any) => Boolean((p as any)?.receiptImage || (p as any)?.receiptImageUrl);
export const canContinue = (p: any) => !isPlisio(p) && isActionable(Number(p.status)) && !hasReceipt(p);

export const PaymentCard: React.FC<{
    item: any; lang?: string; onClickDetails: () => void;
}> = ({item, lang, onClickDetails}) => {
    const {t} = useTranslation();
    const methodText = isPlisio(item) ? t("paymentMethods.plisio") : t("paymentMethods.cardToCard");
    const receiptUrl = !isPlisio(item) ? buildReceiptUrl(item.receiptImageUrl || item.receiptImage) : null;
    const createDateTehran = formatInTehran(item.createDate, lang);

    return (
        <div className="bg-gray-800/60 border border-gray-700 rounded-xl p-3 sm:p-4 flex flex-col gap-3 sm:gap-4">
            <div className="flex items-start justify-between gap-3">
                <div className="min-w-0 space-y-1.5">
                    <div className="flex items-center gap-2">
            <span className="inline-flex items-center gap-1 rounded-lg bg-white/5 ring-1 ring-white/10 px-2 py-1 text-[11px] sm:text-xs font-semibold">
              <span className="opacity-70">ID</span>
              <span className="font-mono">#{item.id}</span>
            </span>
                        <span className="inline-flex items-center rounded-full bg-cyan-500/15 text-cyan-300 ring-1 ring-cyan-400/20 px-2 py-0.5 text-[10px] sm:text-xs">
              {methodText}
            </span>
                    </div>

                    <div className="flex items-baseline gap-2 text-white">
                        <span className="text-xs sm:text-sm opacity-70">{t("common.amount")}:</span>
                        <span className="font-bold text-sm sm:text-base font-mono tabular-nums">
              {item.amount.toLocaleString()}
            </span>
                    </div>

                    <dl className="grid grid-cols-1 sm:grid-cols-2 gap-x-4 gap-y-1 text-[11px] sm:text-[13px] text-gray-400">
                        <div className="flex items-center gap-1">
                            <dt className="opacity-70">{t("common.createDate")}:</dt>
                            <dd className="font-mono">{createDateTehran}</dd>
                        </div>
                        <div className="flex items-center gap-1">
                            <dt className="opacity-70">{t("status")}:</dt>
                            <dd className="truncate">{t(statusLabel(Number(item.status)))}</dd>
                        </div>
                    </dl>
                </div>

                <div className="translate-y-0.5">
                    <StatusBadge status={Number(item.status)} />
                </div>
            </div>

            <div className="flex items-end justify-between gap-3">
                {!isPlisio(item) && (
                    receiptUrl ? (
                        <button onClick={onClickDetails}
                                className="w-20 h-14 sm:w-24 sm:h-16 bg-black/20 rounded-lg overflow-hidden flex-shrink-0 ring-1 ring-gray-700/70"
                                title={t("myPaymentsPage.card.viewReceipt") as string}>
                            <img src={receiptUrl} alt={t("allPaymentsPage.card.receiptThumbnailAlt") as string}
                                 className="w-full h-full object-cover" loading="lazy" />
                        </button>
                    ) : (
                        <div className="w-20 h-14 sm:w-24 sm:h-16 flex items-center justify-center text-[10px] sm:text-xs text-gray-500 border border-dashed border-gray-700 rounded-lg">
                            {t("allPaymentsPage.card.noReceipt")}
                        </div>
                    )
                )}

                <div className="flex items-center gap-2 flex-wrap justify-end">
                    <button onClick={onClickDetails}
                            className="px-3 py-2 rounded-lg bg-cyan-600 hover:bg-cyan-700 font-semibold text-sm">
                        {canContinue(item) ? t("myPaymentsPage.card.continue") : t("details")}
                    </button>
                </div>
            </div>
        </div>
    );
};
