import {motion} from "framer-motion";
import React from "react";
import {useTranslation} from "react-i18next";
import type {User} from "../../../models/user/user.ts";
import {useBalanceForm} from "../../../hooks/useBalanceForm.ts";


type FormInputs = { amount: string; description: string };

export const BalanceModal: React.FC<{
    user: User;
    type: "credit" | "debit";
    onClose: () => void;
    onSubmit: (data: FormInputs) => void;
    isLoading: boolean;
}> = ({user, type, onClose, onSubmit, isLoading}) => {
    const {t} = useTranslation();
    const isDebit = type === "debit";
    const {formData, errors, handleChange, validate, isValid} = useBalanceForm(isDebit);

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (validate()) onSubmit(formData);
    };

    const title = isDebit ? t("usersPage.modal.debitTitle") : t("usersPage.modal.creditTitle");

    return (
        <motion.div initial={{opacity: 0}} animate={{opacity: 1}} exit={{opacity: 0}}
                    className="fixed inset-0 bg-black/60 flex items-center justify-center p-4 z-50" onClick={onClose}>
            <motion.div
                initial={{scale: 0.9, y: 20}} animate={{scale: 1, y: 0}} exit={{scale: 0.9, y: 20}}
                className="bg-gray-800 border border-gray-700 rounded-xl p-6 w-full max-w-md"
                onClick={(e) => e.stopPropagation()}
            >
                <h3 className="text-xl font-bold mb-1">{title}</h3>
                <p className="text-gray-400 mb-6">
                    {t("usersPage.modal.forUser", {username: user.userName || `ID: ${user.id}`})}
                </p>

                <form onSubmit={handleSubmit} className="space-y-4">
                    <div>
                        <label className="block text-sm text-gray-400 mb-1">{t("usersPage.modal.amountLabel")}</label>
                        <input
                            type="number"
                            name="amount"
                            value={formData.amount}
                            onChange={handleChange}
                            className="w-full px-3 py-2 rounded-lg bg-gray-900/60 border border-gray-700 text-white focus:ring-2 focus:ring-cyan-500 outline-none"
                        />
                        {errors.amount && <p className="text-red-400 text-sm mt-1">{errors.amount}</p>}
                    </div>
                    <div>
                        <label className="block text-sm text-gray-400 mb-1">
                            {t("usersPage.modal.descriptionLabel")}
                            {isDebit && <span className="text-red-400">*</span>}
                        </label>
                        <textarea
                            rows={3}
                            name="description"
                            value={formData.description}
                            onChange={handleChange}
                            className="w-full px-3 py-2 rounded-lg bg-gray-900/60 border border-gray-700 text-white focus:ring-2 focus:ring-cyan-500 outline-none"
                        />
                        {errors.description && <p className="text-red-400 text-sm mt-1">{errors.description}</p>}
                    </div>

                    <div className="pt-4 flex gap-3">
                        <button type="button" onClick={onClose}
                                className="w-full py-2.5 rounded-lg bg-gray-600 hover:bg-gray-700 font-semibold transition-colors">
                            {t("buttons.cancel")}
                        </button>
                        <button
                            type="submit"
                            disabled={isLoading || !isValid}
                            className={`w-full py-2.5 rounded-lg font-semibold transition-colors disabled:opacity-50 ${isDebit ? "bg-red-600 hover:bg-red-700" : "bg-green-600 hover:bg-green-700"}`}
                        >
                            {isLoading ? t("common.sending") : t("buttons.confirm")}
                        </button>
                    </div>
                </form>
            </motion.div>
        </motion.div>
    );
};
