import React, {useEffect, useMemo, useState} from "react";
import {useTranslation} from "react-i18next";
import {useDirection} from "../../hooks/useDirection";
import {isSuperAdmin} from "../../models/auth.utils";
import {AnimatePresence, motion} from "framer-motion";
import {FaEdit, FaPlus, FaTrash} from "react-icons/fa";

import type {BankAccount, CreateBankAccountDto, UpdateBankAccountDto,} from "../../models/bankAccounts/bankAccounts";
import {
    useBankAccountsAdmin,
    useCreateBankAccountAdmin,
    useDeleteBankAccountAdmin,
    useUpdateBankAccountAdmin,
} from "../../hooks/useBankAccount.ts";

const formatCard = (num: string | null | undefined) => {
    if (!num) return "—";
    const raw = num.replace(/\D/g, "").slice(0, 16);
    return raw.replace(/(.{4})/g, "$1 ").trim();
};


const ModalShell: React.FC<{
    title: string;
    onClose: () => void;
    children: React.ReactNode;
}> = ({title, onClose, children}) => (
    <AnimatePresence>
        <motion.div
            initial={{opacity: 0}}
            animate={{opacity: 1}}
            exit={{opacity: 0}}
            className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60"
            onClick={onClose}
        >
            <motion.div
                initial={{y: 24, scale: 0.98, opacity: 0}}
                animate={{y: 0, scale: 1, opacity: 1}}
                exit={{y: 24, scale: 0.98, opacity: 0}}
                transition={{type: "spring", stiffness: 260, damping: 24}}
                className="w-full max-w-lg rounded-2xl bg-gray-900 border border-gray-700 shadow-2xl"
                onClick={(e) => e.stopPropagation()}
            >
                <div className="flex items-center justify-between px-5 py-3 border-b border-gray-700">
                    <h3 className="text-lg font-bold text-white">{title}</h3>
                    <button onClick={onClose} className="text-gray-400 hover:text-white">
                        <i className="fas fa-times"/>
                    </button>
                </div>
                <div className="p-5">{children}</div>
            </motion.div>
        </motion.div>
    </AnimatePresence>
);

const StatusTextToggle: React.FC<{
    value: boolean;
    onToggle: () => void;
}> = ({value, onToggle}) => {
    const {t} = useTranslation();
    return (
        <button
            onClick={onToggle}
            className="relative inline-flex items-center gap-2 px-3 py-1.5 rounded-lg bg-gray-800 border border-gray-700 hover:border-cyan-500 transition-colors"
        >
            <AnimatePresence mode="popLayout" initial={false}>
                {value ? (
                    <motion.span
                        key="on"
                        initial={{opacity: 0, y: 4, scale: 0.98}}
                        animate={{opacity: 1, y: 0, scale: 1}}
                        exit={{opacity: 0, y: -4, scale: 0.98}}
                        className="text-emerald-300 text-sm font-semibold flex items-center gap-1"
                    >
                        <i className="fas fa-check-circle"/>
                        {t("bankAccountsPage.status.active")}
                    </motion.span>
                ) : (
                    <motion.span
                        key="off"
                        initial={{opacity: 0, y: 4, scale: 0.98}}
                        animate={{opacity: 1, y: 0, scale: 1}}
                        exit={{opacity: 0, y: -4, scale: 0.98}}
                        className="text-gray-300 text-sm font-semibold flex items-center gap-1"
                    >
                        <i className="fas fa-circle"/>
                        {t("bankAccountsPage.status.inactive")}
                    </motion.span>
                )}
            </AnimatePresence>
        </button>
    );
};

const StatusPill: React.FC<{ active: boolean }> = ({active}) => {
    const {t} = useTranslation();
    return (
        <AnimatePresence mode="popLayout" initial={false}>
            <motion.span
                key={active ? "on" : "off"}
                initial={{opacity: 0, y: -6}}
                animate={{opacity: 1, y: 0}}
                exit={{opacity: 0, y: 6}}
                className={`px-3 py-1 rounded-full text-xs font-semibold ring-1 ${
                    active
                        ? "bg-emerald-500/15 text-emerald-300 ring-emerald-500/30"
                        : "bg-gray-600/20 text-gray-300 ring-gray-500/30"
                }`}
            >
                {active ? t("bankAccountsPage.status.active") : t("bankAccountsPage.status.inactive")}
            </motion.span>
        </AnimatePresence>
    );
};

type BankFormValues = {
    bankName: string | null;
    holderName: string | null;
    cardNumber: string | null;
    isActive: boolean;
};

