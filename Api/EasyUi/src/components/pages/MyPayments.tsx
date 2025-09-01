import React, {useEffect, useState} from "react";
import {AnimatePresence, motion} from "framer-motion";
import {useTranslation} from "react-i18next";
import {useQueryClient} from "@tanstack/react-query";
import {ITEMS_PER_PAGE, useMyPayments} from "../../hooks/useMyPayments.ts";
import { PaymentsFilters } from "./payment/PaymentsFilters.tsx";
import { PaymentCard } from "./payment/PaymentCard.tsx";
import {PaymentDetailsModal} from "./payment/PaymentDetailsModal.tsx";


const SkeletonCard: React.FC = () => (
    <motion.div initial={{opacity: 0.35}} animate={{opacity: 1}} transition={{repeat: Infinity, repeatType: "reverse", duration: 0.9}}
                className="bg-gray-800/60 border border-gray-700 rounded-xl p-3 sm:p-4">
        <div className="animate-pulse">
            <div className="h-5 w-40 bg-gray-700 rounded mb-2"/>
            <div className="h-4 w-24 bg-gray-700 rounded mb-4"/>
            <div className="h-14 sm:h-16 w-full bg-gray-700/70 rounded"/>
        </div>
    </motion.div>
);

const MyPayments: React.FC = () => {
    const {t, i18n} = useTranslation();
    const isFa = (i18n.language ?? "").toLowerCase().startsWith("fa");
    const queryClient = useQueryClient();

    const {
        page, setPage,
        handleFilterChange, resetFilters,
        totalPages, totalCount, list,
        query,
    } = useMyPayments();

    const [isFiltersOpen, setIsFiltersOpen] = useState(false);
    const [isPageSwitch, setIsPageSwitch] = useState(false);
    const [dateFromValue, setDateFromValue] = useState<any>(null);
    const [dateToValue, setDateToValue] = useState<any>(null);

    useEffect(() => { if (!query.isFetching) setIsPageSwitch(false); }, [query.isFetching]);
    useEffect(() => { setIsPageSwitch(false); }, [list.length, page]);

    const [details, setDetails] = useState<any | null>(null);

    return (
        <div className="container mx-auto px-3 sm:px-4 py-6 sm:py-8 text-white">
            <AnimatePresence>
                {isPageSwitch && (
                    <motion.div key="top-progress" initial={{scaleX: 0}} animate={{scaleX: 1}} exit={{scaleX: 0}}
                                transition={{duration: 0.7, ease: "easeInOut"}}
                                style={{transformOrigin: "0% 50%"}} className="fixed top-0 left-0 right-0 h-1 bg-cyan-400 z-50"/>
                )}
            </AnimatePresence>

            <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3 mb-4">
                <h1 className="text-2xl sm:text-3xl font-bold truncate">{t("payment.myPayments")}</h1>

            </div>

            <PaymentsFilters
                isOpen={isFiltersOpen}
                setOpen={setIsFiltersOpen}
                isFa={isFa}
                onChange={handleFilterChange}
                onReset={() => { resetFilters(); setDateFromValue(null); setDateToValue(null); }}
                dateFrom={dateFromValue} setDateFrom={setDateFromValue}
                dateTo={dateToValue} setDateTo={setDateToValue}
            />

            {query.isFetching && <div className="self-start sm:self-auto w-5 h-5 border-2 border-cyan-400 border-t-transparent justify-self-center rounded-full animate-spin" />}
            {query.isError && <p className="text-center text-red-500">{t("error")}</p>}

            <div className="grid gap-3 sm:gap-4 grid-cols-1 md:grid-cols-2 xl:grid-cols-3">
                <AnimatePresence mode="popLayout">
                    {isPageSwitch
                        ? Array.from({length: Math.min(ITEMS_PER_PAGE, 6)}).map((_, i) => <SkeletonCard key={`sk-${i}`} />)
                        : list.map((item) => (
                            <motion.div key={item.id} layout initial={{opacity: 0, y: 16}} animate={{opacity: 1, y: 0}} exit={{opacity: 0}}>
                                <PaymentCard item={item} lang={i18n.language} onClickDetails={() => setDetails(item)} />
                            </motion.div>
                        ))
                    }
                </AnimatePresence>
            </div>

            {totalCount > 0 && (
                <Pagination
                    page={page}
                    totalPages={totalPages}
                    onPageChange={(p)=>{ setIsPageSwitch(true); setPage(p); }}
                    totalItems={totalCount}
                    isBusy={isPageSwitch || query.isFetching}
                />
            )}

            {details && (
                <PaymentDetailsModal
                    payment={details}
                    onClose={() => setDetails(null)}
                    onUpdated={() => { queryClient.invalidateQueries({queryKey: ["user-payment-requests"]}); setDetails(null); }}
                />
            )}
        </div>
    );
};

export default MyPayments;

const Pagination: React.FC<{
    page: number; totalPages: number; onPageChange: (p: number) => void; totalItems: number; isBusy?: boolean;
}> = ({page, totalPages, onPageChange, totalItems, isBusy}) => {
    const {t} = useTranslation();
    const from = totalItems === 0 ? 0 : (page - 1) * ITEMS_PER_PAGE + 1;
    const to = Math.min(page * ITEMS_PER_PAGE, totalItems);

    return (
        <div className="mt-6 flex flex-col sm:flex-row items-center justify-between gap-3 sm:gap-4">
            <p className="text-gray-400 text-xs sm:text-sm">
                {t("pagination.info", {from, to, total: totalItems})}
            </p>
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
