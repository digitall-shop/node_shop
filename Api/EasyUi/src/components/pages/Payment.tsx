import React, {useCallback, useMemo, useState} from "react";
import {AnimatePresence, motion} from "framer-motion";
import {useMutation, type UseMutationResult} from '@tanstack/react-query';
import confetti from 'canvas-confetti';
import {useTranslation} from "react-i18next";
import {useDirection} from "../../hooks/useDirection.ts";
import type {
    AcceptPaymentPayload,
    AcceptPaymentResponse,
    NewPaymentResponseData
} from "../../models/peyment/payment.ts";
import {acceptPaymentRequest, addPaymentRequest} from "../../api/peyment/payment.ts";
import {bankBinList} from "../data/bankBinList.ts";


const formatNumber = (value: string | number): string => {
    if (!value) return "";
    const stringValue = String(value).replace(/[^0-9]/g, '');
    if (stringValue === "") return "";
    return new Intl.NumberFormat('en-US').format(Number(stringValue));
};

const findBankInfo = (cardNumber: string | undefined) => {
    if (!cardNumber || cardNumber.length < 6) return null;
    const bin = cardNumber.substring(0, 6);
    return bankBinList.find(b => b.bin === bin) || null;
};

const fireConfetti = () => {
    confetti({
        particleCount: 150,
        spread: 180,
        origin: {y: 0.6}
    });
};

const PlisioLoadingStep = () => {
    const {t} = useTranslation();
    return (
        <motion.div
            key="plisio-loading"
            initial={{opacity: 0, scale: 0.8}}
            animate={{opacity: 1, scale: 1}}
            exit={{opacity: 0, scale: 0.8}}
            transition={{duration: 0.4, ease: "backOut"}}
            className="flex flex-col items-center justify-center space-y-5 h-64 text-center"
        >
            <div className="relative flex items-center justify-center w-24 h-24">
                {[...Array(3)].map((_, i) => (
                    <motion.div
                        key={i}
                        className="absolute border-4 border-cyan-500 rounded-full"
                        style={{borderColor: '#22d3ee'}}
                        initial={{opacity: 0, scale: 0}}
                        animate={{
                            opacity: [0.5, 1, 0.5],
                            scale: [1, 1.5, 1],
                            width: ['40%', '80%', '40%'],
                            height: ['40%', '80%', '40%'],
                        }}
                        transition={{
                            duration: 2,
                            ease: "easeInOut",
                            repeat: Infinity,
                            delay: i * 0.3,
                        }}
                    />
                ))}
                <span className="text-3xl">🌐</span>
            </div>
            <h3 className="text-lg font-semibold text-cyan-300">{t('transactionsPage.loading.creatingGateway')}</h3>
            <p className="text-sm text-gray-400">{t('transactionsPage.loading.pleaseWait')}</p>
        </motion.div>
    );
};


const AmountInputSection = ({
                                amount,
                                amountError,
                                paymentMutation,
                                handleAmountChange,
                                formattedAmountDisplay,
                                onSubmit
                            }: {
    amount: string;
    amountError: string | null;
    paymentMutation: UseMutationResult<NewPaymentResponseData | null, Error, { amount: number; method: 0 | 1; }>;
    handleAmountChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
    formattedAmountDisplay: string | null;
    onSubmit: () => void;
}) => {
    const {t} = useTranslation();
    const {styles} = useDirection();
    return (
        <div className="space-y-4">
            <div className="space-y-2">
                <p className={`text-gray-200 text-sm ${styles.text.start}`}>{t('transactionsPage.enterAmount')}</p>
                <input
                    type="text"
                    inputMode="numeric"
                    value={formatNumber(amount)}
                    onChange={handleAmountChange}
                    placeholder={t('transactionsPage.amountPlaceholder')}
                    className="w-full px-4 py-2 rounded bg-gray-700 text-white focus:outline-none text-left"
                    dir="ltr"
                    autoFocus
                />
                {formattedAmountDisplay && (
                    <p className={`text-xs text-green-400 pr-1 ${styles.text.start}`}>
                        {t('transactionsPage.paymentAmountLabel', {amount: formattedAmountDisplay})}
                    </p>
                )}
                {amountError && <p className="text-red-400 text-sm">{amountError}</p>}
                {paymentMutation.isError && <p className="text-red-400 text-sm">{paymentMutation.error.message}</p>}
            </div>
            <div className="flex justify-end">
                <button onClick={onSubmit} disabled={!amount || paymentMutation.isPending}
                        className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 disabled:opacity-40 w-32">
                    {t('transactionsPage.nextStep')}
                </button>
            </div>
        </div>
    );
};