const BankFormModal: React.FC<{
    open: boolean;
    initial?: Partial<BankFormValues>;
    submitting?: boolean;
    onSubmit: (values: BankFormValues) => void;
    onClose: () => void;
}> = ({open, initial, submitting, onSubmit, onClose}) => {
    const {t} = useTranslation();
    const dir = useDirection();
    const [values, setValues] = useState<BankFormValues>({
        bankName: initial?.bankName ?? "",
        holderName: initial?.holderName ?? "",
        cardNumber: initial?.cardNumber ?? "",
        isActive: initial?.isActive ?? true,
    });

    useEffect(() => {
        if (open) {
            setValues({
                bankName: initial?.bankName ?? "",
                holderName: initial?.holderName ?? "",
                cardNumber: initial?.cardNumber ?? "",
                isActive: initial?.isActive ?? true,
            });
        }
    }, [initial, open]);

    if (!open) return null;

    return (
        <ModalShell title={t("bankAccountsPage.form.title")} onClose={onClose}>
            <div className={dir.classes("grid gap-3", dir.isRtl && "text-right")}>
                <label className="space-y-1">
                    <span className="text-sm text-gray-300">{t("bankAccountsPage.form.fields.bankName")}</span>
                    <input
                        className="w-full px-3 py-2 rounded-lg bg-gray-800 border border-gray-700 text-white focus:ring-2 focus:ring-cyan-500 outline-none"
                        value={values.bankName ?? ""}
                        onChange={(e) => setValues((s) => ({...s, bankName: e.target.value}))}
                        placeholder={t("bankAccountsPage.form.placeholders.bankName")}
                    />
                </label>

                <label className="space-y-1">
                    <span className="text-sm text-gray-300">{t("bankAccountsPage.form.fields.holderName")}</span>
                    <input
                        className="w-full px-3 py-2 rounded-lg bg-gray-800 border border-gray-700 text-white focus:ring-2 focus:ring-cyan-500 outline-none"
                        value={values.holderName ?? ""}
                        onChange={(e) => setValues((s) => ({...s, holderName: e.target.value}))}
                        placeholder={t("bankAccountsPage.form.placeholders.holderName")}
                    />
                </label>

                <label className="space-y-1">
                    <span className="text-sm text-gray-300">{t("bankAccountsPage.form.fields.cardNumber")}</span>
                    <input
                        className="w-full px-3 py-2 rounded-lg bg-gray-800 border border-gray-700 text-white focus:ring-2 focus:ring-cyan-500 outline-none font-mono tracking-wider"
                        value={values.cardNumber ?? ""}
                        onChange={(e) => {
                            const raw = e.target.value.replace(/\D/g, "").slice(0, 16);
                            const formatted = raw.replace(/(.{4})/g, "$1 ").trim();
                            setValues((s) => ({...s, cardNumber: formatted}));
                        }}
                        placeholder="6037 9911 .... ...."
                    />
                </label>

                <div className={dir.classes("mt-1 flex items-center gap-2", dir.isRtl && "flex-row-reverse")}>
                    <span className="text-sm text-gray-300">{t("bankAccountsPage.form.fields.isActive")}:</span>
                    <StatusTextToggle
                        value={!!values.isActive}
                        onToggle={() => setValues((s) => ({...s, isActive: !s.isActive}))}
                    />
                </div>

                <div className={dir.classes("mt-4 flex justify-end gap-2", dir.isRtl && "flex-row-reverse")}>
                    <button
                        onClick={onClose}
                        className="px-4 py-2 rounded-lg bg-gray-700 hover:bg-gray-600 text-white"
                        disabled={submitting}
                    >
                        {t("buttons.close")}
                    </button>
                    <button
                        onClick={() => onSubmit(values)}
                        className="px-4 py-2 rounded-lg bg-cyan-600 hover:bg-cyan-700 text-white disabled:opacity-60"
                        disabled={submitting}
                    >
                        {submitting ? t("buttons.saving") : t("buttons.save")}
                    </button>
                </div>
            </div>
        </ModalShell>
    );
};

