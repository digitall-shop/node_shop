import React, {useState, useRef, useEffect} from 'react';
import {useTranslation} from 'react-i18next';
import {useDirection} from "../../hooks/useDirection.ts";

interface Language {
    code: string;
    name: string;
    flag: string;
    rtl: boolean;
}

const languages: Language[] = [
    {code: 'en', name: 'English', flag: '🇺🇸', rtl: false},
    {code: 'fa', name: 'فارسی', flag: '🇮🇷', rtl: true}
];

const LanguageSwitcher: React.FC = () => {
    const {i18n} = useTranslation();
    const dir = useDirection();
    const [isOpen, setIsOpen] = useState(false);
    const dropdownRef = useRef<HTMLDivElement>(null);

    const currentLanguage = languages.find(lang => lang.code === i18n.language) || languages[0];

    const changeLanguage = (code: string) => {
        i18n.changeLanguage(code);
        setIsOpen(false);
    };

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
                setIsOpen(false);
            }
        };

        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);

    return (
        <div className="relative" ref={dropdownRef}>
            <button
                onClick={() => setIsOpen(!isOpen)}
                className="flex items-center space-x-1 p-2 rounded-full hover:bg-gray-200 dark:hover:bg-gray-700"
                aria-expanded={isOpen}
                aria-haspopup="true"
            >
                <span className="text-xl">{currentLanguage.flag}</span>
                <span className="hidden ">{currentLanguage.name}</span>
            </button>

            {isOpen && (
                <div
                    className={`absolute ${dir.isRtl ? 'left-0 origin-top-left' : 'right-0 origin-top-right'} z-10 mt-2 sm:w-[12rem] md:w-[14rem]  lg:w-[16rem] rounded-md bg-white dark:bg-gray-800 shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none`}
                    role="menu"
                    aria-orientation="vertical"
                    aria-labelledby="menu-button"
                    tabIndex={-1}
                >
                    <div className="py-1" role="none">
                        {languages.map((language) => (
                            <button
                                key={language.code}
                                onClick={() => changeLanguage(language.code)}
                                className={`${
                                    i18n.language === language.code
                                        ? 'bg-gray-100 dark:bg-gray-700 text-primary-light dark:text-primary-dark font-bold'
                                        : 'text-gray-700 dark:text-gray-300'
                                } ${dir.styles.text.start} block w-full px-4 py-2 text-sm hover:bg-gray-100 dark:hover:bg-gray-700`}
                                role="menuitem"
                                tabIndex={-1}
                            >
                                <span className={dir.classes("inline-flex items-center", dir.space('x', 2))}>
                                    <span className="text-xl">{language.flag}</span>
                                    <span>{language.name}</span>
                                </span>
                            </button>
                        ))}
                    </div>
                </div>
            )}
        </div>
    );
};

export default LanguageSwitcher;
