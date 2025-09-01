export interface TelegramUser {
    id: number;
    first_name: string;
    last_name?: string;
    username?: string;
    language_code: string;
    photo_url?: string;
}

export interface WebAppInitData {
    user?: TelegramUser;
}

export interface TelegramWebApp {
    initDataUnsafe: WebAppInitData;
    initData: string;
    colorScheme: 'light' | 'dark';
    ready: () => void;
    expand: () => void;
}

declare global {
    interface Window {
        Telegram?: {
            WebApp: TelegramWebApp;
        };
    }
}

export {};
