import {motion} from "framer-motion";
import {useTranslation} from "react-i18next";
import {useDirection} from "../../../hooks/useDirection"; // Path might need adjustment
import type {NodeStatus, Props} from "../../../models/nodes/node.ts";

const codeToFlag = (name?: string) => {
    const raw = (name ?? "").split("_")[0]?.slice(0, 2).toUpperCase();
    if (!raw || raw.length !== 2) return "🏳️";
    const base = 0x1f1e6;
    return String.fromCodePoint(
        base + (raw.charCodeAt(0) - 65),
        base + (raw.charCodeAt(1) - 65)
    );
};

const ServerNodeCard = ({
                            node,
                            footer,
                            isConnected = false,
                            onSelect,
                            selecting = false,
                            className = "",
                        }: Props) => {
    const {t, i18n} = useTranslation();
    const dir = useDirection();

    const STATUS: Record<NodeStatus, { label: string; dot: string; pill: string }> = {
        0: {
            label: t("panelNodesPage.serverCard.status.ready"),
            dot: "bg-emerald-400",
            pill: "bg-emerald-500/15 text-emerald-300"
        },
        1: {
            label: t("panelNodesPage.serverCard.status.inactive"),
            dot: "bg-gray-400",
            pill: "bg-gray-500/15 text-gray-300"
        },
        2: {
            label: t("panelNodesPage.serverCard.status.updating"),
            dot: "bg-amber-400",
            pill: "bg-amber-500/15 text-amber-300"
        },
    };

    const formatPrice = (n: number) => {
        const locale = i18n.language === 'fa' ? 'fa-IR' : 'en-US';
        return new Intl.NumberFormat(locale).format(n);
    };

    const s = STATUS[node.status];
    const canSelect = node.status === 0;

    return (
        <motion.div
            variants={{
                hidden: {opacity: 0, y: 20},
                visible: {opacity: 1, y: 0},
            }}
            whileHover={canSelect && !isConnected ? {y: -4, scale: 1.02} : {}}
            transition={{type: "spring", stiffness: 260, damping: 18}}
            className={dir.classes(
                `relative overflow-hidden rounded-2xl bg-gradient-to-b from-slate-800/70 to-slate-900/70
                 ring-1 ring-white/10 shadow-xl p-5 flex flex-col gap-4`,
                className,
                (!canSelect || isConnected) && "opacity-60"
            )}
        >
            <div className={dir.classes(
                "pointer-events-none absolute -top-16 h-40 w-40 rounded-full bg-cyan-500/10 blur-3xl",
                dir.className("-left-16", "-right-16") // RTL-aware positioning
            )}/>

            <div className="flex items-start justify-between gap-3">
                <div className="flex items-center gap-3">
                    <div className="h-10 w-10 rounded-2xl bg-slate-700/60 flex items-center justify-center text-xl">
                        {codeToFlag(node?.nodeName)}
                    </div>
                    <div>
                        <div className="text-white font-bold text-lg">{node.nodeName}</div>
                        <div className="text-sm text-slate-400">{t("panelNodesPage.serverCard.dedicatedServer")}</div>
                    </div>
                </div>

                <span
                    className={`inline-flex items-center gap-2 px-3 py-1 rounded-full text-xs font-medium ${s.pill}`}
                >
                    <span className={`h-2 w-2 rounded-full ${s.dot}`}/>
                    {s.label}
                </span>
            </div>

            <div className="flex items-center justify-between border-t border-white/10 pt-4 mt-auto">
                <div className="text-slate-400 text-sm">{t("panelNodesPage.serverCard.pricePerGig")}</div>
                <div className="text-white  font-extrabold tracking-wide">
                    {formatPrice(node.price)}
                    <span className={dir.classes(
                        "text-sm font-normal text-slate-400 pl-1 pr-1",
                        dir.styles.margin.start(1)
                    )}>
                        {t("panelNodesPage.serverCard.currency")}
                    </span>
                </div>
            </div>

            <div className="flex justify-end">
                {footer ? (
                    footer
                ) : isConnected ? (
                    <div
                        className="flex items-center gap-2 text-green-400 text-sm font-semibold px-3 py-1.5 rounded-lg bg-green-500/10">
                        <i className="fas fa-check-circle"/>
                        <span>{t("panelNodesPage.serverCard.alreadyActive")}</span>
                    </div>
                ) : (
                    <button
                        type="button"
                        onClick={onSelect}
                        disabled={selecting || !canSelect}
                        className="w-full px-3 py-2 text-sm font-bold rounded-xl bg-cyan-600/50 hover:bg-cyan-600/80
                       text-cyan-200 ring-1 ring-cyan-500/30 transition-all disabled:opacity-50 disabled:cursor-not-allowed"
                    >
                        {selecting ? t("panelNodesPage.serverCard.sending") : t("panelNodesPage.serverCard.activate")}
                    </button>
                )}
            </div>
        </motion.div>
    );
};

export default ServerNodeCard;