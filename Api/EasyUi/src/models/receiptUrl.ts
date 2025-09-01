const API_BASE = import.meta.env.VITE_API_BASE_URL ?? "";
const BASE_NO_API = API_BASE.replace(/\/api\/?$/, "");

export function buildReceiptUrl(raw?: string | null) {
    if (!raw) return null;


    try {
        const u = new URL(raw);

        u.pathname = u.pathname.replace(/^\/api(\/|$)/, "/");
        return u.toString();
    } catch {
        const path = (raw.startsWith("/") ? raw : `/${raw}`).replace(/^\/api(\/|$)/, "/");
        return `${BASE_NO_API}${path}`;
    }
}