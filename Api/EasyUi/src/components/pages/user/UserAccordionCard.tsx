import {AnimatePresence, motion} from "framer-motion";
import React from "react";
import {useTranslation} from "react-i18next";
import {FaArrowDown, FaArrowUp, FaBan, FaChevronDown, FaPaperPlane, FaUnlockAlt, FaUserCircle} from "react-icons/fa";
import type {User} from "../../../models/user/user";
import {ActionButton} from "./ActionButton.tsx";


export const UserAccordionCard: React.FC<{
    user: User;
    isExpanded: boolean;
    onToggle: () => void;
    onCredit: () => void;
    onDebit: () => void;
    onMessage: () => void;
    onToggleBlock: () => void;
    toggleBusy?: boolean;
}> = ({user, isExpanded, onToggle, onCredit, onDebit, onMessage, onToggleBlock, toggleBusy}) => {
    const {t} = useTranslation();

    const containerCls = user.isBlocked ? "bg-red-900/30 border-red-700" : "bg-gray-800/60 border-gray-700";

    const blockLabel = user.isBlocked ? (t("usersPage.card.unblock")) : (t("usersPage.card.block"));
    const blockIcon = user.isBlocked ? <FaUnlockAlt/> : <FaBan/>;
    const blockBtnClasses = user.isBlocked
        ? "bg-emerald-500/20 hover:bg-emerald-500/40 text-emerald-300"
        : "bg-yellow-500/20 hover:bg-yellow-500/40 text-yellow-300";

    const actionsDisabled = user.isBlocked;

    return (
        <motion.div layout className={`${containerCls} border rounded-xl overflow-hidden`}>
            <button onClick={onToggle} className="w-full flex items-center justify-between p-3 sm:p-4 text-left">
                <div className="flex items-center gap-3 min-w-0">
                    <FaUserCircle
                        className={`w-10 h-10 ${user.isBlocked ? "text-red-400" : "text-cyan-400"} flex-shrink-0`}/>
                    <div className="min-w-0">
                        <p
                            className="font-bold text-sm sm:text-base text-white overflow-hidden"
                            style={{display: "-webkit-box", WebkitLineClamp: 2, WebkitBoxOrient: "vertical"}}
                            title={user.userName ?? `User #${user.id}`}
                        >
                            {user.userName || `User #${user.id}`}
                        </p>
                        <p className="text-xs sm:text-sm text-gray-400 truncate">
                            {t("usersPage.card.idLine", { ID: user.id })}
                        </p>

                        {user.isBlocked && (
                            <span
                                className="inline-block mt-1 text-[10px] px-2 py-0.5 rounded bg-red-700/40 text-red-200">
                {t("usersPage.card.blockedBadge")}
              </span>
                        )}
                    </div>
                </div>
                <motion.div animate={{rotate: isExpanded ? 180 : 0}} className="text-gray-500 ml-2 shrink-0">
                    <FaChevronDown/>
                </motion.div>
            </button>

            <AnimatePresence>
                {isExpanded && (
                    <motion.div
                        key="content"
                        initial={{height: 0, opacity: 0}}
                        animate={{height: "auto", opacity: 1}}
                        exit={{height: 0, opacity: 0}}
                        className="overflow-hidden"
                    >
                        <div className="border-t border-gray-700 p-3 sm:p-4 space-y-4">
                            <div
                                className={`rounded-lg p-3 text-center ${user.isBlocked ? "bg-red-950/40" : "bg-gray-900/50"}`}>
                                <p className="text-[11px] sm:text-xs text-gray-400 uppercase">{t("usersPage.card.balance")}</p>
                                <p className={`text-xl sm:text-2xl font-mono font-bold ${user.isBlocked ? "text-red-300" : "text-cyan-300"}`}>
                                    {user.balance.toLocaleString()}
                                </p>
                            </div>

                            <div className="grid grid-cols-2 sm:grid-cols-4 gap-3">
                                <ActionButton icon={<FaArrowUp/>} label={t("usersPage.card.credit")} onClick={onCredit}
                                              className="bg-green-500/20 hover:bg-green-500/40 text-green-400"
                                              disabled={actionsDisabled}/>
                                <ActionButton icon={<FaArrowDown/>} label={t("usersPage.card.debit")} onClick={onDebit}
                                              className="bg-red-500/20 hover:bg-red-500/40 text-red-400"
                                              disabled={actionsDisabled}/>
                                <ActionButton icon={<FaPaperPlane/>}
                                              label={t("usersPage.card.sendMessage")}
                                              onClick={onMessage}
                                              className="bg-cyan-500/20 hover:bg-cyan-500/40 text-cyan-300"
                                              disabled={actionsDisabled}/>
                                <ActionButton icon={blockIcon} label={blockLabel} onClick={onToggleBlock}
                                              className={`${blockBtnClasses} ${toggleBusy ? "opacity-50 cursor-not-allowed" : ""}`}/>
                            </div>
                        </div>
                    </motion.div>
                )}
            </AnimatePresence>
        </motion.div>
    );
};
