import React from "react";
import {motion, AnimatePresence} from "framer-motion";
import {FaChevronDown, FaFilter, FaUndo} from "react-icons/fa";
import {useTranslation} from "react-i18next";
import DateFilter from "../../../components/filters/DateFilter";
import {PaymentMethod} from "../../../models/peyment/paymentMethod";
import type {GetPaymentsParams} from "../../../hooks/useAdminPayments.ts";

export const AdminPaymentsFilters: React.FC<{
    isOpen: boolean; setOpen: (v: boolean) => void; isFa: boolean;
    onChange: (field: keyof GetPaymentsParams, value?: string) => void;
    onReset: () => void;
    dateFrom: any; setDateFrom: (v: any) => void;
    dateTo: any; setDateTo: (v: any) => void;
}> = ({isOpen, setOpen, isFa, onChange, onReset, dateFrom, setDateFrom, dateTo, setDateTo}) => {
    const {t} = useTranslation();
    return (
        <div className="mb-5 sm:mb-6">
            <motion.button onClick={() => setOpen(!isOpen)}
                           className="w-full flex justify-between items-center px-3 sm:px-4 py-3 bg-gray-800/80 hover:bg-gray-800 rounded-lg font-semibold">
                <span className="flex items-center gap-2"><FaFilter/>{t("filters.title")}</span>
                <motion.div animate={{rotate: isOpen ? 180 : 0}}><FaChevronDown/></motion.div>
            </motion.button>

            <AnimatePresence>
                {isOpen && (
                    <motion.div initial={{height: 0, opacity: 0, marginTop: 0}}
                                animate={{height: 'auto', opacity: 1, marginTop: '0.75rem'}}
                                exit={{height: 0, opacity: 0, marginTop: 0}}
                                className="bg-gray-800/50 border border-gray-700/60 rounded-xl p-3 sm:p-4 overflow-hidden">
                        <div className="grid gap-3 sm:gap-4 grid-cols-2 sm:grid-cols-3 lg:grid-cols-6 items-end">
                            <FilterInput label={t("filters.fromAmount")} type="number"
                                         onChange={v => onChange("FromAmount", v)}/>
                            <FilterInput label={t("filters.toAmount")} type="number"
                                         onChange={v => onChange("ToAmount", v)}/>
                            <div>
                                <label
                                    className="block text-xs sm:text-sm text-gray-400 mb-1">{t("filters.dateFrom")}</label>
                                <DateFilter locale={isFa ? "fa" : "en"} value={dateFrom} type="from"
                                            onChange={(utc) => {
                                                setDateFrom(utc ? new Date(utc) : null);
                                                onChange("DateFrom", utc || "");
                                            }}/>
                            </div>
                            <div>
                                <label
                                    className="block text-xs sm:text-sm text-gray-400 mb-1">{t("filters.dateTo")}</label>
                                <DateFilter locale={isFa ? "fa" : "en"} value={dateTo} type="to"
                                            onChange={(utc) => {
                                                setDateTo(utc ? new Date(utc) : null);
                                                onChange("DateTo", utc || "");
                                            }}/>
                            </div>
                            <FilterSelect label={t("filters.method")} options={[
                                {label: t("common.all"), value: ""},
                                {label: t("paymentMethods.cardToCard"), value: String(PaymentMethod.CardToCard)},
                                {label: t("paymentMethods.plisio"), value: String(PaymentMethod.Plisio)},
                            ]} onChange={v => onChange("Method", v)}/>
                            <FilterSelect label={t("filters.status")} options={[
                                {label: t("common.all"), value: ""},
                                {label: t("paymentStatus.submitted"), value: "1"},
                                {label: t("paymentStatus.completed"), value: "2"},
                                {label: t("paymentStatus.failed"), value: "3"},
                            ]} onChange={v => onChange("Status", v)}/>
                            <div className="col-span-2 sm:col-span-3 lg:col-span-2">
                                <button onClick={onReset}
                                        className="w-full flex items-center justify-center gap-2 px-3 sm:px-4 py-2.5 rounded-lg bg-gray-600 hover:bg-gray-700 text-white font-semibold">
                                    <FaUndo/>{t("filters.resetButton")}
                                </button>
                            </div>
                        </div>
                    </motion.div>
                )}
            </AnimatePresence>
        </div>
    );
};

const FilterInput: React.FC<{ label: string; type?: string; onChange: (v: string) => void; }> = ({
                                                                                                     label,
                                                                                                     type = "text",
                                                                                                     onChange
                                                                                                 }) => (
    <div>
        <label className="block text-xs sm:text-sm text-gray-400 mb-1">{label}</label>
        <input type={type} min={type === "number" ? 0 : undefined} step={type === "number" ? "1" : undefined}
               onChange={e => onChange(e.target.value)}
               className="w-full px-3 py-2 rounded-lg bg-gray-900/60 border border-gray-700 text-sm text-white focus:ring-2 focus:ring-cyan-500 outline-none"/>
    </div>
);

const FilterSelect: React.FC<{
    label: string;
    options: { label: string; value: string }[];
    onChange: (v: string) => void;
}> =
    ({label, options, onChange}) => (
        <div>
            <label className="block text-xs sm:text-sm text-gray-400 mb-1">{label}</label>
            <select
                className="w-full px-3 py-2 rounded-lg bg-gray-900/60 border border-gray-700 text-sm text-white focus:ring-2 focus:ring-cyan-500 outline-none"
                onChange={(e) => onChange(e.target.value)}>
                {options.map(o => <option key={o.value ?? o.label} value={o.value}>{o.label}</option>)}
            </select>
        </div>
    );
