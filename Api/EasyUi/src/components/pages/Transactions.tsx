import React, {useMemo, useState} from 'react';
import {useQuery} from '@tanstack/react-query';
import {useTranslation} from 'react-i18next';
import {useDirection} from '../../hooks/useDirection';
import {FaFilter, FaChevronDown, FaUndo, FaArrowUp, FaArrowDown, FaClock, FaTag} from 'react-icons/fa';
import type {GetTransactionsParams, PagedResult, Transaction} from '../../models/transactions/transactions.ts';
import {getTransactions} from '../../api/transactions/transactions.ts';
import DateFilter from '../filters/DateFilter.tsx';

const takeDefault = 10;

const parseBackendIso = (s?: string | number | null) => {
    if (s == null) return null;
    if (typeof s === 'number') return new Date(s);
    const hasTZ = /[zZ]|([+\-]\d{2}:?\d{2})$/.test(s);
    return new Date(hasTZ ? s : s + 'Z');
};

const formatInTehran = (iso?: string | number, lang?: string) => {
    const d = typeof iso === 'string' || typeof iso === 'number' ? parseBackendIso(iso) : (iso as any);
    if (!d) return '-';
    const isFa = (lang ?? '').toLowerCase().startsWith('fa');
    const locale = isFa ? 'fa-IR-u-ca-persian' : (lang || 'en-US');
    try {
        return new Intl.DateTimeFormat(locale, {
            year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit', hour12: false,
            timeZone: 'Asia/Tehran',
        }).format(d as Date);
    } catch {
        return new Intl.DateTimeFormat('en-US', {
            year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit', hour12: false,
            timeZone: 'Asia/Tehran',
        }).format(d as Date);
    }
};

const TX_TYPES = ['Credit', 'Debit'] as const;
const TX_REASONS = ['TopUp', 'ServiceUsage', 'Refund', 'ManualCredit', 'ManualDebit'] as const;

const useTxI18n = () => {
    const {t} = useTranslation();
    const typeLabel = (v?: string) => (v === 'Credit' ? t('txType.credit') : v === 'Debit' ? t('txType.debit') : v ?? '-');
    const reasonLabel = (v?: string) => {
        switch (v) {
            case 'TopUp':
                return t('txReason.topUp');
            case 'ServiceUsage':
                return t('txReason.serviceUsage');
            case 'Refund':
                return t('txReason.refund');
            case 'ManualCredit':
                return t('txReason.manualCredit');
            case 'ManualDebit':
                return t('txReason.manualDebit');
            default:
                return v ?? '-';
        }
    };
    return {typeLabel, reasonLabel};
};

