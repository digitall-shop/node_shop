export const dtfOpts: Intl.DateTimeFormatOptions = {
    year: "numeric", month: "2-digit", day: "2-digit",
    hour: "2-digit", minute: "2-digit", hour12: false,
};

export const parseBackendIso = (s?: string | null) => {
    if (!s) return null;
    const hasTZ = /[zZ]|([+\-]\d{2}:?\d{2})$/.test(s);
    try { return new Date(hasTZ ? s : s + "Z"); } catch { return new Date(s); }
};

export const formatInTehran = (iso?: string, lang?: string) => {
    const d = typeof iso === "string" ? parseBackendIso(iso) : iso;
    if (!d) return "-";
    try {
        const isFa = (lang ?? "").toLowerCase().startsWith("fa");
        const locale = isFa ? "fa-IR-u-ca-persian" : (lang || "en-US");
        return new Intl.DateTimeFormat(locale, {...dtfOpts, timeZone: "Asia/Tehran"}).format(d);
    } catch {
        return new Intl.DateTimeFormat("en-US", {...dtfOpts, timeZone: "Asia/Tehran"}).format(d as any);
    }
};
