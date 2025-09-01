import React, { useEffect, useState } from "react";
import { Link, useLocation } from "react-router-dom";
import { motion, type Variants } from "framer-motion";
import { useTranslation } from "react-i18next";
import { useDirection } from "../../../hooks/useDirection.ts";
import LanguageSwitcher from "../../common/LanguageSwitcher.tsx";
import { FaMoneyCheckAlt, FaDollarSign } from "react-icons/fa";
import {isSuperAdmin} from "../../../models/auth.utils.ts";

const NavbarComponent: React.FC = () => {
    const { t } = useTranslation();
    useDirection();
    const location = useLocation();

    const [superAdmin, setSuperAdmin] = useState(isSuperAdmin());

    useEffect(() => {
        document.documentElement.classList.add("dark");
    }, []);

    useEffect(() => {
        const onStorage = (e: StorageEvent) => {
            if (e.key === "roles") setSuperAdmin(isSuperAdmin());
        };
        window.addEventListener("storage", onStorage);
        return () => window.removeEventListener("storage", onStorage);
    }, []);

    const navLinks = [
        { to: "/", label: t("footerNav.home") },
        { to: "/profile", label: t("footerNav.profile") },
        { to: "/panel", label: t("footerNav.panel") },
    ];

    const linkVariants: Variants = {
        hidden: { y: -20, opacity: 0 },
        visible: { y: 0, opacity: 1, transition: { type: "spring", stiffness: 120 } },
    };

    const navContainerVariants = {
        hidden: { opacity: 0 },
        visible: { opacity: 1, transition: { staggerChildren: 0.15, delayChildren: 0.2 } },
    };

    return (
        <div className="flex items-center justify-end gap-4 w-full text-black dark:text-white">
            <motion.nav
                className="hidden md:flex gap-6 text-sm font-medium"
                variants={navContainerVariants}
                initial="hidden"
                animate="visible"
            >
                {navLinks.map((link) => (
                    <motion.div key={link.to} variants={linkVariants}>
                        <Link
                            to={link.to}
                            className={`hover:text-cyan-600 transition-colors duration-300 ${
                                location.pathname === link.to ? "text-cyan-500 font-bold" : ""
                            }`}
                        >
                            {link.label}
                        </Link>
                    </motion.div>
                ))}
            </motion.nav>

            <Link to="/transaction" className="text-xl p-2 hover:text-cyan-500 transition-colors">
                <FaMoneyCheckAlt />
            </Link>

            {!superAdmin && (
                <Link to="/payment" className="text-xl p-2 hover:text-cyan-500 transition-colors">
                    <FaDollarSign />
                </Link>
            )}

            <LanguageSwitcher />
        </div>
    );
};

export default NavbarComponent;
