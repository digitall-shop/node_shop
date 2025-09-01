import React, {useCallback, useEffect, useMemo, useRef, useState} from "react";
import {getSubUserInfo} from "../../api/superAdmin/superAdmin";
import {setRoles, removeRoles} from "../../models/auth.utils";
import type {SubUserData} from "../../models/superAdmin/superAdmin";

const clearRolesOnly = () => {
    try {
        removeRoles();
    } catch { /* ignore */
    }
};

const IconShield = (props: React.SVGProps<SVGSVGElement>) => (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.6" {...props}>
        <path d="M12 3l7 3v5c0 5-3.4 9-7 10-3.6-1-7-5-7-10V6l7-3z"/>
        <path d="M9.5 12.5l2 2 3.5-3.5" strokeLinecap="round" strokeLinejoin="round"/>
    </svg>
);

const IconUser = (props: React.SVGProps<SVGSVGElement>) => (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.6" {...props}>
        <circle cx="12" cy="7" r="3.2"/>
        <path d="M4 20c1.8-3.4 5-5 8-5s6.2 1.6 8 5" strokeLinecap="round"/>
    </svg>
);

const IconWarning = (props: React.SVGProps<SVGSVGElement>) => (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.6" {...props}>
        <path d="M10.3 3.7l-8 13.9A2 2 0 004 20h16a2 2 0 001.7-3l-8-13.9a2 2 0 00-3.4 0z"/>
        <path d="M12 9v4" strokeLinecap="round"/>
        <circle cx="12" cy="16.5" r="1" fill="currentColor" stroke="none"/>
    </svg>
);

/* =======================  Loader  ======================= */
const FullScreenLoader = ({message}: { message: string }) => (
    <div dir="rtl"
         className="relative flex min-h-screen items-center justify-center bg-gray-950 text-white overflow-hidden font-sans">
        {/* پس‌زمینه‌ی اورورا + گرادیان چرخان */}
        <AuroraBackground/>

        <div className="flex flex-col items-center z-10">
            <div className="relative">
                <div className="h-16 w-16 rounded-full border-4 border-white/15 border-t-white/70 animate-spin"/>
            </div>
            <p className="mt-6 text-lg text-white/90">{message}</p>
            <div className="mt-4 h-1.5 w-48 overflow-hidden rounded-full bg-white/10">
                <div className="h-full w-1/3 bg-white/70 animate-[slide_1.8s_ease-in-out_infinite]"/>
            </div>

        </div>
    </div>
);

/* =======================  Error  ======================= */
const ErrorDisplay = ({message, onRetry}: { message: string; onRetry: () => void }) => (
    <div dir="rtl"
         className="relative flex min-h-screen items-center justify-center bg-gray-950 text-white overflow-hidden font-sans p-4">
        <AuroraBackground tone="danger"/>
        <div
            className="w-full max-w-lg rounded-2xl border border-white/10 bg-white/5/50 backdrop-blur-xl p-6 shadow-2xl z-10">
            <div className="mx-auto mb-4 flex h-12 w-12 items-center justify-center rounded-full bg-red-500/20">
                <IconWarning className="h-7 w-7 text-red-300"/>
            </div>
            <h2 className="text-center text-2xl font-extrabold">خطا در دریافت اطلاعات</h2>
            <p className="mt-3 text-center text-sm text-white/75 leading-7">{message}</p>
            <button
                onClick={onRetry}
                className="mt-6 w-full rounded-xl bg-white/90 px-5 py-3 text-gray-900 font-bold transition hover:bg-white"
            >
                تلاش دوباره
            </button>
            <p className="mt-3 text-center text-xs text-white/50">در صورت تداوم مشکل، وضعیت لاگین یا اینترنت را بررسی
                کنید.</p>
        </div>
    </div>
);

