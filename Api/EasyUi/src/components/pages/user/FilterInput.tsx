import React from "react";

export const FilterInput: React.FC<{
    label: string;
    placeholder: string;
    type?: string;
    onChange: (val: string) => void;
}> = ({label, placeholder, type = "text", onChange}) => (
    <div>
        <label className="block text-xs sm:text-sm text-gray-400 mb-1">{label}</label>
        <input
            type={type}
            placeholder={placeholder}
            onChange={(e) => onChange(e.target.value)}
            className="w-full px-3 py-2 rounded-lg bg-gray-900/60 border border-gray-700 text-sm text-white focus:ring-2 focus:ring-cyan-500 outline-none"
        />
    </div>
);
