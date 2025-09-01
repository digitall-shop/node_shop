import { useEffect, useState } from 'react';

export const useAppSetup = () => {
    const [isInitialized, setIsInitialized] = useState(false);

    useEffect(() => {
        const initializeApp = () => {
            // ۱. توکن را از URL می‌خوانیم
            const urlParams = new URLSearchParams(window.location.search);
            const urlToken = urlParams.get('token');

            if (urlToken) {
                // ۲. اگر توکن وجود داشت، آن را در حافظه ذخیره می‌کنیم
                localStorage.setItem('token', urlToken);
                // URL را پاک می‌کنیم تا توکن در آدرس باقی نماند
                window.history.replaceState({}, document.title, window.location.pathname);
            }

            // ۳. اعلام می‌کنیم که فرآیند آماده‌سازی اولیه تمام شده است
            setIsInitialized(true);
        };

        initializeApp();
    }, []); // این هوک فقط یک بار اجرا می‌شود

    return { isInitialized };
};