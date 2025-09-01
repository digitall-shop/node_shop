import React, {useEffect, useState} from "react";
import {Link} from "react-router-dom";
import {useTranslation} from "react-i18next";
import {motion} from "framer-motion";
import {useTelegramUser} from "../../hooks/useTelegramUser";
import {useUserBalance} from "../../hooks/useUserBalance";
import {isSuperAdmin} from "../../models/auth.utils";
import {FaHistory, FaListAlt, FaPaperPlane, FaTasks, FaUniversity, FaUsers} from "react-icons/fa";
import {FaMoneyBills} from "react-icons/fa6";

const ActionCard: React.FC<{
    to?: string;
    title: string;
    desc?: string;
    icon: React.ReactNode;
    gradient: string;
    disabled?: boolean;
}> = ({to = "#", title, desc, icon, gradient, disabled}) => (
    <motion.div
        initial={{opacity: 0, y: 16}}
        whileInView={{opacity: 1, y: 0}}
        viewport={{once: true, amount: 0.4}}
        transition={{duration: 0.35}}
    >
        {disabled ? (
            <div
                className={`group relative block rounded-2xl overflow-hidden border border-white/10 bg-white/5 backdrop-blur-xl p-5 sm:p-6 hover:shadow-2xl transition-shadow pointer-events-none opacity-75`}
            >
                <div
                    aria-hidden
                    className={`absolute -inset-1 rounded-3xl blur-2xl bg-gradient-to-r ${gradient} opacity-0 group-hover:opacity-100 transition-opacity`}
                />
                <div
                    className="absolute top-3 left-3 text-[10px] px-2 py-0.5 rounded-full bg-white/20 text-white/90">به‌زودی
                </div>
                <div className="relative z-10 flex items-center gap-4">
                    <div
                        className={`shrink-0 grid place-items-center w-12 h-12 rounded-xl text-white bg-gradient-to-br ${gradient}`}>
                        {icon}
                    </div>
                    <div className="min-w-0">
                        <h3 className="text-base sm:text-lg font-extrabold text-white tracking-tight">{title}</h3>
                        {desc ? (
                            <p className="mt-1 text-xs sm:text-sm text-white/70 leading-relaxed line-clamp-2">{desc}</p>
                        ) : null}
                    </div>
                    <div className="ml-auto text-white/40">→</div>
                </div>
            </div>
        ) : (
            <Link
                to={to}
                className={`group relative block rounded-2xl overflow-hidden border border-white/10 bg-white/5 backdrop-blur-xl p-5 sm:p-6 hover:shadow-2xl transition-shadow`}
            >
                <div
                    aria-hidden
                    className={`absolute -inset-1 rounded-3xl opacity-0 group-hover:opacity-100 blur-2xl transition-opacity bg-gradient-to-r ${gradient}`}
                />

                <div className="relative z-10 flex items-center gap-4">
                    <div
                        className={`shrink-0 grid place-items-center w-12 h-12 rounded-xl text-white bg-gradient-to-br ${gradient}`}>
                        {icon}
                    </div>
                    <div className="min-w-0">
                        <h3 className="text-base sm:text-lg font-extrabold text-white tracking-tight">{title}</h3>
                        {desc ? (
                            <p className="mt-1 text-xs sm:text-sm text-white/70 leading-relaxed line-clamp-2">{desc}</p>
                        ) : null}
                    </div>
                </div>
            </Link>
        )}
    </motion.div>
);

