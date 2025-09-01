import {useState} from "react";
import {useTranslation} from "react-i18next";
import {motion} from "framer-motion";
import {FaIdCard, FaCopy, FaCheck} from "react-icons/fa";

import {useTelegramUser} from "../../hooks/useTelegramUser";
import {useUserBalance} from "../../hooks/useUserBalance";
import Avatar from "./Avatar";

const ProfileMenu = () => {
    const {user, loading} = useTelegramUser();
    const {t} = useTranslation();
    const {
        data: balance,
        isLoading: balanceLoading,
        isError: balanceError,
    } = useUserBalance(user?.id);

    const [copied, setCopied] = useState(false);

    async function copyToClipboard(text: string) {
        try {
            if (navigator.clipboard?.writeText) {
                await navigator.clipboard.writeText(text);
            } else {
                const ta = document.createElement("textarea");
                ta.value = text;
                ta.style.position = "fixed";
                ta.style.left = "-9999px";
                document.body.appendChild(ta);
                ta.select();
                document.execCommand("copy");
                document.body.removeChild(ta);
            }
            setCopied(true);
            setTimeout(() => setCopied(false), 1200);
        } catch (e) {
            console.error("Copy failed", e);
        }
    }

    if (loading) {
        return (
            <div className="text-center mt-10 text-gray-400">
                {t("profilePage.loading")}
            </div>
        );
    }

    if (!user) {
        return (
            <div className="text-center mt-10 text-red-500">
                {t("profilePage.unauthorized")}
            </div>
        );
    }

    const fullName = `${user.first_name ?? ""}${
        user.last_name ? " " + user.last_name : ""
    }`.trim();

    return (
        <div className="flex justify-center items-start pt-10 pb-20 px-4">
            <motion.div
                initial={{opacity: 0, y: 30}}
                animate={{opacity: 1, y: 0}}
                transition={{duration: 0.5, ease: "easeOut"}}
                className="w-full max-w-sm bg-gray-900/50 backdrop-blur-0 border border-white/10 rounded-2xl shadow-2xl overflow-hidden"
            >
                <div className="relative">
                    <div className="h-24 bg-gradient-to-r from-cyan-500 to-blue-600 opacity-80"/>
                    <div className="absolute -bottom-12 left-1/2 -translate-x-1/2">
                        <Avatar
                            src={user.photo_url ?? undefined}
                            name={fullName || user.username || t("profilePage.avatarFallback")}
                            className="w-24 h-24 text-4xl border-4 border-gray-800 rounded-full shadow-lg"
                        />
                    </div>
                </div>

                <div className="pt-16 pb-6 px-6 text-center">
                    <h2 className="text-2xl font-bold text-white">{fullName}</h2>
                    {user.username && (
                        <p className="text-md text-gray-400">@{user.username}</p>
                    )}
                </div>

                <div className="px-6 pb-6">
                    <div className="bg-black/20 rounded-xl p-4 text-center">
                        <p className="text-sm font-medium text-gray-400 uppercase tracking-wider">
                            {t("profilePage.labels.balance")}
                        </p>
                        <p className="text-3xl font-bold text-cyan-400 mt-1">
                            {balanceLoading
                                ? t("profilePage.balanceLoading")
                                : balanceError
                                    ? t("profilePage.balanceError")
                                    : (balance ?? 0).toLocaleString()}
                        </p>
                    </div>
                </div>

                <div className="px-6 pb-6 text-sm text-gray-300">
                    <ul className="space-y-3">
                        <li className="flex items-center gap-3">
                            <FaIdCard className="text-gray-500 w-5 h-5"/>
                            <span className="text-gray-300">
                <strong>{t("profilePage.labels.id")}:</strong>
              </span>

                            <button
                                type="button"
                                onClick={() => copyToClipboard(String(user.id))}
                                className="inline-flex items-center gap-2 font-mono text-cyan-300 hover:text-cyan-200 active:scale-95 transition"
                                title={t("profilePage.copyIdTooltip") ?? "برای کپی کلیک کنید"}
                                aria-label="Copy user id"
                            >
                                <span>{user.id}</span>
                                {copied ? (
                                    <FaCheck className="w-4 h-4 text-emerald-400"/>
                                ) : (
                                    <FaCopy className="w-4 h-4"/>
                                )}
                            </button>
                        </li>
                    </ul>
                </div>
            </motion.div>
        </div>
    );
};

export default ProfileMenu;
