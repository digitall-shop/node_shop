import { useTranslation } from 'react-i18next';
import { useMemo } from 'react';
import classNames from 'classnames';

export interface DirectionalStyles {
    margin: {
        start: (size: number) => string;
        end: (size: number) => string;
    };
    padding: {
        start: (size: number) => string;
        end: (size: number) => string;
    };
    text: {
        start: string;
        end: string;
    };
    float: {
        start: string;
        end: string;
    };
    position: {
        start: string;
        end: string;
    };
    justify: {
        start: string;
        end: string;
    };
    items: {
        start: string;
        end: string;
    };
}

export function useDirection() {
    const { i18n } = useTranslation();
    const rtlLanguages = ['fa', 'ar', 'he', 'ur'];
    const isRtl = rtlLanguages.includes(i18n.language);

    return useMemo(() => {
        const direction = isRtl ? 'rtl' : 'ltr';

        const styles: DirectionalStyles = {
            margin: {
                start: (size) => `ms-${size}`,
                end: (size) => `me-${size}`,
            },
            padding: {
                start: (size) => `ps-${size}`,
                end: (size) => `pe-${size}`,
            },
            text: {
                start: isRtl ? 'text-right' : 'text-left',
                end: isRtl ? 'text-left' : 'text-right',
            },
            float: {
                start: isRtl ? 'float-right' : 'float-left',
                end: isRtl ? 'float-left' : 'float-right',
            },
            position: {
                start: isRtl ? 'right' : 'left',
                end: isRtl ? 'left' : 'right',
            },
            justify: {
                start: 'justify-start',
                end: 'justify-end',
            },
            items: {
                start: 'items-start',
                end: 'items-end',
            },
        };

        return {
            isRtl,
            isLtr: !isRtl,
            direction,
            opposite: isRtl ? 'ltr' : 'rtl',
            current: direction,

            className: (ltrClass: string, rtlClass: string) =>
                isRtl ? rtlClass : ltrClass,

            classes: (...args: any[]) => classNames(...args),

            styles,

            space: (axis: 'x' | 'y', size: number) =>
                `space-${axis}-${size} ${isRtl && axis === 'x' ? 'rtl:space-x-reverse' : ''}`,
        };
    }, [isRtl, i18n.language]);
}
