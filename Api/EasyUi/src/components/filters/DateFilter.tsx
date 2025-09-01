import React from "react";
import DatePicker from "react-multi-date-picker";
import type { Value } from "react-multi-date-picker";
import persian from "react-date-object/calendars/persian";
import gregorian from "react-date-object/calendars/gregorian";
import persian_fa from "react-date-object/locales/persian_fa";
import gregorian_en from "react-date-object/locales/gregorian_en";
import { useTranslation } from "react-i18next";
import { useDirection } from "../../hooks/useDirection";

type Props = {
    locale: "fa" | "en";
    value: Value;
    onChange: (utcISOString: string | null) => void;
    type: "from" | "to";
    direction?: "rtl" | "ltr";
};

const DateFilter: React.FC<Props> = ({ locale, value, onChange, type }) => {
    const isFa = locale === "fa";
    const calendar = isFa ? persian : gregorian;
    const localeObj = isFa ? persian_fa : gregorian_en;

    const { t } = useTranslation();
    const direction = useDirection();

    // برای دسترسی‌پذیری و fallback بهتر
    const placeholder =
        type === "from"
            ? t("filters.dateFrom") ?? t("order.filters.from_date")
            : t("filters.dateTo") ?? t("order.filters.to_date");

    return (
        <div dir={direction.current} className="w-full">
            <DatePicker
                value={value ?? null}
                onChange={(date) => {
                    if (!date || Array.isArray(date)) return onChange(null);
                    const js = date?.toDate?.();
                    onChange(js ? js.toISOString() : null);
                }}
                calendar={calendar}
                locale={localeObj}
                format="YYYY/MM/DD"
                calendarPosition={direction.current === "rtl" ? "bottom-right" : "bottom-left"}

                /** --- مهم: جلوگیری از بریدن پاپ‌آپ زیر والد --- */
                portal                           // رندر در body
                zIndex={70}                      // مطمئن که روی همه بیاد

                /** پهنای کامل ورودی و کانتینر */
                style={{ width: "100%" }}
                containerStyle={{ width: "100%" }}

                /** استایل ورودی مطابق بقیه‌ی فرم‌ها */
                inputClass="w-full px-3 py-2 rounded-lg bg-gray-900/60 border border-gray-700 text-sm text-white
                    focus:ring-2 focus:ring-cyan-500 outline-none placeholder-gray-400"

                placeholder={placeholder}
                disableDayPicker={false}
                // قابل پاک‌کردن در صورت نیاز:
                // clearable
            />
        </div>
    );
};

export default DateFilter;
