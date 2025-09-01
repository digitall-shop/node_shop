import React, {useEffect, useMemo, useRef, useState} from "react";
import {FaTimes, FaUpload, FaTrashAlt, FaEye, FaSpinner} from "react-icons/fa";
import {useTranslation} from "react-i18next";
import {useReceiptUpload} from "../../../hooks/useReceiptUpload.ts";
import {PaymentMethod} from "../../../models/peyment/paymentMethod.ts";
import {buildReceiptUrl} from "../../../models/receiptUrl.ts";
import {isActionable, statusLabel} from "../../../models/paymentStatus.ts";

const isPlisio = (p: any) => Number(p?.method) === PaymentMethod.Plisio;

export const PaymentDetailsModal: React.FC<{
    payment: any;
    onClose: () => void;
    onUpdated: () => void;
}> = ({payment, onClose, onUpdated}) => {
    const {t} = useTranslation();
    const [file, setFile] = useState<File | null>(null);
    const [preview, setPreview] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);

    const fileInputRef = useRef<HTMLInputElement | null>(null);
    const [inputKey, setInputKey] = useState(0); // برای re-mount

    const {send, uploading, error: sendError} = useReceiptUpload(onUpdated);

    const receiptRaw = (payment as any).receiptImageUrl || (payment as any).receiptImage || null;
    const receiptUrl = useMemo(() => buildReceiptUrl(receiptRaw), [receiptRaw]);
    const allowUpload = !isPlisio(payment) && isActionable(Number(payment.status)) && !receiptUrl;

    useEffect(() => {
        if (!file) { setPreview(null); return; }
        const url = URL.createObjectURL(file);
        setPreview(url);
        return () => URL.revokeObjectURL(url);
    }, [file]);

    const onPickFile = (f: File | null) => {
        setError(null);
        if (!f) { setFile(null); return; }
        if (!f.type.startsWith("image/")) { setError(t("errors.invalidImage") as string); return; }
        const MAX = 5 * 1024 * 1024;
        if (f.size > MAX) { setError(t("errors.imageTooLarge") as string); return; }
        setFile(f);
    };

    const resetInput = () => {
        setFile(null);
        setPreview(null);
        setError(null);
        if (fileInputRef.current) fileInputRef.current.value = "";
        setInputKey(k => k + 1); // اطمینان از re-mount
    };

    const onSend = () => { if (file) send(payment.id as number, file); };

    return (
        <div className="fixed inset-0 bg-black/60 flex items-center justify-center p-4 z-50 " onClick={onClose}>
            <div className="bg-gray-900 border border-gray-700 rounded-xl p-5 w-full max-w-lg" onClick={(e)=>e.stopPropagation()}>
                <div className="flex items-start justify-between gap-3 mb-3">
                    <h3 className="text-lg font-bold">{t("payment.details")}</h3>
                    <button onClick={onClose} className="p-2 rounded hover:bg-white/5" aria-label={t("buttons.close") as string}>
                        <FaTimes />
                    </button>
                </div>

                <div className="space-y-1 text-sm text-gray-300">
                    <div className="font-mono">#{payment.id}</div>
                    <div>{t("amount")}: {payment.amount?.toLocaleString?.() ?? payment.amount}</div>
                    <div>{t("status")}: {t(statusLabel(Number(payment.status)))}</div>
                    {isPlisio(payment) && <div className="text-xs text-sky-300 mt-1">{t("myPaymentsPage.info.plisioNoReceipt")}</div>}
                </div>

                {receiptUrl && (
                    <div className="mt-4">
                        <p className="text-xs text-gray-400 mb-2">{t("myPaymentsPage.alreadyUploaded")}:</p>
                        <div className="rounded-lg overflow-hidden border border-gray-700">
                            <img src={receiptUrl} alt="Receipt" className="w-full max-h-[50vh] object-contain" loading="lazy"/>
                        </div>
                    </div>
                )}

                {allowUpload && (
                    <div className="mt-4 space-y-2 max-h-screen">
                        <label className="block text-sm mb-1 font-semibold">
                            {t("transactionsPage.uploadReceiptLabel")}
                        </label>

                        {/* Dropzone فقط وقتی فایلی انتخاب نشده */}
                        {!file && (
                            <label
                                className="flex flex-col items-center justify-center gap-2 p-4 rounded-lg border-2 border-dashed cursor-pointer transition border-gray-700 bg-gray-800/40 hover:border-cyan-400 hover:bg-cyan-400/10"
                                onClick={() => { // قبل از باز شدن دیالوگ انتخاب فایل، ورودی را خالی کن
                                    if (fileInputRef.current) fileInputRef.current.value = "";
                                }}
                            >
                                <input
                                    key={inputKey}
                                    ref={fileInputRef}
                                    type="file"
                                    accept="image/*"
                                    onChange={(e)=>onPickFile(e.target.files?.[0] ?? null)}
                                    className="hidden"
                                />
                                <FaUpload />
                                <span className="text-sm text-gray-300">{t("transactionsPage.chooseImage")}</span>
                                <span className="text-xs text-gray-500">PNG / JPG / JPEG — ≤ 5MB</span>
                            </label>
                        )}

                        {/* وقتی فایل هست: فقط پیش‌نمایش + اکشن‌ها */}
                        {file && preview && (
                            <>
                                <div className="rounded-lg overflow-hidden border border-gray-700 ">
                                    <img src={preview} alt="preview" className="w-full max-h-[30vh] object-contain"/>
                                </div>
                                <div className="flex items-center justify-end gap-2">
                                    <button
                                        onClick={resetInput}
                                        className="px-3 py-2 rounded-lg bg-gray-700 hover:bg-gray-600 text-white inline-flex items-center gap-2 text-sm"
                                    >
                                        <FaTrashAlt /> {t("common.remove")}
                                    </button>
                                    <a
                                        href={preview}
                                        target="_blank"
                                        rel="noreferrer"
                                        className="px-3 py-2 rounded-lg bg-gray-700 hover:bg-gray-600 text-white inline-flex items-center gap-2 text-sm"
                                    >
                                        <FaEye /> {t("common.preview")}
                                    </a>
                                </div>
                            </>
                        )}

                        {error && <p className="text-rose-400 text-sm">{error}</p>}

                        <div className="flex items-center justify-end gap-2">
                            <button onClick={onClose} className="px-4 py-2 rounded-lg bg-gray-700 hover:bg-gray-600 text-white font-semibold">
                                {t("buttons.cancel")}
                            </button>
                            <button
                                disabled={!file || uploading}
                                onClick={onSend}
                                className="px-4 py-2 rounded-lg bg-blue-600 hover:bg-blue-700 text-white font-semibold disabled:opacity-40 inline-flex items-center gap-2"
                            >
                                {uploading && <FaSpinner className="animate-spin" />}
                                {uploading ? t("transactionsPage.sending") : t("transactionsPage.sendReceipt")}
                            </button>
                        </div>
                        {sendError && <p className="text-rose-400 text-sm">{sendError}</p>}
                    </div>
                )}
            </div>
        </div>
    );
};