/* =======================  پس‌زمینه‌ی اورورا/پارتیکل  ======================= */
const AuroraBackground = ({tone = "brand"}: { tone?: "brand" | "danger" }) => (
    <div className="pointer-events-none absolute inset-0 -z-10 overflow-hidden">
        {/* لایه‌ی گرادیان نرم */}
        <div className="absolute inset-0 [mask-image:radial-gradient(ellipse_at_center,black_60%,transparent_100%)]"/>
        {/* هاله‌های متحرک */}
        <div className="absolute -top-1/3 -right-1/4 h-[55rem] w-[55rem] rounded-full blur-3xl animate-aurora-slow"
             style={{background: tone === "danger" ? "rgba(244,63,94,0.18)" : "rgba(99,102,241,0.22)"}}/>
        <div className="absolute -bottom-1/3 -left-1/4 h-[55rem] w-[55rem] rounded-full blur-3xl animate-aurora-slower"
             style={{background: tone === "danger" ? "rgba(251,191,36,0.12)" : "rgba(217,70,239,0.20)"}}/>
        {/* نور چرخان */}
        <div className="absolute inset-[-20%] opacity-25 mix-blend-screen animate-rotate-slow"
             style={{background: "conic-gradient(from 0deg at 50% 50%, #ffffff20, #60a5fa40, #a78bfa40, #f472b640, #ffffff20)"}}/>
        {/* ذرات ریز براق */}
        <div
            className="absolute inset-0 animate-twinkle bg-[radial-gradient(2px_2px_at_10%_20%,#fff8,transparent_40%),radial-gradient(2px_2px_at_80%_30%,#fff7,transparent_40%),radial-gradient(1.5px_1.5px_at_40%_70%,#fff6,transparent_40%)]"/>
    </div>
);

/* =======================  Hook: Tilt سه‌بعدی برای کارت  ======================= */
function useCardTilt<T extends HTMLElement>() {
    const ref = useRef<T | null>(null);
    const [style, setStyle] = useState<React.CSSProperties>({});
    const reset = () => setStyle({transform: "perspective(900px) rotateX(0deg) rotateY(0deg) scale(1)"});

    useEffect(() => {
        reset();
    }, []);

    const onMouseMove = (e: React.MouseEvent) => {
        const el = ref.current;
        if (!el) return;
        const rect = el.getBoundingClientRect();
        const px = (e.clientX - rect.left) / rect.width;
        const py = (e.clientY - rect.top) / rect.height;
        const rx = (py - 0.5) * -12; // زاویه X
        const ry = (px - 0.5) * 14;  // زاویه Y
        setStyle({
            transform: `perspective(900px) rotateX(${rx}deg) rotateY(${ry}deg) scale(1.015)`,
        });
    };

    const onMouseLeave = () => reset();

    return {ref, style, onMouseMove, onMouseLeave};
}

/* =======================  Liquid Glass Button  ======================= */
/* افکت شیشه‌ای خیلی نمایان + موج/ژله داخلی + شاین دنبال‌گر موس + ریپل کلیک */
type LiquidGlassButtonProps = {
    as?: "div" | "button";
    glow?: string;            // رنگ اصلی درخشش
    glow2?: string;           // رنگ مکمل
    intensity?: number;       // شدت نور 0..1
    className?: string;
    onClick?: React.MouseEventHandler<HTMLElement>;
    children: React.ReactNode;
};

