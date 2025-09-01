import React, {useEffect, useState} from "react";
import {AnimatePresence, motion} from "framer-motion";
import {useTranslation} from "react-i18next";
import {isSuperAdmin} from "../../models/auth.utils";
import {ITEMS_PER_PAGE, useAdminPayments} from "../../hooks/useAdminPayments";
import {useHolderMap} from "../../hooks/useHolderMap";
import {usePaymentActions} from "../../hooks/usePaymentActions";
import {AdminPaymentsFilters} from "./payment/AdminPaymentsFilters";
import {AdminPaymentCard} from "./payment/AdminPaymentCard";


const SkeletonCard: React.FC = () => (
    <motion.div initial={{opacity: 0.35}} animate={{opacity: 1}}
                transition={{repeat: Infinity, repeatType: "reverse", duration: 0.9}}
                className="bg-gray-800/60 border border-gray-700 rounded-xl p-3 sm:p-4">
        <div className="animate-pulse">
            <div className="h-5 w-40 bg-gray-700 rounded mb-2"/>
            <div className="h-4 w-24 bg-gray-700 rounded mb-4"/>
            <div className="h-14 sm:h-16 w-full bg-gray-700/70 rounded"/>
        </div>
    </motion.div>
);

const AdminPayments: React.FC = () => {
    const {t, i18n} = useTranslation();
    const isFa = (i18n.language ?? "").toLowerCase().startsWith("fa");
    const superAdmin = isSuperAdmin();

    const {page, setPage, handleFilterChange, resetFilters, totalPages, totalCount, list, query} =
        useAdminPayments(superAdmin);

    const holderById = useHolderMap();
    const {busyId, rejectModal, setRejectModal, approve, reject, approving, rejecting} = usePaymentActions();

    const [isFiltersOpen, setIsFiltersOpen] = useState(false);
    const [isPageSwitch, setIsPageSwitch] = useState(false);
    const [dateFromValue, setDateFromValue] = useState<any>(null);
    const [dateToValue, setDateToValue] = useState<any>(null);
    const [receiptModal, setReceiptModal] = useState<{ open: boolean; url: string | null }>({open: false, url: null});

    useEffect(() => {
        if (!query.isFetching) setIsPageSwitch(false);
    }, [query.isFetching]);
    useEffect(() => {
        setIsPageSwitch(false);
    }, [list.length, page]);

    if (!superAdmin) {
        return (
            <div className="container mx-auto px-3 sm:px-4 py-6 sm:py-8">
                <p className="text-center text-red-500 font-semibold">{t("allPaymentsPage.accessDenied")}</p>
            </div>
        );
    }

    return (
        <div className="container mx-auto px-3 sm:px-4 py-6 sm:py-8 text-white">
            <AnimatePresence>
                {isPageSwitch && (
                    <motion.div key="top-progress" initial={{scaleX: 0}} animate={{scaleX: 1}} exit={{scaleX: 0}}
                                transition={{duration: 0.7, ease: "easeInOut"}}
                                style={{transformOrigin: "0% 50%"}}
                                className="fixed top-0 left-0 right-0 h-1 bg-cyan-400 z-50"/>
                )}
            </AnimatePresence>

            <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3 mb-4">
                <h1 className="text-2xl sm:text-3xl font-bold truncate">{t("allPaymentsPage.title")}</h1>

            </div>

            <AdminPaymentsFilters
                isOpen={isFiltersOpen} setOpen={setIsFiltersOpen} isFa={isFa}
                onChange={handleFilterChange}
                onReset={() => {
                    resetFilters();
                    setDateFromValue(null);
                    setDateToValue(null);
                }}
                dateFrom={dateFromValue} setDateFrom={setDateFromValue}
                dateTo={dateToValue} setDateTo={setDateToValue}
            />

            {query.isFetching && <div
                className="self-start sm:self-auto w-5 h-5 border-2 border-cyan-400 border-t-transparent justify-self-center rounded-full animate-spin"></div>}
            {query.isError && <p className="text-center text-red-500">{t("allPaymentsPage.error")}</p>}

            <div className="grid gap-3 sm:gap-4 grid-cols-1 md:grid-cols-2 xl:grid-cols-3">
                <AnimatePresence mode="popLayout">
                    {isPageSwitch
                        ? Array.from({length: Math.min(ITEMS_PER_PAGE, 6)}).map((_, i) => <SkeletonCard
                            key={`sk-${i}`}/>)
                        : list.map((item) => (
                            <motion.div key={item.id} layout initial={{opacity: 0, y: 16}} animate={{opacity: 1, y: 0}}
                                        exit={{opacity: 0}}>
                                <AdminPaymentCard
                                    item={item}
                                    lang={i18n.language}
                                    holderName={holderById[(item as any).bankAccountId as number] ?? null}
                                    busy={busyId === (item.id as number)}
                                    onShowReceipt={(url) => setReceiptModal({open: true, url})}
                                    onApprove={(id) => approve(id)}
                                    onReject={(id) => setRejectModal({open: true, id})}
                                />
                            </motion.div>
                        ))
                    }
                </AnimatePresence>
            </div>

            {totalCount > 0 && (
                <Pagination
                    page={page}
                    totalPages={totalPages}
                    onPageChange={(p) => {
                        setIsPageSwitch(true);
                        setPage(p);
                    }}
                    totalItems={totalCount}
                    isBusy={isPageSwitch || query.isFetching}
                />
            )}

            {rejectModal.open && rejectModal.id != null && (
                <RejectModal
                    busy={approving || rejecting}
                    onClose={() => setRejectModal({open: false, id: null})}
                    onConfirm={(reason) => reject(rejectModal.id!, reason)}
                />
            )}

            {receiptModal.open && receiptModal.url && (
                <ImageModal url={receiptModal.url} onClose={() => setReceiptModal({open: false, url: null})}/>
            )}
        </div>
    );
};

