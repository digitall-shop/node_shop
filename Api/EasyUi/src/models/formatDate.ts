import i18n from "i18next";

export const formatLocalizedDate = (dateString: Date): string => {
    const date = new Date(dateString);
    const lang = i18n.language;

    if (lang === "fa") {
        return date.toLocaleDateString("fa-IR", {
            year: "numeric",
            month: "long",
            day: "numeric",
        });
    } else {
        return date.toLocaleDateString("en-US", {
            year: "numeric",
            month: "long",
            day: "numeric",
        });
    }
};
