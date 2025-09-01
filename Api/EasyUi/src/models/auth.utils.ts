export const setToken = (token: string): void => {
    localStorage.setItem("token", token);
};


export const getToken = (): string | null => {
    return localStorage.getItem("token");
};

export const removeToken = (): void => {
    localStorage.removeItem("token");
};

const ROLES_KEY = "roles";

export const setRoles = (roles: string[]) =>
    sessionStorage.setItem(ROLES_KEY, JSON.stringify(roles));

export const getRoles = (): string[] =>
    JSON.parse(sessionStorage.getItem(ROLES_KEY) ?? "[]");

export const clearRoles = () => sessionStorage.removeItem(ROLES_KEY);

export const removeRoles = (): void => {
    localStorage.removeItem("roles");
};

export const hasRole = (role: string): boolean => {
    const userRoles = getRoles();

    return userRoles.map(r => r.toLowerCase()).includes(role.toLowerCase());
};

export const isSuperAdmin = (): boolean => {
    return hasRole("superadmin");
};

export const isUser = (): boolean => {
    return hasRole("user");
};

export const clearAuthData = (): void => {
    removeToken();
    removeRoles();
};