const ConfirmDeleteModal: React.FC<{
    open: boolean;
    title?: string;
    message?: string;
    busy?: boolean;
    onConfirm: () => void;
    onClose: () => void;
}> = ({open, title, message, busy, onConfirm, onClose}) => {
    const {t} = useTranslation();
    if (!open) return null;
    return (
        <ModalShell title={title || t("bankAccountsPage.deleteModal.defaultTitle")} onClose={onClose}>
            <p className="text-gray-300 mb-4">{message || t("bankAccountsPage.deleteModal.defaultMessage")}</p>
            <div className="flex justify-end gap-2">
                <button onClick={onClose} className="px-4 py-2 rounded-lg bg-gray-700 text-white">
                    {t("buttons.cancel")}
                </button>
                <button
                    onClick={onConfirm}
                    disabled={busy}
                    className="px-4 py-2 rounded-lg bg-rose-600 hover:bg-rose-700 text-white disabled:opacity-60"
                >
                    {busy ? t("buttons.deleting") : t("buttons.delete")}
                </button>
            </div>
        </ModalShell>
    );
};

const CardSkeleton: React.FC = () => (
    <div className="rounded-2xl bg-gray-800/60 border border-gray-700/60 p-5 animate-pulse">
        <div className="flex items-center justify-between mb-3">
            <div className="h-4 w-40 bg-gray-700 rounded"/>
            <div className="h-7 w-20 bg-gray-700 rounded-full"/>
        </div>
        <div className="h-6 w-64 bg-gray-700 rounded mb-3"/>
        <div className="flex gap-2">
            <div className="h-10 w-24 bg-gray-700 rounded"/>
            <div className="h-10 w-20 bg-gray-700 rounded"/>
        </div>
    </div>
);