const LiquidGlassButton: React.FC<LiquidGlassButtonProps> = ({
                                                                 children,
                                                                 className = "",
                                                                 glow = "#8b5cf6",   // indigo-500
                                                                 glow2 = "#f43f5e",  // rose-500
                                                                 intensity = 1,
                                                                 as = "div",
                                                                 onClick,
                                                             }) => {
    const Comp: any = as === "button" ? "button" : "div";
    const ref = useRef<HTMLDivElement | HTMLButtonElement | null>(null);
    const [ripples, setRipples] = useState<Array<{ id: number; x: number; y: number }>>([]);
    const rippleId = useRef(0);

    const handleMouseMove = (e: React.MouseEvent) => {
        const el = ref.current as HTMLElement | null;
        if (!el) return;
        const rect = el.getBoundingClientRect();
        const x = e.clientX - rect.left;
        const y = e.clientY - rect.top;
        el.style.setProperty("--mx", `${x}px`);
        el.style.setProperty("--my", `${y}px`);
    };

    const handleClick = (e: React.MouseEvent<HTMLElement>) => {
        const el = ref.current as HTMLElement | null;
        if (el) {
            const rect = el.getBoundingClientRect();
            setRipples((r) => [
                ...r,
                {id: ++rippleId.current, x: e.clientX - rect.left, y: e.clientY - rect.top},
            ]);
            setTimeout(() => setRipples((r) => r.slice(1)), 600);
        }
        onClick?.(e as any);
    };

    return (
        <Comp
            ref={ref as any}
            className={[
                "group relative inline-flex select-none items-center justify-center",
                "rounded-2xl isolate will-change-transform",
                "transition-transform duration-200 active:scale-[.98]",
                className,
            ].join(" ")}
            style={{["--glow" as any]: glow, ["--glow2" as any]: glow2, ["--int" as any]: intensity}}
            onMouseMove={handleMouseMove}
            onClick={handleClick}
        >
            <span
                aria-hidden
                className="pointer-events-none absolute -inset-5 -z-30 blur-3xl opacity-70 transition group-hover:opacity-95"
                style={{
                    background: `radial-gradient(60% 80% at 20% 20%, var(--glow) ${40 * intensity}%, transparent 70%), radial-gradient(70% 90% at 80% 80%, var(--glow2) ${30 * intensity}%, transparent 70%)`,
                }}/>

            <span
                aria-hidden
                className="absolute inset-0 -z-10 rounded-2xl p-[2px]"
                style={{
                    background: "linear-gradient(180deg, rgba(255,255,255,.75), rgba(255,255,255,.18))",
                    WebkitMask: "linear-gradient(#000 0 0) content-box, linear-gradient(#000 0 0)",
                    WebkitMaskComposite: "xor" as any,
                    maskComposite: "exclude",
                }}
            />

            <span
                className={[
                    "relative rounded-2xl border border-white/25 bg-white/15 backdrop-blur-3xl",
                    "px-7 py-3 text-white font-extrabold tracking-tight",
                    "shadow-[inset_0_1px_0_rgba(255,255,255,.55),0_20px_50px_-18px_rgba(0,0,0,.75)]",
                    "overflow-hidden",
                ].join(" ")}
            >
                <span className="pointer-events-none absolute inset-0 -z-10 rounded-2xl overflow-hidden">
          <span
              className="absolute w-48 h-48 -top-10 -left-6 rounded-full opacity-40 blur-2xl"
              style={{
                  background: "radial-gradient(circle at 30% 30%, var(--glow), transparent 60%)",
                  animation: "lgb-float1 7s ease-in-out infinite"
              }}
          />
          <span
              className="absolute w-40 h-40 -bottom-10 -right-6 rounded-full opacity-35 blur-2xl"
              style={{
                  background: "radial-gradient(circle at 70% 70%, var(--glow2), transparent 60%)",
                  animation: "lgb-float2 8s ease-in-out infinite"
              }}
          />
          <span
              className="absolute w-56 h-56 -bottom-14 left-1/3 rounded-full opacity-30 blur-2xl"
              style={{
                  background: "radial-gradient(circle at 50% 50%, #60a5fa, transparent 60%)",
                  animation: "lgb-float3 10s ease-in-out infinite"
              }}
          />
        </span>

                <span
                    aria-hidden
                    className="pointer-events-none absolute -inset-px rounded-2xl opacity-80"
                    style={{
                        background: "radial-gradient(120px 120px at var(--mx) var(--my), rgba(255,255,255,.45), transparent 60%)",
                        transition: "background 120ms ease-out",
                    }}
                />

                <span
                    aria-hidden
                    className="pointer-events-none absolute inset-x-4 top-1.5 h-2 rounded-full opacity-90 blur"
                    style={{
                        background: "linear-gradient(90deg, rgba(255,255,255,.15), rgba(255,255,255,.9), rgba(255,255,255,.15))",
                        animation: "lgb-shine 2.6s ease-in-out infinite"
                    }}
                />

                <span aria-hidden
                      className="pointer-events-none absolute inset-x-10 bottom-1.5 h-5 rounded-full bg-white/15 blur-2xl"/>

                {ripples.map((r) => (
                    <span
                        key={r.id}
                        className="pointer-events-none absolute rounded-full bg-white/40"
                        style={{
                            left: r.x,
                            top: r.y,
                            width: 10,
                            height: 10,
                            transform: "translate(-50%,-50%)",
                            filter: "blur(2px)",
                            animation: "lgb-ripple 600ms ease-out forwards",
                        }}
                    />
                ))}

                {children}
      </span>
            <span aria-hidden className="pointer-events-none absolute inset-0 rounded-2xl ring-1 ring-white/30"/>
        </Comp>
    );
};

