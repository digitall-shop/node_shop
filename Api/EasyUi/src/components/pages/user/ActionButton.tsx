import {motion} from "framer-motion";
import React from "react";

export const ActionButton: React.FC<{
    icon: React.ReactNode;
    label: string;
    onClick: () => void;
    className?: string;
    disabled?: boolean;
}> = ({icon, label, onClick, className, disabled}) => (
    <motion.button
        whileHover={!disabled ? {scale: 1.05} : undefined}
        whileTap={!disabled ? {scale: 0.95} : undefined}
        onClick={() => !disabled && onClick()}
        disabled={disabled}
        className={`w-full flex items-center justify-center gap-2 p-3 rounded-lg font-semibold transition-colors
      ${className} ${disabled ? "opacity-50 cursor-not-allowed" : ""}`}
    >
        {icon}
        <span className="text-sm">{label}</span>
    </motion.button>
);
