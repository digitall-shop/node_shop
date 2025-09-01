import React from "react";
import {useTranslation} from "react-i18next";
import {ITEMS_PER_PAGE} from "../../../hooks/useUsersQuery.ts";

export const Pagination: React.FC<{
    page: number;
    totalPages: number;
    onPageChange: (p: number) => void;
    totalItems: number;
    isBusy?: boolean;
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