const RoleSelectionPrompt = ({
                                 onSelectRole,
                                 busy,
                                 userInfo,
                             }: {
    onSelectRole: (role: "superadmin" | "user") => void;
    busy?: boolean;
    userInfo?: SubUserData | null;
}) => {
    const adminTilt = useCardTilt<HTMLButtonElement>();
    const userTilt = useCardTilt<HTMLButtonElement>();

    return (
        <div dir="rtl"
             className="relative flex min-h-screen items-center justify-center bg-gray-950 text-white overflow-hidden font-sans p-4">
            <AuroraBackground/>

            <div className="w-full max-w-5xl z-10">
                <div className="mb-7 text-center">
                    <h1 className="text-3xl font-black tracking-tight">
                        {userInfo?.firstName ? (
                            <span>سلام <span className="text-indigo-300">{userInfo.firstName}</span> 👋</span>
                        ) : (<span>خوش آمدید</span>)}
                    </h1>
                    <p className="mt-2 text-white/70 text-sm">لطفاً نحوه ورود خود را انتخاب کنید.</p>
                </div>

                <div className="grid gap-5 sm:grid-cols-2">
                    <button
                        ref={adminTilt.ref}
                        onMouseMove={adminTilt.onMouseMove}
                        onMouseLeave={adminTilt.onMouseLeave}
                        style={adminTilt.style}
                        onClick={() => onSelectRole("superadmin")}
                        disabled={busy}
                        className="group relative overflow-hidden rounded-3xl border border-white/15 p-6 text-right transition disabled:cursor-not-allowed focus:outline-none focus:ring-2 focus:ring-indigo-300/60"
                    >
                        <div className="absolute inset-0 -z-10 rounded-3xl p-[1.5px]"
                             style={{
                                 background: "linear-gradient(140deg, rgba(255,255,255,.6), rgba(255,255,255,.08))",
                                 WebkitMask: "linear-gradient(#000 0 0) content-box, linear-gradient(#000 0 0)",
                                 WebkitMaskComposite: "xor" as any, maskComposite: "exclude",
                             }}/>
                        <div
                            className="absolute inset-0 -z-20 rounded-3xl bg-gradient-to-br from-indigo-600/30 via-fuchsia-600/25 to-indigo-700/25 backdrop-blur-2xl"/>
                        <div className="absolute -inset-10 -z-30 blur-3xl opacity-60 group-hover:opacity-90 transition"
                             style={{background: "radial-gradient(60% 80% at 80% 20%, #a78bfa66, transparent 60%), radial-gradient(70% 90% at 20% 90%, #f472b666, transparent 60%)"}}/>

                        <div className="mb-4 flex items-center gap-3">
                            <div
                                className="flex h-12 w-12 items-center justify-center rounded-2xl bg-white/10 backdrop-blur-md shadow-inner">
                                <IconShield className="h-7 w-7"/>
                            </div>
                            <h3 className="text-xl font-extrabold">ورود به عنوان سوپر ادمین</h3>
                        </div>
                        <p className="text-sm text-white/85 leading-7">
                            دسترسی کامل به پنل‌های مدیریتی، تایید/رد پرداخت‌ها و مدیریت کاربران.
                        </p>

                        <LiquidGlassButton as="div" glow="#8b5cf6" glow2="#f43f5e" intensity={1} className="mt-6">
                            انتخاب این نقش
                        </LiquidGlassButton>
                    </button>

                    <button
                        ref={userTilt.ref}
                        onMouseMove={userTilt.onMouseMove}
                        onMouseLeave={userTilt.onMouseLeave}
                        style={userTilt.style}
                        onClick={() => onSelectRole("user")}
                        disabled={busy}
                        className="group relative overflow-hidden rounded-3xl border border-white/15 p-6 text-right transition disabled:cursor-not-allowed focus:outline-none focus:ring-2 focus:ring-cyan-300/50"
                    >
                        <div className="absolute inset-0 -z-10 rounded-3xl p-[1.5px]"
                             style={{
                                 background: "linear-gradient(140deg, rgba(255,255,255,.6), rgba(255,255,255,.08))",
                                 WebkitMask: "linear-gradient(#000 0 0) content-box, linear-gradient(#000 0 0)",
                                 WebkitMaskComposite: "xor" as any, maskComposite: "exclude",
                             }}/>
                        <div
                            className="absolute inset-0 -z-20 rounded-3xl bg-gradient-to-br from-white/8 via-white/6 to-white/10 backdrop-blur-2xl"/>
                        <div className="absolute -inset-10 -z-30 blur-3xl opacity-55 group-hover:opacity-85 transition"
                             style={{background: "radial-gradient(60% 80% at 20% 15%, #22d3ee66, transparent 60%), radial-gradient(70% 90% at 80% 85%, #60a5fa66, transparent 60%)"}}/>

                        <div className="mb-4 flex items-center gap-3">
                            <div
                                className="flex h-12 w-12 items-center justify-center rounded-2xl bg-white/10 backdrop-blur-md shadow-inner">
                                <IconUser className="h-7 w-7"/>
                            </div>
                            <h3 className="text-xl font-extrabold">ورود به عنوان کاربر</h3>
                        </div>
                        <p className="text-sm text-white/85 leading-7">
                            مشاهده وضعیت پرداخت‌ها و تراکنش‌ها، بدون دسترسی مدیریتی.
                        </p>

                        <LiquidGlassButton as="div" glow="#06b6d4" glow2="#60a5fa" intensity={1} className="mt-6">
                            انتخاب این نقش
                        </LiquidGlassButton>
                    </button>
                </div>

                <p className="mt-7 text-center text-xs text-white/60">
                    نقش انتخاب‌شده تا پایان این نشست ذخیره می‌شود و با بستن مینی‌اپ پاک خواهد شد.
                </p>
            </div>
        </div>
    );
};

