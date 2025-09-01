import React, {useMemo} from "react";
import {useTranslation} from "react-i18next";
import {PaymentRequestStatus, statusLabel} from "../../models/paymentStatus";

const STATUS_STYLES: Record<number, { bg: string; text: string; ring: string }> = {
    [PaymentRequestStatus.Pending]:   { bg: "bg-slate-500/15",   text: "text-slate-300",   ring: "ring-slate-400/20" },
    [PaymentRequestStatus.Submitted]: { bg: "bg-amber-500/15",   text: "text-amber-300",   ring: "ring-amber-400/20" },
    [PaymentRequestStatus.Completed]: { bg: "bg-emerald-500/15", text: "text-emerald-300", ring: "ring-emerald-400/20" },
    [PaymentRequestStatus.Failed]:    { bg: "bg-rose-500/15",    text: "text-rose-300",    ring: "ring-rose-400/20" },
};

type Props = {
    status: number;
    size?: "sm" | "md";
    className?: string;
    withDot?: boolean;
};

const StatusBadge: React.FC<Props> = ({ status, size = "sm", className = "", withDot = false }) => {
    const { t } = useTranslation();

    const style = useMemo(() => {
        return STATUS_STYLES[status] ?? { bg: "bg-gray-500/15", text: "text-gray-300", ring: "ring-gray-400/20" };
    }, [status]);

    const pad = size === "sm" ? "px-2.5 py-0.5 text-[11px]" : "px-3 py-1 text-xs";

    return (
        <span
            className={`inline-flex items-center gap-1 rounded-full ${style.bg} ${style.text} ring-1 ${style.ring} ${pad} font-semibold ${className}`}
        >
      {withDot && <span className={`inline-block w-1.5 h-1.5 rounded-full ${style.text} opacity-70`} />}
            {t(statusLabel(status))}
    </span>
    );
};

export default StatusBadge;