export default function Home() {
    const {t} = useTranslation();
    const {user} = useTelegramUser();
    const {data: balance, isLoading: balanceLoading, isError: balanceError} = useUserBalance(user?.id);
    const [superAdmin, setSuperAdmin] = useState(isSuperAdmin());

    useEffect(() => {
        const onStorage = (e: StorageEvent) => {
            if (e.key === "roles") setSuperAdmin(isSuperAdmin());
        };
        window.addEventListener("storage", onStorage);
        return () => window.removeEventListener("storage", onStorage);
    }, []);

    const locale = user?.language_code?.startsWith("fa") ? "fa-IR" : undefined;
    const formattedBalance = (balance ?? 0).toLocaleString(locale);

    return (
        <div className="relative text-white ">
            <motion.div
                aria-hidden
                className="pointer-events-none absolute -top-12 -left-12 w-64 h-64 rounded-full bg-fuchsia-400/25 blur-3xl"
                animate={{x: [0, 8, -6, 0], y: [0, -10, 6, 0]}}
                transition={{duration: 12, repeat: Infinity, ease: "easeInOut"}}
            />

            <motion.div
                aria-hidden
                className="pointer-events-none absolute -bottom-12 -right-12 w-72 h-72 rounded-full bg-cyan-400/25 blur-3xl"
                animate={{x: [0, -10, 8, 0], y: [0, 8, -8, 0]}}
                transition={{duration: 13, repeat: Infinity, ease: "easeInOut"}}
            />
            {!superAdmin && (
                <motion.div
                    initial={{opacity: 0, y: 18}}
                    animate={{opacity: 1, y: 0}}
                    transition={{duration: 0.45}}
                    className="lg:col-span-1 pl-5 pr-5 sm:p-6 pt-0  "
                >
                    <div
                        className="rounded-xl border border-white/10 bg-white/10 backdrop-blur-xl px-4 py-3  items-center justify-between inline-flex ">
                        <div className="flex items-center gap-3">
                            <div
                                className="w-8 h-8 rounded-lg bg-cyan-400/20 border border-cyan-300/20 grid place-items-center">
                                <FaMoneyBills/>
                            </div>
                            <div>
                                <p className="text-[11px] sm:text-xs text-gray-300 tracking-wider">{t("profilePage.labels.balance")}</p>
                                <div className="mt-0.5 min-h-[20px]">
                                    {balanceLoading ? (
                                        <div className="h-4 w-24 rounded bg-white/20 animate-pulse"/>
                                    ) : balanceError ? (
                                        <span
                                            className="text-red-300 text-xs">{t("profilePage.balanceError")}</span>
                                    ) : (
                                        <span className="text-sm font-bold">{formattedBalance} {t("profilePage.toman")}</span>
                                    )}
                                </div>
                            </div>
                        </div>
                    </div>
                </motion.div>
            )}

            <div className="relative z-10 max-w-6xl mx-auto px-4 sm:px-6 py-8 sm:py-12">
                <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 lg:gap-8">
                    <motion.div
                        initial={{opacity: 0, y: 18}}
                        animate={{opacity: 1, y: 0}}
                        transition={{duration: 0.55, delay: 0.05}}
                        className={`${superAdmin ? "lg:col-span-3" : "lg:col-span-2"} relative overflow-hidden rounded-2xl border border-white/10 bg-gradient-to-br from-slate-900/40 to-slate-800/40 p-6 sm:p-8`}
                    >
                        <div
                            className="absolute inset-0 opacity-[0.08] bg-[radial-gradient(circle_at_30%_20%,#fff,transparent_40%),radial-gradient(circle_at_70%_60%,#fff,transparent_40%)]"/>
                        <div className="relative z-10 grid grid-cols-1 md:grid-cols-2 gap-6 items-center">
                            <div>
                                <h1 className="text-2xl sm:text-3xl font-extrabold tracking-tight">
                                    <span
                                        className="bg-clip-text text-transparent bg-gradient-to-r from-fuchsia-400 to-emerald-300">
                                        {t("home.heroTitle", {defaultValue: "فروش نود و پنل — آسان، سریع، امن"})}
                                    </span>
                                </h1>
                                <p className="mt-3 text-sm sm:text-base text-white/80 leading-relaxed">
                                    {t("home.heroCopy", {
                                        defaultValue:
                                            "مدیریت کاربران، تایید تراکنش‌ها و اتصال به درگاه‌های بانکی—از همین‌جا وارد بخش‌های اصلی شوید.",
                                    })}
                                </p>
                                <ul className="mt-4 flex flex-wrap gap-2">
                                    {[
                                        t("home.points.fast", {defaultValue: "پرداخت سریع"}),
                                        t("home.points.secure", {defaultValue: "امنیت بالا"}),
                                        t("home.points.panel", {defaultValue: "مدیریت کامل پنل"}),
                                    ].map((chip, i) => (
                                        <li key={i}
                                            className="text-xs sm:text-sm rounded-full border border-white/15 bg-white/5 px-3 py-1 text-white/80">
                                            {chip}
                                        </li>
                                    ))}
                                </ul>
                            </div>

                            <div className="mx-auto w-full max-w-[440px]">
                                <svg className="w-full h-auto" viewBox="0 0 520 320" fill="none"
                                     xmlns="http://www.w3.org/2000/svg">
                                    <defs>
                                        <linearGradient id="g1" x1="0" y1="0" x2="1" y2="1">
                                            <stop offset="0%" stopColor="#22d3ee"/>
                                            <stop offset="100%" stopColor="#a78bfa"/>
                                        </linearGradient>
                                        <linearGradient id="g2" x1="0" y1="0" x2="1" y2="0">
                                            <stop offset="0%" stopColor="#f472b6"/>
                                            <stop offset="100%" stopColor="#34d399"/>
                                        </linearGradient>
                                    </defs>

                                    <g opacity="0.9">
                                        <path
                                            d="M160 180c-30 0-54-24-54-54s24-54 54-54c19 0 36 10 46 25 8-5 17-8 27-8 26 0 48 20 50 46 23 3 41 23 41 47 0 26-21 48-48 48H160c-22 0-40-18-40-40s18-40 40-40z"
                                            fill="url(#g1)"
                                            fillOpacity="0.25"
                                        />
                                        <rect x="136" y="188" width="196" height="86" rx="16" fill="#0ea5e9"
                                              opacity="0.08"/>
                                    </g>

                                    {[0, 1, 2].map((row) => (
                                        <g key={row} transform={`translate(190 ${190 - row * 26})`}>
                                            <rect width="160" height="22" rx="6" fill="#0ea5e9" opacity="0.18"/>
                                            <rect x="10" y="6" width="28" height="10" rx="3" fill="#22d3ee"
                                                  opacity="0.9"/>
                                            <rect x="44" y="6" width="28" height="10" rx="3" fill="#22d3ee"
                                                  opacity="0.6"/>
                                            <rect x="78" y="6" width="28" height="10" rx="3" fill="#22d3ee"
                                                  opacity="0.35"/>
                                            <rect x="112" y="6" width="38" height="10" rx="3" fill="#22d3ee"
                                                  opacity="0.2"/>
                                        </g>
                                    ))}

                                    <g transform="translate(60 206)">
                                        <rect width="120" height="76" rx="12" fill="url(#g2)" opacity="0.25"/>
                                        <rect x="16" y="18" width="56" height="10" rx="3" fill="#fff" opacity="0.35"/>
                                        <rect x="16" y="36" width="88" height="10" rx="3" fill="#fff" opacity="0.18"/>
                                        <circle cx="92" cy="56" r="8" fill="#fff" opacity="0.35"/>
                                    </g>

                                    <g opacity="0.5">
                                        <path d="M318 216l18-10-18-10" stroke="#a78bfa" strokeWidth="2"
                                              strokeLinecap="round"/>
                                        <path d="M148 138l18-10-18-10" stroke="#34d399" strokeWidth="2"
                                              strokeLinecap="round"/>
                                    </g>
                                </svg>
                            </div>
                        </div>
                    </motion.div>
                </div>

                <div className="mt-8 sm:mt-12">
                    {superAdmin ? (
                        <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-5 gap-4 sm:gap-6">
                            <ActionCard
                                to="/users"
                                title={t("profilePage.buttons.users", {defaultValue: "کاربران"})}
                                desc={t("home.actions.users", {defaultValue: "مدیریت و دسترسی کاربران"})}
                                icon={<FaUsers className="w-6 h-6"/>}
                                gradient="from-rose-500 to-fuchsia-500"
                            />
                            <ActionCard
                                to="/paymentConfirmation"
                                title={t("profilePage.buttons.pendingTransactions", {defaultValue: "تراکنش‌های در انتظار"})}
                                desc={t("home.actions.pending", {defaultValue: "بررسی و تایید پرداخت‌ها"})}
                                icon={<FaTasks className="w-6 h-6"/>}
                                gradient="from-amber-500 to-orange-500"
                            />
                            <ActionCard
                                to="/allPayment"
                                title={t("profilePage.buttons.allTransactions", {defaultValue: "همه تراکنش‌ها"})}
                                desc={t("home.actions.allTx", {defaultValue: "گزارش‌ها و فیلتر پیشرفته"})}
                                icon={<FaHistory className="w-6 h-6"/>}
                                gradient="from-cyan-500 to-blue-500"
                            />
                            <ActionCard
                                to="/banks"
                                title={t("profilePage.buttons.bankAccounts", {defaultValue: "حساب‌های بانکی"})}
                                desc={t("home.actions.banks", {defaultValue: "درگاه و حساب‌های متصل"})}
                                icon={<FaUniversity className="w-6 h-6"/>}
                                gradient="from-emerald-500 to-teal-500"
                            />
                            <ActionCard
                                to="/sendMessage"
                                title={t("home.actions.sendMsgTitle", {defaultValue: "ارسال پیام به کاربران"})}
                                desc={t("home.actions.sendMsgDesc", {defaultValue: "ارسال اعلان/پیام "})}
                                icon={<FaPaperPlane className="w-6 h-6"/>}
                                gradient="from-indigo-500 to-violet-500"

                            />
                        </div>
                    ) : (
                        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 sm:gap-6">
                            <ActionCard
                                to="/myPayments"
                                title={t("profilePage.buttons.myTransactions", {defaultValue: "تراکنش‌های من"})}
                                desc={t("home.actions.myTx", {defaultValue: "مشاهده و پیگیری پرداخت‌های شما"})}
                                icon={<FaListAlt className="w-6 h-6"/>}
                                gradient="from-cyan-500 to-blue-500"
                            />
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}