export default function CheckingSuperAdminPage({onRoleSet}: { onRoleSet: () => void }) {
    const [loading, setLoading] = useState(true);
    const [isSuperAdmin, setIsSuperAdmin] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [busySelect, setBusySelect] = useState(false);
    const [userInfo, setUserInfo] = useState<SubUserData | null>(null);

    const once = useMemo(() => ({done: false}), []);

    const fetchUserInfo = useCallback(async () => {
        setLoading(true);
        setError(null);
        try {
            const info = await getSubUserInfo();
            setUserInfo(info ?? null);
            if (info?.isSuperAdmin) {
                setIsSuperAdmin(true);
            } else {
                setRoles(["user"]);
                onRoleSet();
            }
        } catch (e: any) {
            setError(e?.message || "دریافت اطلاعات کاربر ناموفق بود.");
        } finally {
            setLoading(false);
        }
    }, [onRoleSet]);

    useEffect(() => {
        if (once.done) return;
        once.done = true;
        fetchUserInfo();
    }, [fetchUserInfo, once]);

    useEffect(() => {
        const handlePageHide = () => clearRolesOnly();
        const handleVisibility = () => {
            if (document.visibilityState === "hidden") clearRolesOnly();
        };
        window.addEventListener("pagehide", handlePageHide);
        document.addEventListener("visibilitychange", handleVisibility);
        return () => {
            window.removeEventListener("pagehide", handlePageHide);
            document.removeEventListener("visibilitychange", handleVisibility);
        };
    }, []);

    const handleRoleSelection = (role: "superadmin" | "user") => {
        if (busySelect) return;
        setBusySelect(true);
        try {
            setRoles([role]);
            onRoleSet();
        } finally {
            setBusySelect(false);
        }
    };

    if (loading) return <FullScreenLoader message="در حال بررسی اطلاعات کاربر..."/>;
    if (error) return <ErrorDisplay message={error} onRetry={fetchUserInfo}/>;
    if (isSuperAdmin) return (
        <RoleSelectionPrompt onSelectRole={handleRoleSelection} busy={busySelect} userInfo={userInfo}/>
    );
    return null;
}