const AllBanks: React.FC = () => {
    const {t} = useTranslation();
    const dir = useDirection();
    const superAdmin = isSuperAdmin();

    const {data: accounts = [], isLoading, isError} = useBankAccountsAdmin();
    const createMut = useCreateBankAccountAdmin();
    const updateMut = useUpdateBankAccountAdmin();
    const deleteMut = useDeleteBankAccountAdmin();

    const [items, setItems] = useState<BankAccount[]>([]);
    useEffect(() => setItems(accounts), [accounts]);

    const [createOpen, setCreateOpen] = useState(false);
    const [edit, setEdit] = useState<{ open: boolean; item: BankAccount | null }>({open: false, item: null});
    const [del, setDelete] = useState<{ open: boolean; id: number | null }>({open: false, id: null});

    const visible = useMemo(
        () => [...items].sort((a, b) => Number(b.isActive) - Number(a.isActive) || a.id - b.id),
        [items]
    );

    if (!superAdmin) {
        return (
            <div className="container mx-auto px-4 py-10">
                <div className="text-center text-red-500 font-semibold">
                    {t("bankAccountsPage.accessDenied")}
                </div>
            </div>
        );
    }

    const submitCreate = (valus: BankFormValues) => {
        const tempId = -Date.now();
        const tempItem: BankAccount = {
            id: tempId as any,
            bankName: valus.bankName ?? null,
            holderName: valus.holderName ?? null,
            cardNumber: valus.cardNumber ?? null,
            isActive: !!valus.isActive,
            isDeleted: false,
        };
        setItems((s) => [tempItem, ...s]);
        setCreateOpen(false);
        createMut.mutate(
            {
                bankName: tempItem.bankName,
                holderName: tempItem.holderName,
                cardNumber: tempItem.cardNumber,
                isActive: tempItem.isActive,
            } as CreateBankAccountDto,
            {
                onSuccess: (created: any) => {
                    if (created?.id) {
                        setItems((s) => s.map((x) => (x.id === tempId ? {...x, ...created, id: created.id} : x)));
                    }
                },
                onError: () => setItems((s) => s.filter((x) => x.id !== tempId)),
            }
        );
    };

    const submitEdit = (current: BankAccount, vals: BankFormValues) => {
        const prev = items;
        const patch: UpdateBankAccountDto = {
            bankName: vals.bankName ?? null,
            holderName: vals.holderName ?? null,
            cardNumber: vals.cardNumber ?? null,
            isActive: !!vals.isActive,
        };

        setItems((s) => s.map((x) => (x.id === current.id ? {...x, ...patch} : x)));
        setEdit({open: false, item: null});
        updateMut.mutate({id: current.id, data: patch}, {onError: () => setItems(prev)});
    };

    const submitDelete = (id: number) => {
        const prev = items;
        setItems((s) => s.filter((x) => x.id !== id));
        setDelete({open: false, id: null});
        deleteMut.mutate(id, {onError: () => setItems(prev)});
    };

    return (
        <div className="container mx-auto px-3 sm:px-4 py-6 sm:py-8">
            <div className={dir.classes("flex items-center justify-between mb-6", dir.isRtl && "flex-row-reverse")}>
                <h1 className="text-2xl sm:text-3xl font-bold text-white">{t("bankAccountsPage.title")}</h1>
                <button
                    onClick={() => setCreateOpen(true)}
                    className="px-4 py-2 rounded-xl bg-emerald-600 hover:bg-emerald-700 text-white shadow-lg shadow-emerald-600/20 flex items-center gap-2"
                >
                    <FaPlus className={dir.isRtl ? "ml-2" : "mr-2"}/>
                    {t("bankAccountsPage.addNew")}
                </button>
            </div>

            {isLoading && (
                <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
                    {Array.from({length: 6}).map((_, i) => <CardSkeleton key={i}/>)}
                </div>
            )}
            {isError && <p className="text-red-500">{t("bankAccountsPage.error")}</p>}

            {!isLoading && (
                <motion.div layout className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
                    <AnimatePresence>
                        {visible.map((acc) => (
                            <motion.div
                                key={acc.id}
                                layout
                                initial={{opacity: 0, y: 12}}
                                animate={{opacity: 1, y: 0}}
                                exit={{opacity: 0, y: -12}}
                                className="rounded-2xl bg-gray-800/60 border border-gray-700/60 backdrop-blur-sm p-5 shadow-md hover:shadow-lg transition"
                            >
                                <div
                                    className={dir.classes("flex items-center justify-between mb-3", dir.isRtl && "flex-row-reverse")}>
                                    <div className="flex items-center gap-3 min-w-0">
                                        <div
                                            className="h-10 w-10 rounded-xl bg-cyan-500/20 text-cyan-400 flex items-center justify-center shrink-0">
                                            <i className="fas fa-credit-card"/>
                                        </div>
                                        <div className="min-w-0">
                                            <p
                                                className="text-white font-semibold overflow-hidden"
                                                style={{
                                                    display: "-webkit-box",
                                                    WebkitLineClamp: 2,
                                                    WebkitBoxOrient: "vertical"
                                                }}
                                                title={acc.bankName || t("bankAccountsPage.unknownBank")}
                                            >
                                                {acc.bankName || t("bankAccountsPage.unknownBank")}
                                            </p>
                                            <p className="text-xs text-gray-400 truncate">
                                                {t("bankAccountsPage.holder")}: {acc.holderName || "—"}
                                            </p>
                                        </div>
                                    </div>

                                    <StatusPill active={!!acc.isActive}/>
                                </div>

                                <div className="font-mono text-lg sm:text-xl text-white tracking-widest">
                                    {formatCard(acc.cardNumber)}
                                </div>

                                <div
                                    className={dir.classes("mt-4 flex items-center gap-2", dir.isRtl && "flex-row-reverse")}>
                                    <button
                                        onClick={() => setEdit({open: true, item: acc})}
                                        className="flex-1 px-3 py-2 rounded-lg bg-cyan-600 hover:bg-cyan-700 text-white flex items-center justify-center gap-2"
                                    >
                                        <FaEdit/> {t("buttons.edit")}
                                    </button>
                                    <button
                                        onClick={() => setDelete({open: true, id: acc.id})}
                                        className="px-3 py-2 rounded-lg bg-rose-600 hover:bg-rose-700 text-white flex items-center justify-center gap-2"
                                    >
                                        <FaTrash/> {t("buttons.delete")}
                                    </button>
                                </div>
                            </motion.div>
                        ))}
                    </AnimatePresence>
                </motion.div>
            )}

            <BankFormModal
                open={createOpen}
                submitting={createMut.isPending}
                onClose={() => setCreateOpen(false)}
                onSubmit={submitCreate}
            />

            <BankFormModal
                open={edit.open}
                initial={{
                    bankName: edit.item?.bankName ?? "",
                    holderName: edit.item?.holderName ?? "",
                    cardNumber: edit.item?.cardNumber ?? "",
                    isActive: edit.item?.isActive ?? true,
                }}
                submitting={updateMut.isPending}
                onClose={() => setEdit({open: false, item: null})}
                onSubmit={(vals) => edit.item && submitEdit(edit.item, vals)}
            />

            <ConfirmDeleteModal
                open={del.open}
                busy={deleteMut.isPending}
                onClose={() => setDelete({open: false, id: null})}
                onConfirm={() => del.id && submitDelete(del.id)}
                title={t("bankAccountsPage.deleteModal.title")}
                message={t("bankAccountsPage.deleteModal.message")}
            />
        </div>
    );
};

export default AllBanks;