export default AdminPayments;

const Pagination: React.FC<{
    page: number; totalPages: number; onPageChange: (p: number) => void; totalItems: number; isBusy?: boolean;
}> = ({page, totalPages, onPageChange, totalItems, isBusy}) => {
    const {t} = useTranslation();
    const from = totalItems === 0 ? 0 : (page - 1) * ITEMS_PER_PAGE + 1;
    const to = Math.min(page * ITEMS_PER_PAGE, totalItems);

    return (
        <div className="mt-6 flex flex-col sm:flex-row items-center justify-between gap-3 sm:gap-4">
            <p className="text-gray-400 text-xs sm:text-sm">{t("pagination.info", {from, to, total: totalItems})}</p>
            <div className="flex items-center gap-2">
                <button disabled={page <= 1 || !!isBusy} onClick={() => onPageChange(page - 1)}
                        className="px-3 py-2 rounded-lg bg-gray-700/60 text-white disabled:opacity-50 text-sm">
                    {t("pagination.prev")}
                </button>
                <span className="text-gray-300 text-sm font-mono">{page} / {totalPages}</span>
                <button disabled={page >= totalPages || !!isBusy} onClick={() => onPageChange(page + 1)}
                        className="px-3 py-2 rounded-lg bg-gray-700/60 text-white disabled:opacity-50 text-sm">
                    {t("pagination.next")}
                </button>
            </div>
        </div>
    );
};

export const RejectModal: React.FC<{
    busy?: boolean;
    onClose: () => void;
    onConfirm: (reason: string) => void;
}> = ({busy, onClose, onConfirm}) => {
    const {t} = useTranslation();
    const [reason, setReason] = useState("");
    return (
        <div className="fixed inset-0 bg-black/60 flex items-center justify-center p-4 z-50" onClick={onClose}>
            <div className="bg-gray-900 border border-gray-700 rounded-xl p-5 w-full max-w-md"
                 onClick={(e) => e.stopPropagation()}>
                <h3 className="text-lg font-bold mb-3 text-center">{t("modals.rejectTitle")}</h3>
                <textarea rows={4} className="w-full bg-gray-800 border border-gray-700 rounded-lg p-2 mb-4"
                          placeholder={t("modals.rejectPlaceholder") as string} value={reason}
                          onChange={(e) => setReason(e.target.value)}/>
                <div className="flex items-center justify-end gap-2">
                    <button onClick={onClose}
                            className="px-4 py-2 rounded-lg bg-gray-700 hover:bg-gray-600 text-white font-semibold">{t("buttons.cancel")}</button>
                    <button disabled={busy || !reason.trim()} onClick={() => onConfirm(reason.trim())}
                            className="px-4 py-2 rounded-lg bg-rose-600 hover:bg-rose-700 text-white font-semibold disabled:opacity-60">{t("buttons.reject")}</button>
                </div>
            </div>
        </div>
    );
};

export const ImageModal: React.FC<{ url: string; onClose: () => void }> = ({url, onClose}) => {
    const {t} = useTranslation();
    return (
        <div className="fixed inset-0 bg-black/75 flex items-center justify-center p-4 z-50" onClick={onClose}>
            <div className="bg-gray-900 border border-gray-700 rounded-xl p-3 w-full max-w-2xl"
                 onClick={(e) => e.stopPropagation()}>
                <img src={url} alt={t("common.receiptAlt")} className="w-full max-h-[80vh] object-contain rounded"/>
                <div className="mt-3 flex justify-end">
                    <button onClick={onClose}
                            className="px-4 py-2 rounded-lg bg-gray-700 hover:bg-gray-600 text-white font-semibold">{t("buttons.close")}</button>
                </div>
            </div>
        </div>
    );
};