const Transactions: React.FC = () => {
    const {t, i18n} = useTranslation();
    const isFa = (i18n.language ?? '').toLowerCase().startsWith('fa');
    const dir = useDirection();
    const {typeLabel, reasonLabel} = useTxI18n();

    const [page, setPage] = useState<number>(1);
    const [type, setType] = useState<string>('');
    const [reason, setReason] = useState<string>('');
    const [dateFrom, setDateFrom] = useState<string>('');
    const [dateTo, setDateTo] = useState<string>('');
    const [dateFromValue, setDateFromValue] = useState<Date | null>(null);
    const [dateToValue, setDateToValue] = useState<Date | null>(null);
    const [isFiltersOpen, setIsFiltersOpen] = useState<boolean>(false);

    const filters: GetTransactionsParams = useMemo(() => ({
        Type: type || undefined,
        Reason: reason || undefined,
        DateFrom: dateFrom || undefined,
        DateTo: dateTo || undefined,
        Skip: (page - 1) * takeDefault,
        Take: takeDefault,
        OrderBy: 'timestamp',
        OrderDir: 'desc',
    }), [type, reason, dateFrom, dateTo, page]);

    const {data, isLoading, isError, isFetching} = useQuery<PagedResult<Transaction>>({
        queryKey: ['transactions', filters],
        queryFn: () => getTransactions(filters),
        staleTime: 30_000,
        refetchOnWindowFocus: false,

    });

    const total = data?.totalCount ?? 0;
    const totalPages = Math.max(1, Math.ceil(total / takeDefault));

    const handleReset = () => {
        setType('');
        setReason('');
        setDateFrom('');
        setDateTo('');
        setDateFromValue(null);
        setDateToValue(null);
        setPage(1);

    };

    const handlePageChange = (p: number) => {
        setPage(p);
        window.scrollTo({top: 0, behavior: 'smooth'});
    };

    return (

        <div className="container mx-auto px-3 sm:px-4 py-6 sm:py-8">
            {isFetching && <div className="fixed top-0 left-0 right-0 h-1 bg-cyan-400 z-50 animate-pulse"/>}

            <div
                className={dir.classes('flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3 mb-5 sm:mb-6', dir.isRtl && 'sm:flex-row-reverse')}>
                <h1 className="text-2xl sm:text-3xl font-bold text-white truncate">{t('transactionsPage.title')}</h1>

            </div>

            <button
                onClick={() => setIsFiltersOpen(!isFiltersOpen)}
                className="w-full flex justify-between items-center px-3 sm:px-4 py-3 bg-gray-800/80 hover:bg-gray-800 rounded-lg font-semibold text-white transition-colors mb-3"
            >
                <span
                    className="flex items-center gap-2 whitespace-nowrap overflow-hidden text-ellipsis"><FaFilter/>{t('transactionsPage.filters.title')}</span>
                <i className={'transition-transform ' + (isFiltersOpen ? 'rotate-180' : '')}><FaChevronDown/></i>
            </button>

            {isFiltersOpen && (
                <div className="bg-gray-800/50 backdrop-blur-sm border border-gray-700/60 rounded-xl p-3 sm:p-4 mb-5">
                    <div className={dir.classes('grid gap-3 grid-cols-2 md:grid-cols-5', dir.isRtl && 'text-right')}>
                        <div className="col-span-2 md:col-span-1">
                            <label
                                className="block text-xs sm:text-sm text-gray-400 mb-1">{t('transactionsPage.filters.type')}</label>
                            <select value={type} onChange={(e) => {
                                setType(e.target.value);
                                setPage(1);
                            }}
                                    className="w-full px-3 py-2 rounded-lg bg-gray-900/60 border border-gray-700 text-sm text-white focus:ring-2 focus:ring-cyan-500 outline-none">
                                <option value="">{t('common.all', {defaultValue: 'All'})}</option>
                                {TX_TYPES.map(v => <option key={v} value={v}>{typeLabel(v)}</option>)}
                            </select>
                        </div>
                        <div className="col-span-2 md:col-span-1">
                            <label
                                className="block text-xs sm:text-sm text-gray-400 mb-1">{t('transactionsPage.filters.reason')}</label>
                            <select value={reason} onChange={(e) => {
                                setReason(e.target.value);
                                setPage(1);
                            }}
                                    className="w-full px-3 py-2 rounded-lg bg-gray-900/60 border border-gray-700 text-sm text-white focus:ring-2 focus:ring-cyan-500 outline-none">
                                <option value="">{t('common.all', {defaultValue: 'All'})}</option>
                                {TX_REASONS.map(v => <option key={v} value={v}>{reasonLabel(v)}</option>)}
                            </select>
                        </div>
                        <div>
                            <label
                                className="block text-xs sm:text-sm text-gray-400 mb-1">{t('filters.dateFrom')}</label>
                            <DateFilter
                                locale={isFa ? 'fa' : 'en'}
                                value={dateFromValue}
                                type="from"
                                onChange={(utc) => {
                                    setDateFromValue(utc ? new Date(utc) : null);
                                    setDateFrom(utc || '');
                                    setPage(1);
                                }}
                            />
                        </div>
                        <div>
                            <label className="block text-xs sm:text-sm text-gray-400 mb-1">{t('filters.dateTo')}</label>
                            <DateFilter
                                locale={isFa ? 'fa' : 'en'}
                                value={dateToValue}
                                type="to"
                                onChange={(utc) => {
                                    setDateToValue(utc ? new Date(utc) : null);
                                    setDateTo(utc || '');
                                    setPage(1);
                                }}
                            />
                        </div>
                        <div className={dir.classes('col-span-2 md:col-span-1 flex items-end', dir.isRtl)}>
                            <button onClick={handleReset}
                                    className="w-full flex items-center justify-center gap-2 px-3 sm:px-4 py-2.5 rounded-lg bg-gray-600 hover:bg-gray-700 text-white font-semibold transition-colors">
                                <FaUndo/>{t('filters.resetButton')}
                            </button>
                        </div>
                    </div>

                    {(type || reason || dateFrom || dateTo) && (
                        <div className="flex flex-wrap items-center gap-2 mt-3">
                            {type && <FilterChip label={`${t('transactionsPage.filters.type')}: ${typeLabel(type)}`}
                                                 onClear={() => setType('')}/>}
                            {reason &&
                                <FilterChip label={`${t('transactionsPage.filters.reason')}: ${reasonLabel(reason)}`}
                                            onClear={() => setReason('')}/>}
                            {dateFrom && <FilterChip
                                label={`${t('filters.dateFrom')}: ${formatInTehran(dateFrom, i18n.language)}`}
                                onClear={() => {
                                    setDateFrom('');
                                    setDateFromValue(null);
                                }}/>}
                            {dateTo &&
                                <FilterChip label={`${t('filters.dateTo')}: ${formatInTehran(dateTo, i18n.language)}`}
                                            onClear={() => {
                                                setDateTo('');
                                                setDateToValue(null);
                                            }}/>}
                        </div>
                    )}
                </div>
            )}

            {/* List */}
            {isFetching && <div
                className="self-start sm:self-auto w-5 h-5 border-2 border-cyan-400 border-t-transparent justify-self-center rounded-full animate-spin"/>}
            {isError && <p className="text-center text-red-500">{t('transactionsPage.error')}</p>}

            <div className="grid gap-3">
                {data?.items?.length ? (
                    data.items.map((tx) => (
                        <div key={tx.id}
                             className="relative bg-gray-900/60 backdrop-blur border border-white/10 rounded-2xl p-4 sm:p-5 hover:border-cyan-400/30 transition-colors">
                            <div className={dir.classes('flex items-start justify-between gap-3', dir.isRtl)}>
                                <div className={dir.classes('min-w-0 space-y-1.5', dir.isRtl && 'text-right')}>
                                    <div className="flex items-center gap-2 flex-wrap">
                                        <TxBadge type={tx.type} t={t}/>
                                        <ReasonPill label={reasonLabel(tx.reason)}/>
                                        <span
                                            className="inline-flex items-center gap-1 text-[11px] sm:text-xs text-gray-400">
                      <FaClock className="opacity-70"/>
                      <span className="font-mono">{formatInTehran(tx.timestamp as any, i18n.language)}</span>
                    </span>
                                    </div>

                                    {tx.description && (
                                        <div className="text-sm text-gray-300 line-clamp-3"
                                             title={tx.description as any}>{tx.description}</div>
                                    )}
                                </div>
                                <AmountBlock amount={tx.amount} type={tx.type} before={tx.balanceBefore}
                                             after={tx.balanceAfter} t={t} dir={dir}/>
                            </div>
                        </div>
                    ))
                ) : (
                    !isLoading && !isError && (
                        <div className="text-center py-10 px-6 bg-gray-800/20 rounded-lg">
                            <i className="fas fa-inbox text-4xl text-gray-500 mb-4"/>
                            <h3 className="font-bold text-lg text-white">{t('transactionsPage.empty.title')}</h3>
                            <p className="text-gray-400">{t('transactionsPage.empty.description')}</p>
                        </div>
                    )
                )}
            </div>

            {total > 0 && (
                <Pagination page={page} totalPages={totalPages} onPageChange={handlePageChange} totalItems={total}
                            isBusy={isFetching} pageSize={takeDefault} t={t}/>
            )}
        </div>
    );
};