const SuccessStep = ({onReset}: { onReset: () => void; }) => {
    const {t} = useTranslation();
    return (
        <motion.div
            initial={{scale: 0.5, opacity: 0}}
            animate={{scale: 1, opacity: 1}}
            className="text-center space-y-4 flex flex-col items-center"
        >
            <motion.div initial={{scale: 0}} animate={{scale: 1}}
                        transition={{delay: 0.2, type: 'spring', stiffness: 200}}>
                <svg className="w-24 h-24 text-green-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
                          d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
            </motion.div>
            <h3 className="text-2xl font-bold text-green-300">{t('transactionsPage.success.title')}</h3>
            <p className="text-gray-300">{t('transactionsPage.success.message')}</p>
            <button
                onClick={onReset}
                className="bg-green-600 text-white font-bold py-2 px-6 rounded-lg hover:bg-green-700 transition-colors mt-4"
            >
                {t('buttons.close')}
            </button>
        </motion.div>
    );
};

const Payment: React.FC = () => {
    const {t} = useTranslation();
    const {direction, styles} = useDirection();

    const [activeModal, setActiveModal] = useState<"card" | "plisio" | null>(null);
    const [step, setStep] = useState(1);
    const [amount, setAmount] = useState("");
    const [amountError, setAmountError] = useState<string | null>(null);
    const [receipt, setReceipt] = useState<File | null>(null);
    const [copied, setCopied] = useState(false);
    const [receiptPreview, setReceiptPreview] = useState<string | null>(null);

    const paymentMutation = useMutation<
        NewPaymentResponseData | null,
        Error,
        { amount: number; method: 0 | 1; }
    >({
        mutationFn: addPaymentRequest,
        onSuccess: (data) => {
            if (data?.requiresRedirect || data?.displayData) {
                setStep(2);
            }
        },
    });

    const uploadReceiptMutation = useMutation<
        AcceptPaymentResponse | null,
        Error,
        AcceptPaymentPayload
    >({
        mutationFn: acceptPaymentRequest,
        onSuccess: () => {
            setStep(4);
            fireConfetti();
        }
    });

    const reset = useCallback(() => {
        setActiveModal(null);
        setTimeout(() => {
            setStep(1);
            setAmount("");
            setReceipt(null);
            if (receiptPreview) {
                URL.revokeObjectURL(receiptPreview);
            }
            setCopied(false);
            setAmountError(null);
            paymentMutation.reset();
            uploadReceiptMutation.reset();
        }, 300);
    }, [paymentMutation, uploadReceiptMutation, receiptPreview]);

    const validateAmount = (value: string): boolean => {
        const numericAmount = parseInt(value, 10);
        if (!value || isNaN(numericAmount)) {
            setAmountError(t('transactionsPage.validation.enterNumericAmount'));
            return false;
        }
        if (numericAmount < 1000) {
            setAmountError(t('transactionsPage.validation.minAmount', {amount: formatNumber(1000)}));
            return false;
        }
        setAmountError(null);
        return true;
    };

    const handlePaymentSubmit = (method: 0 | 1) => {
        const numericAmount = parseInt(amount.replace(/,/g, ''), 10);
        if (validateAmount(String(numericAmount))) {
            paymentMutation.mutate({amount: numericAmount, method});
        }
    };

    const handleAmountChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const rawValue = e.target.value.replace(/[^0-9]/g, '');
        setAmount(rawValue);
        if (amountError) {
            validateAmount(rawValue);
        }
    };

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0] || null;
        setReceipt(file);

        if (receiptPreview) {
            URL.revokeObjectURL(receiptPreview);
        }

        if (file) {
            setReceiptPreview(URL.createObjectURL(file));
        } else {
            setReceiptPreview(null);
        }
    };

    const handleSubmitReceipt = () => {
        const paymentRequestId = paymentMutation.data?.paymentRequestId;
        if (receipt && paymentRequestId) {
            uploadReceiptMutation.mutate({paymentRequestId, receipt});
        }
    };

    const handleCopy = () => {
        const cardNumber = paymentMutation.data?.displayData?.cardNumber;
        if (cardNumber) {
            navigator.clipboard.writeText(cardNumber.replace(/\s/g, ''));
            setCopied(true);
            setTimeout(() => setCopied(false), 2000);
        }
    };

    const cardData = paymentMutation.data?.displayData;
    const bankInfo = useMemo(() => findBankInfo(cardData?.cardNumber), [cardData]);
    const bankName = bankInfo?.name ?? t('transactionsPage.unknownBank');
    const gradient = bankInfo?.gradient ?? "from-gray-700 to-gray-500";
    const shimmerClass = bankInfo ? 'bg-[length:200%_100%] animate-shimmer' : '';

    const formattedAmountDisplay = useMemo(() => {
        const numericAmount = parseInt(amount, 10);
        if (numericAmount > 0) {
            return formatNumber(amount);
        }
        return null;
    }, [amount]);

    const getProgress = () => {
        if (activeModal === 'card') {
            if (step === 1) return '25%';
            if (step === 2) return '50%';
            if (step === 3) return '75%';
            return '100%';
        }
        if (activeModal === 'plisio') {
            if (step === 1 && !paymentMutation.isPending) return '33%';
            if (step === 1 && paymentMutation.isPending) return '66%';
            if (step === 2) return '100%';
        }
        return '0%';
    };

    const ModalContent = (
        <motion.div
            initial={{scale: 0.95, opacity: 0}}
            animate={{scale: 1, opacity: 1}}
            exit={{scale: 0.95, opacity: 0}}
            transition={{duration: 0.3}}
            className="bg-gray-900 bg-opacity-90 backdrop-blur-sm w-full max-w-md p-6 rounded-2xl shadow-2xl border border-gray-700 space-y-6"
            onClick={(e) => e.stopPropagation()}
        >
            <div className="flex justify-between items-center">
                <h2 className="text-xl font-bold">{activeModal === 'card' ? t('transactionsPage.cardModalTitle') : t('transactionsPage.plisioModalTitle')}</h2>
                <button onClick={reset}
                        className="text-gray-400 hover:text-white transition-colors">{t('buttons.close')}</button>
            </div>
            <div className="w-full bg-gray-700 rounded-full h-2">
                <motion.div
                    className={`h-2 rounded-full ${step === 4 || (activeModal === 'plisio' && step === 2)
                        ? 'bg-gradient-to-r from-green-500 to-emerald-500'
                        : 'bg-gradient-to-r from-blue-500 to-cyan-400'
                    }`}
                    initial={{width: '0%'}}
                    animate={{width: getProgress()}}
                    transition={{duration: 0.5, ease: "easeInOut"}}
                />
            </div>
            <AnimatePresence mode="wait">
                <motion.div
                    key={`${activeModal}-${step}-${paymentMutation.isPending}`}
                    initial={{x: 50, opacity: 0}}
                    animate={{x: 0, opacity: 1}}
                    exit={{x: -50, opacity: 0}}
                    transition={{duration: 0.3}}
                >
                    {activeModal === 'card' && (
                        <>
                            {step === 1 && (
                                <AmountInputSection
                                    amount={amount}
                                    amountError={amountError}
                                    paymentMutation={paymentMutation}
                                    handleAmountChange={handleAmountChange}
                                    formattedAmountDisplay={formattedAmountDisplay}
                                    onSubmit={() => handlePaymentSubmit(0)}
                                />
                            )}
                            {step === 2 && cardData && (
                                <div className="space-y-4">
                                    <motion.div
                                        whileHover={{scale: 1.02}}
                                        className={`relative w-full h-52 rounded-2xl overflow-hidden shadow-xl text-white px-6 py-4 flex flex-col justify-between bg-gradient-to-br ${gradient} ${shimmerClass}`}>
                                        <div className={styles.text.start}>
                                            <div
                                                className="text-sm font-semibold">{t('transactionsPage.cardHolderLabel')}: <span
                                                className="font-bold text-lg">{cardData.holderName}</span></div>
                                            <div className="mt-2 text-sm">{t('transactionsPage.bankLabel')}: <span
                                                className="font-semibold">{bankName}</span></div>
                                        </div>
                                        <div className="flex justify-between items-center" dir="ltr">
                                            <div
                                                className="text-xl font-semibold tracking-wider font-mono">{cardData.cardNumber}</div>
                                            <button onClick={handleCopy}
                                                    className="text-white text-lg bg-white bg-opacity-20 p-2 rounded-full hover:bg-opacity-40 transition"
                                                    title={t('transactionsPage.copyCardNumberTitle')}>
                                                <i className={`fas ${copied ? "fa-check" : "fa-clipboard"}`}/>
                                            </button>
                                        </div>
                                        {copied && <div
                                            className="absolute bottom-16 right-6 bg-black bg-opacity-60 text-xs text-white px-2 py-1 rounded">
                                            {t('transactionsPage.copied')}
                                        </div>}
                                    </motion.div>
                                    <div className="flex justify-between items-center mt-4">
                                        <button onClick={() => {
                                            setStep(1);
                                            paymentMutation.reset();
                                        }} className="text-sm text-gray-300 hover:text-white">
                                            {t('transactionsPage.previousStep')}
                                        </button>
                                        <button onClick={() => setStep(3)}
                                                className="bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700">
                                            {t('transactionsPage.iHavePaid')}
                                        </button>
                                    </div>
                                </div>
                            )}
                            {step === 3 && (
                                <div className="space-y-4">
                                    <label
                                        className={`text-gray-200 text-sm block ${styles.text.start}`}>{t('transactionsPage.uploadReceiptLabel')}</label>

                                    {receiptPreview && (
                                        <div className="my-3 p-2 border border-dashed border-gray-600 rounded-lg">
                                            <img
                                                src={receiptPreview}
                                                alt={t('transactionsPage.receiptPreviewAlt')}
                                                className="w-full max-h-48 object-contain rounded-md"
                                            />
                                        </div>
                                    )}

                                    <input type="file" accept="image/*"
                                           onChange={handleFileChange}
                                           className="w-full text-sm text-white bg-gray-700 rounded px-3 py-2 file:mr-4 file:py-2 file:px-4 file:rounded-full file:border-0 file:text-sm file:font-semibold file:bg-blue-50 file:text-blue-700 hover:file:bg-blue-100 cursor-pointer"/>
                                    {uploadReceiptMutation.isError &&
                                        <p className="text-red-400 text-sm">{uploadReceiptMutation.error.message}</p>}
                                    <div className="flex justify-between">
                                        <button onClick={() => setStep(2)}
                                                className="text-sm text-gray-300 hover:text-white">{t('transactionsPage.previousStep')}
                                        </button>
                                        <button disabled={!receipt || uploadReceiptMutation.isPending}
                                                onClick={handleSubmitReceipt}
                                                className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 disabled:opacity-30 w-32">
                                            {uploadReceiptMutation.isPending ?
                                                <span
                                                    className="animate-pulse">{t('transactionsPage.sending')}</span> : t('transactionsPage.sendReceipt')}
                                        </button>
                                    </div>
                                </div>
                            )}
                            {step === 4 && <SuccessStep onReset={reset}/>}
                        </>
                    )}
                    {activeModal === 'plisio' && (
                        <>
                            {step === 1 && (
                                paymentMutation.isPending ? (
                                    <PlisioLoadingStep/>
                                ) : (
                                    <AmountInputSection
                                        amount={amount}
                                        amountError={amountError}
                                        paymentMutation={paymentMutation}
                                        handleAmountChange={handleAmountChange}
                                        formattedAmountDisplay={formattedAmountDisplay}
                                        onSubmit={() => handlePaymentSubmit(1)}
                                    />
                                )
                            )}

                            {step === 2 && paymentMutation.data?.redirectUrl && (
                                <motion.div
                                    initial={{opacity: 0, y: 20}} animate={{opacity: 1, y: 0}}
                                    className="text-center space-y-4 p-4 border border-dashed border-green-500 rounded-lg"
                                >
                                    <div className="text-5xl animate-pulse">🌐</div>
                                    <h3 className="text-lg font-semibold text-green-400">{t('transactionsPage.gatewayReady.title')}</h3>
                                    <p className="text-lg text-blue-400">
                                        {t('transactionsPage.paymentAmountLabel', {amount: formattedAmountDisplay})}
                                    </p>
                                    <p className="text-gray-300 text-sm">{t('transactionsPage.gatewayReady.message')}</p>
                                    <a href={paymentMutation.data.redirectUrl} target="_blank" rel="noopener noreferrer"
                                       className="inline-block w-full bg-green-600 text-white font-bold py-3 px-6 rounded-lg hover:bg-green-700 transition-transform hover:scale-105">
                                        {t('transactionsPage.gatewayReady.button')}
                                    </a>
                                </motion.div>
                            )}
                        </>
                    )}
                </motion.div>
            </AnimatePresence>
        </motion.div>
    );

    return (
        <div className="max-w-xl mx-auto p-6 text-white" dir={direction}>
            <h1 className="text-2xl font-bold mb-2">{t('transactionsPage.pageTitle')}</h1>
            <p className="text-gray-400 mb-6">{t('transactionsPage.pageSubtitle')}</p>
            <div className="grid gap-6">
                <motion.div
                    whileHover={{scale: 1.03, y: -5, rotateX: '5deg'}}
                    transition={{type: 'spring', stiffness: 300}}
                    onClick={() => setActiveModal("card")}
                    className="cursor-pointer bg-gray-800 hover:bg-gray-700/80 p-5 rounded-xl border border-gray-700 transition-colors"
                    style={{transformStyle: 'preserve-3d'}}
                >
                    <div className="flex justify-between items-center">
                        <div>
                            <p className="text-lg font-semibold">{t('transactionsPage.cardToCard.title')}</p>
                            <p className="text-sm text-gray-400">{t('transactionsPage.cardToCard.subtitle')}</p>
                        </div>
                        <span className="text-3xl">💳</span>
                    </div>
                </motion.div>
                <motion.div
                    whileHover={{scale: 1.03, y: -5, rotateX: '5deg'}}
                    transition={{type: 'spring', stiffness: 300}}
                    onClick={() => setActiveModal("plisio")}
                    className="cursor-pointer bg-gray-800 hover:bg-gray-700/80 p-5 rounded-xl border border-gray-700 transition-colors"
                    style={{transformStyle: 'preserve-3d'}}
                >
                    <div className="flex justify-between items-center">
                        <div>
                            <p className="text-lg font-semibold">{t('transactionsPage.onlineGateway.title')}</p>
                            <p className="text-sm text-gray-400">{t('transactionsPage.onlineGateway.subtitle')}</p>
                        </div>
                        <span className="text-3xl">🌐</span>
                    </div>
                </motion.div>
            </div>
            <AnimatePresence>
                {activeModal && (
                    <motion.div
                        className="fixed inset-0 bg-black bg-opacity-70 flex items-center justify-center z-50 p-4"
                        initial={{opacity: 0}} animate={{opacity: 1}} exit={{opacity: 0}}
                        onClick={reset}
                    >
                        {ModalContent}
                    </motion.div>
                )}
            </AnimatePresence>
        </div>
    );
};

export default Payment;