import { motion } from "framer-motion";
import { useTranslation } from "react-i18next";
import { useDirection } from "../../../hooks/useDirection";
import type { PanelNodeDto } from "../../../models/instance/instance.ts";
import {
    ContainerProvisionStatus,
    canTogglePause,
    isPausedBySystem,
} from "../../../models/instance/status.ts";
import { statusConfig } from "../../../models/instanceStatus.ts";

interface Props {
    node: PanelNodeDto;
    onTogglePause?: () => void;
    toggling?: boolean;
    onEdit?: () => void;
    onDelete?: () => void;
    deleting?: boolean;
}

const codeToFlag = (name?: string) => {
    const raw = (name ?? "").split("_")[0]?.slice(0, 2).toUpperCase();
    if (!raw || raw.length !== 2) return "🏳️";
    const base = 0x1f1e6;
    return String.fromCodePoint(
        base + (raw.charCodeAt(0) - 65),
        base + (raw.charCodeAt(1) - 65)
    );
};

const PanelNodeCard = ({ node, onTogglePause, toggling = false, onDelete }: Props) => {
    const { t, i18n } = useTranslation();
    const dir = useDirection();

    const statusInfo = statusConfig[node.status as keyof typeof statusConfig];

    const status = statusInfo || {
        labelKey: "containerStatus.unknown",
        icon: "fa-question-circle",
        color: "text-gray-400",
        bg: "bg-gray-500/10",
    };

    const showToggle = canTogglePause(node.status);
    const pausedBySystem = isPausedBySystem(node.status);
    const isPausedByUser = node.status === ContainerProvisionStatus.PausedByUser;

    const locale = i18n.language === 'fa' ? 'fa-IR' : 'en-US';
    const formattedDate = new Date(node.createDate).toLocaleDateString(locale);

    return (
        <motion.div
            dir={dir.direction}
            layout
            initial={{ opacity: 0, y: 20, scale: 0.98 }}
            animate={{ opacity: 1, y: 0, scale: 1 }}
            exit={{ opacity: 0, scale: 0.95, transition: { duration: 0.2 } }}
            whileHover={{ y: -5, transition: { type: "spring", stiffness: 300 } }}
            className="bg-gradient-to-br from-gray-800 to-gray-900 border border-green-500/20 rounded-xl overflow-hidden shadow-lg"
        >
            <div className="p-5 flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
                <div className="flex items-center gap-4">
                    <div className={`h-12 w-12 rounded-lg flex items-center justify-center ${status.bg}`}>
                        <span className="text-2xl">{codeToFlag(node.nodeName)}</span>
                    </div>
                    <div>
                        <h3 className="text-lg font-bold text-white">{node.nodeName}</h3>
                        <p className="text-sm text-gray-400">
                            {t("panelNodesPage.nodeCard.activatedOn")}: {formattedDate}
                        </p>
                    </div>
                </div>
                <div
                    className={`px-3 py-1 rounded-full text-xs font-medium flex items-center gap-2 ${status.bg} ${status.color}`}
                >
                    <i className={`fas ${status.icon}`} />
                    {t(status.labelKey)} {/* Use t() to translate the labelKey */}
                </div>

                <div className="w-full sm:w-auto flex items-center justify-end flex-wrap gap-2">
                    {showToggle && (
                        <motion.button
                            whileHover={{ scale: 1.05 }}
                            whileTap={{ scale: 0.95 }}
                            disabled={toggling}
                            onClick={onTogglePause}
                            className={`h-9 w-9 inline-flex items-center justify-center rounded-full ring-1 transition-all ${
                                isPausedByUser
                                    ? "bg-emerald-500/15 ring-emerald-400/40 text-emerald-300 hover:bg-emerald-500/25"
                                    : "bg-yellow-500/15 ring-yellow-400/40 text-yellow-300 hover:bg-yellow-500/25"
                            } disabled:opacity-60 disabled:cursor-not-allowed`}
                            aria-label={isPausedByUser ? t("panelNodesPage.nodeCard.aria.resume") : t("panelNodesPage.nodeCard.aria.pause")}
                            title={isPausedByUser ? t("panelNodesPage.nodeCard.aria.resume") : t("panelNodesPage.nodeCard.aria.pause")}
                        >
                            {toggling ? (
                                <i className="fas fa-circle-notch fa-spin" />
                            ) : isPausedByUser ? (
                                <i className="fas fa-play" />
                            ) : (
                                <i className="fas fa-pause" />
                            )}
                        </motion.button>
                    )}

                    {onDelete && (
                        <motion.button
                            whileHover={{ scale: 1.05 }}
                            whileTap={{ scale: 0.95 }}
                            onClick={onDelete}
                            className="h-9 w-9 inline-flex items-center justify-center ring-1 rounded-full bg-red-500/20 ring-red-400/40 hover:bg-red-500/40 text-red-400 transition-colors"
                            aria-label={t("panelNodesPage.nodeCard.aria.delete")}
                            title={t("panelNodesPage.nodeCard.aria.delete")}
                        >
                            <i className="fas fa-trash-alt text-sm" />
                        </motion.button>
                    )}

                    {pausedBySystem && (
                        <span
                            className="inline-flex items-center gap-2 px-3 h-9 rounded-xl bg-sky-500/10 text-sky-300 ring-1 ring-sky-400/30 cursor-not-allowed"
                            title={t("panelNodesPage.nodeCard.pausedBySystem")}
                        >
                            <i className="fas fa-info-circle" />
                            <span className="text-xs">{t("panelNodesPage.nodeCard.pausedBySystem")}</span>
                        </span>
                    )}
                </div>
            </div>
        </motion.div>
    );
};

export default PanelNodeCard;