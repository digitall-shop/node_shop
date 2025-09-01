import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import LanguageDetector from "i18next-browser-languagedetector";

import en from "./locales/en/translation_en.json";
import fa from "./locales/fa/translation_fa.json";

const RTL_LANGUAGES = ["fa", "ar", "he"];

const setDocumentDirection = (language: string) => {
    const langShort = language.slice(0, 2);
    const isRTL = RTL_LANGUAGES.includes(langShort);

    document.documentElement.dir = isRTL ? "rtl" : "ltr";
    document.documentElement.lang = langShort;

    document.body.classList.toggle("rtl", isRTL);
    document.body.classList.toggle("ltr", !isRTL);
};

i18n
    .use(LanguageDetector)
    .use(initReactI18next)
    .init({
        resources: {
            en: { translation: en },
            fa: { translation: fa },
        },
        fallbackLng: "en",
        interpolation: {
            escapeValue: false,
        },
        detection: {
            order: ["localStorage", "navigator", "htmlTag"],
            caches: ["localStorage"],
        },
    })
    .then(() => {
        setDocumentDirection(i18n.language);
    });

i18n.on("languageChanged", (lang) => {
    setDocumentDirection(lang);
});

export default i18n;
