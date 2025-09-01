import {useMemo, useRef, useState} from 'react';
import {useQuery} from '@tanstack/react-query';
import {useNavigate} from 'react-router-dom';
import {Loader2, Send, Copy, ArrowLeft, ArrowRight} from 'lucide-react';
import {useTranslation} from 'react-i18next';

import type {
    BroadcastMessageRequest,
    BroadcastMessageResponse,
} from "../../models/sendMessage/SendMessage.ts";
import {apiBroadcastMessage} from "../../api/sendMessage/SendMessage.ts";
import {useDirection} from "../../hooks/useDirection.ts";

const MAX_LEN = 4096;
const cn = (...v: Array<string | false | undefined>) => v.filter(Boolean).join(' ');

function useNumberFormat() {
    const {i18n} = useTranslation();
    const locale = i18n.language?.startsWith('fa') ? 'fa-IR' : 'en-US';
    const fmt = new Intl.NumberFormat(locale);
    return (n: number) => fmt.format(n);
}

export default function SendMessage() {
    const navigate = useNavigate();
    const {t} = useTranslation();
    const dir = useDirection();
    const formatNumber = useNumberFormat();
    const BackIcon = dir.isRtl ? ArrowRight : ArrowLeft;

    const [text, setText] = useState('');
    const [dirty, setDirty] = useState(false);
    const [attempted, setAttempted] = useState(false);
    const inputRef = useRef<HTMLTextAreaElement | null>(null);

    const payload: BroadcastMessageRequest = useMemo(() => ({
        text,
        userIds: null,
    }), [text]);

    const textError =
        text.trim().length === 0
            ? t('sendMessagePage.validation.required')
            : text.length > MAX_LEN
                ? t('sendMessagePage.validation.maxLength', {max: MAX_LEN})
                : '';

    const showError = (dirty || attempted) && Boolean(textError);

    const canSend = !textError;

    const {refetch: send, isFetching, data, error, isError} =
        useQuery<BroadcastMessageResponse>({
            queryKey: ['broadcast-all', payload],
            queryFn: () => apiBroadcastMessage(payload),
            enabled: false,
            retry: false,
        });

    async function handleSend() {
        setAttempted(true);
        if (!canSend) {
            inputRef.current?.focus();
            return;
        }
        await send();
    }

    async function copyFailed() {
        if (!data?.data?.failedUserIds?.length) return;
        await navigator.clipboard.writeText(data.data.failedUserIds.join(','));
    }

    return (


        <div
            className={dir.classes('from-gray-950 via-gray-900 to-black p-4 text-gray-100 flex items-center justify-center', dir.isRtl)}>
            <div className="mx-auto w-full max-w-3xl">
                <div className={cn("mb-6 flex", dir.isRtl ? "justify-start" : "justify-start")}>
                    <button
                        onClick={() => navigate('/')}
                        className={cn(
                            "inline-flex items-center gap-2 text-sm text-indigo-400 hover:text-indigo-300 transition-colors",
                            dir.isRtl && "flex-row"
                        )}
                        aria-label={t('sendMessagePage.backToHome')}
                    >
                        <BackIcon className=" size-4 "/>
                        {t('sendMessagePage.backToHome')}
                    </button>
                </div>

                <div
                    className="relative rounded-3xl border border-gray-700 bg-gray-800/60 backdrop-blur-0 p-6 shadow-xl shadow-indigo-900/10">
                    <header className="mb-5 border-b border-gray-700/50 pb-4">
                        <h1 className="text-2xl font-bold text-gray-50">{t('sendMessagePage.title')}</h1>
                        <p className="mt-2 text-sm text-gray-400">{t('sendMessagePage.subtitle')}</p>
                    </header>

                    <label htmlFor="message-text" className="mb-2 block text-sm font-medium text-gray-300">
                        {t('sendMessagePage.labelText')}
                    </label>
                    <div
                        className="rounded-2xl border border-gray-700 bg-gray-900/50 p-2 shadow-inner shadow-gray-900/20">
            <textarea
                id="message-text"
                ref={inputRef}
                value={text}
                onChange={(e) => {
                    setText(e.target.value);
                    if (!dirty) setDirty(true);
                }}
                rows={8}
                placeholder={t('sendMessagePage.placeholder') || ''}
                className={cn(
                    'w-full resize-y rounded-xl p-3 bg-transparent text-gray-100 placeholder-gray-500 outline-none transition-all duration-200',
                    'focus:ring-2 focus:ring-offset-2 focus:ring-offset-gray-900',
                    showError ? 'ring-red-500 focus:ring-red-500' : 'ring-indigo-500 focus:ring-indigo-500'
                )}
            />
                        <div className="mt-2 flex items-center justify-between px-2 text-xs">
                            <span className={cn('font-medium', showError ? 'text-red-400' : 'text-gray-400')}
                                  id="message-text-error">
                                {showError
                                    ? textError
                                    : t('sendMessagePage.charCount', {
                                        current: formatNumber(text.length),
                                        max: formatNumber(MAX_LEN)
                                    })}
                            </span>
                        </div>
                    </div>

                    <div className="mt-4 flex items-center justify-end gap-4">
                        <button
                            onClick={() => setText('')}
                            disabled={isFetching || text.length === 0}
                            className={cn(
                                'rounded-xl px-5 py-2.5 text-sm font-medium transition-colors duration-200',
                                text.length === 0 || isFetching
                                    ? 'cursor-not-allowed text-gray-600'
                                    : 'text-gray-300 hover:text-white hover:bg-gray-700/50'
                            )}
                        >
                            {t('sendMessagePage.buttonClear')}
                        </button>

                        <button
                            onClick={handleSend}
                            disabled={!canSend || isFetching}
                            className={cn(
                                'inline-flex items-center gap-2 rounded-xl px-6 py-3 text-sm font-semibold transition-all duration-300',
                                !canSend || isFetching
                                    ? 'cursor-not-allowed bg-gray-700 text-gray-500'
                                    : 'bg-indigo-600 text-white shadow-lg shadow-indigo-500/30 hover:bg-indigo-700 active:scale-95'
                            )}
                        >
                            {isFetching ? <Loader2 className="size-5 animate-spin"/> : <Send className="size-5"/>}
                            {t('sendMessagePage.buttonSend')}
                        </button>
                    </div>

                    <section className="mt-4 pt-3 border-t border-gray-700/50">
                        <h3 className="mb-4 text-lg font-semibold text-gray-50">
                            {t('sendMessagePage.resultsTitle')}
                        </h3>

                        {!data && !isError && !isFetching && (
                            <p className="text-sm text-gray-400">{t('sendMessagePage.resultsPlaceholder')}</p>
                        )}

                        {isFetching && (
                            <div className="flex items-center gap-3 text-base text-indigo-400">
                                <Loader2 className="size-5 animate-spin"/>
                                {t('sendMessagePage.sending')}
                            </div>
                        )}

                        {isError && (
                            <div
                                className="rounded-xl border border-red-500 bg-red-900/30 p-4 text-sm text-red-300 shadow-md">
                                <p className="font-semibold mb-1">{t('sendMessagePage.errorTitle')}</p>
                                <p>
                                    {(error as any)?.response?.data?.message ||
                                        (error as any)?.message ||
                                        t('sendMessagePage.errorDefault')}
                                </p>
                            </div>
                        )}

                        {data && (
                            <div className="grid gap-4 md:grid-cols-3">
                                <div
                                    className="rounded-2xl border border-gray-700 bg-gray-800/70 p-4 text-center shadow-md">
                                    <div className="text-xs text-gray-400">{t('sendMessagePage.stats.total')}</div>
                                    <div className="mt-2 text-xl font-bold text-gray-50">
                                        {formatNumber(data.data?.totalTargets ?? 0)}
                                    </div>
                                </div>
                                <div
                                    className="rounded-2xl border border-emerald-500 bg-emerald-900/30 p-4 text-center shadow-md">
                                    <div className="text-xs text-emerald-400">{t('sendMessagePage.stats.sent')}</div>
                                    <div className="mt-2 text-xl font-bold text-emerald-300">
                                        {formatNumber(data.data?.sent ?? 0)}
                                    </div>
                                </div>
                                <div
                                    className="rounded-2xl border border-amber-500 bg-amber-900/30 p-4 text-center shadow-md">
                                    <div className="text-xs text-amber-400">{t('sendMessagePage.stats.failed')}</div>
                                    <div className="mt-2 text-xl font-bold text-amber-300">
                                        {formatNumber(data.data?.failed ?? 0)}
                                    </div>
                                </div>

                                {!!data.data?.failedUserIds?.length && (
                                    <div
                                        className="md:col-span-3 rounded-2xl border border-amber-500 bg-amber-900/30 p-4 shadow-md">
                                        <div className="mb-3 flex items-center justify-between">
                                            <div className="text-sm font-medium text-amber-300">
                                                {t('sendMessagePage.failedIdsTitle')}
                                            </div>
                                            <button
                                                onClick={copyFailed}
                                                className="inline-flex items-center gap-1.5 rounded-lg bg-amber-600 px-3 py-1.5 text-xs text-white shadow-sm hover:bg-amber-700 active:scale-95 transition-all duration-200"
                                            >
                                                <Copy className="size-4"/> {t('sendMessagePage.buttonCopyIds')}
                                            </button>
                                        </div>
                                        <div
                                            className="max-h-32 overflow-auto whitespace-pre-wrap break-words text-xs text-amber-200 bg-amber-950/50 p-2 rounded-lg border border-amber-800">
                                            {data.data.failedUserIds.join(', ')}
                                        </div>
                                    </div>
                                )}
                            </div>
                        )}
                    </section>
                </div>
            </div>
        </div>
    );
}