const TxBadge: React.FC<{ type?: string; t: (k: string) => string }> = ({type, t}) => {
    const isCredit = type === 'Credit';
    const cls = isCredit ? 'bg-emerald-500/12 text-emerald-300 ring-emerald-400/25' : 'bg-rose-500/12 text-rose-300 ring-rose-400/25';
    const Icon = isCredit ? FaArrowUp : FaArrowDown;
    return (
        <span
            className={`inline-flex items-center gap-1.5 rounded-full px-2.5 py-1 text-xs font-semibold ring-1 ${cls}`}
            title={t(isCredit ? 'txType.credit' : 'txType.debit')}>
      <Icon className="text-[12px]"/>{t(isCredit ? 'txType.credit' : 'txType.debit')}
    </span>
    );
};

const ReasonPill: React.FC<{ label?: string }> = ({label}) => (
    <span
        className="inline-flex items-center gap-1 rounded-lg bg-white/5 ring-1 ring-white/10 px-2 py-1 text-[11px] sm:text-xs text-gray-300">
    <FaTag className="text-[10px] opacity-80"/>{label ?? '-'}
  </span>
);

const AmountBlock: React.FC<{
    amount: number | string;
    type?: string;
    before?: number | string;
    after?: number | string;
    t: (k: string, o?: any) => string;
    dir: ReturnType<typeof useDirection>;
}> = ({amount, type, before, after, t, dir}) => {
    const isCredit = type === 'Credit';
    const sign = isCredit ? '+' : '-';
    const color = isCredit ? 'text-emerald-400' : 'text-rose-400';
    const val = Number(Math.abs(Number(amount))).toLocaleString();
    return (
        <div className={dir.classes('text-right shrink-0', dir.isRtl && 'text-left')}>
            <p className={`font-mono text-base sm:text-lg whitespace-nowrap ${color}`}>{sign}{val}</p>
            <p className="text-[11px] sm:text-xs text-gray-400 whitespace-nowrap">
                {t('transactionsPage.card.balanceInfo', {
                    before: Number(before ?? 0).toLocaleString(),
                    after: Number(after ?? 0).toLocaleString(),
                })}
            </p>
        </div>
    );
};

