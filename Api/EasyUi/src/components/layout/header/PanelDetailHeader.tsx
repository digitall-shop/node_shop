import React, {useEffect} from "react";
import {NavLink, useParams} from "react-router-dom";
import {useTranslation} from "react-i18next";
import {useDirection} from "../../../hooks/useDirection.ts";
import BrandComponent from "../BrandComponent.tsx";
import LanguageSwitcher from "../../common/LanguageSwitcher.tsx";

const PanelDetailHeader: React.FC = () => {
    const {t} = useTranslation();
    const dir = useDirection();
    const {panelId} = useParams<{ panelId: string }>();
    const basePath = `/panel/${panelId}`;

    const navLinks = [
        {to: basePath, label: t("panelDetails.nav.node"), end: true},
        {to: `${basePath}/host`, label: t("panelDetails.nav.host"), end: false},
        {to: `${basePath}/shop`, label: t("panelDetails.nav.shop"), end: false},
    ];

    useEffect(() => {
        document.documentElement.classList.add("dark");
    }, []);

    return (
        <header className="bg-gray-900/80 backdrop-blur-md shadow-md sticky top-0 z-50">
            <div
                className={dir.classes(
                    "container mx-auto px-4 py-3 flex justify-between items-center",
                    dir.isRtl && "flex-row-reverse"
                )}
            >

                <BrandComponent/>

                <nav className="hidden md:flex gap-6 text-sm font-medium text-white">
                    {navLinks.map((link) => (
                        <NavLink
                            key={link.to}
                            to={link.to}
                            end={link.end}
                            className={({isActive}) =>
                                `hover:text-cyan-400 transition-colors duration-300 ${
                                    isActive ? "text-cyan-300 font-bold" : ""
                                }`
                            }
                        >
                            {link.label}
                        </NavLink>
                    ))}
                </nav>

                <LanguageSwitcher/>


            </div>

        </header>
    );
};

export default PanelDetailHeader;
