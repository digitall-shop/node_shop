import {motion, AnimatePresence} from "framer-motion";
import React, {useState} from "react";
import {useTranslation} from "react-i18next";
import {FaPaperPlane, FaTrashAlt} from "react-icons/fa";
import type {User} from "../../../models/user/user.ts";

const MAX_DM_LEN = 4096;

export const SendMessageModal: React.FC<{
    user: User;
    isLoading: boolean;
    onSend: (text: string) => void;
    onClose: () => void;
}> = ({user, isLoading, onSend, onClose}) => {
    const {t} = useTranslation();
    const [text, setText] = useState("");
    const hasText = text.trim().length > 0;
    const tooLong = text.length > MAX_DM_LEN;

    return (
        <motion.div initial={{opacity: 0}} animate={{opacity: 1}} exit={{opacity: 0}}
                    className="fixed inset-0 bg-black/60 flex items-center justify-center p-4 z-50" onClick={onClose}>
            <motion.div
                initial={{scale: 0.92, y: 14}} animate={{scale: 1, y: 0}} exit={{scale: 0.92, y: 14}}
                className="bg-gray-800 border border-gray-700 rounded-xl w-full max-w-lg p-5 sm:p-6"
                onClick={(e) => e.stopPropagation()}
            >
                <h3 className="text-lg sm:text-xl font-bold mb-1">{t("usersPage.dm.title")}</h3>
                <p className="text-gray-400 mb-4">
                    {t("usersPage.dm.toUser", {username: user.userName || `ID: ${user.id}`}) || `گیرنده: ${user.userName || `ID: ${user.id}`}`}
                </p>

                <div className="rounded-lg bg-gray-900/60 border border-gray-700 p-2">
          <textarea
              rows={6}
              value={text}
              onChange={(e) => setText(e.target.value)}
              placeholder={t("usersPage.dm.placeholder") || "پیام خود را بنویسید..."}
              className="w-full bg-transparent outline-none text-white placeholder-gray-500 p-2 rounded-md"
          />
                    <div className="flex items-center justify-between px-2 pb-1">
            <span className={`text-xs ${tooLong ? "text-red-400" : "text-gray-400"}`}>
              {`${text.length.toLocaleString("fa-IR")}/${MAX_DM_LEN.toLocaleString("fa-IR")}`}
            </span>
                        {tooLong && <span className="text-xs text-red-400">حداکثر {MAX_DM_LEN} کاراکتر</span>}
                    </div>
                </div>

                <AnimatePresence>
                    {hasText && !tooLong && (
                        <motion.div initial={{opacity: 0, y: 8}} animate={{opacity: 1, y: 0}} exit={{opacity: 0, y: 8}}
                                    className="mt-4 flex gap-3">
                            <button
                                type="button"
                                onClick={() => setText("")}
                                disabled={isLoading}
                                className="w-full py-2.5 rounded-lg bg-gray-600 hover:bg-gray-700 text-white font-semibold transition-colors disabled:opacity-50 flex items-center justify-center gap-2"
                            >
                                <FaTrashAlt/> {t("buttons.clear")}
                            </button>
                            <button
                                type="button"
                                onClick={() => onSend(text)}
                                disabled={isLoading}
                                className="w-full py-2.5 rounded-lg bg-cyan-600 hover:bg-cyan-700 text-white font-semibold transition-colors disabled:opacity-50 flex items-center justify-center gap-2"
                            >
                                <FaPaperPlane/> {isLoading ? (t("common.sending")) : (t("buttons.send"))}
                            </button>
                        </motion.div>
                    )}
                </AnimatePresence>

                <div className="mt-3">
                    <button onClick={onClose}
                            className="w-full py-2.5 rounded-lg bg-gray-700 hover:bg-gray-600 text-white transition-colors">
                        {t("buttons.cancel")}
                    </button>
                </div>
            </motion.div>
        </motion.div>
    );
};