const Pagination: React.FC<{
    page: number;
    totalPages: number;
    onPageChange: (p: number) => void;
    totalItems: number;
    isBusy?: boolean;
    pageSize: number;
    t: (k: string, o?: any) => string;
}> = ({page, totalPages, onPageChange, totalItems, isBusy, pageSize, t}) => {
    const from = totalItems === 0 ? 0 : (page - 1) * pageSize + 1;
    const to = Math.min(page * pageSize, totalItems);
    return (
        <div className="mt-6 flex flex-col sm:flex-row items-center justify-between gap-3 sm:gap-4">
            <p className="text-gray-400 text-xs sm:text-sm">{t('pagination.info', {from, to, total: totalItems})}</p>
            <div className="flex items-center gap-2">
                <button disabled={page <= 1 || !!isBusy} onClick={() => onPageChange(page - 1)}
                        className="px-3 py-2 rounded-lg bg-gray-700/60 text-white disabled:opacity-50 text-sm">{t('pagination.prev')}</button>
                <span className="text-gray-300 text-sm font-mono">{page} / {totalPages}</span>
                <button disabled={page >= totalPages || !!isBusy} onClick={() => onPageChange(page + 1)}
                        className="px-3 py-2 rounded-lg bg-gray-700/60 text-white disabled:opacity-50 text-sm">{t('pagination.next')}</button>
            </div>
        </div>
    );
};

const FilterChip: React.FC<{ label: string; onClear: () => void }> = ({label, onClear}) => (
    <button onClick={onClear}
            className="inline-flex items-center gap-1 rounded-full bg-white/5 ring-1 ring-white/10 px-2.5 py-1 text-xs text-gray-200 hover:bg-white/10 transition"
            title={label}>
        <span className="truncate max-w-[16rem]">{label}</span>
        <span className="ml-1 rounded-full bg-white/10 px-1.5">×</span>
    </button>

);

export default Transactions;
