import {useState} from "react";
import {useTranslation} from "react-i18next";

type FormInputs = { amount: string; description: string };
export const useBalanceForm = (isDebit: boolean) => {
    const {t} = useTranslation();
    const [formData, setFormData] = useState<FormInputs>({amount: "", description: ""});
    const [errors, setErrors] = useState<Partial<FormInputs>>({});

    const validate = () => {
        const next: Partial<FormInputs> = {};
        const n = Number(formData.amount);
        if (!formData.amount || isNaN(n) || n <= 0) next.amount = t("usersPage.modal.validation.positiveAmount");
        if (isDebit && !formData.description.trim())
            next.description = t("usersPage.modal.validation.descriptionRequired");
        setErrors(next);
        return Object.keys(next).length === 0;
    };

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const {name, value} = e.target;
        setFormData((p) => ({...p, [name]: value}));
    };

    return {
        formData,
        errors,
        handleChange,
        validate,
        isValid:
            Number(formData.amount) > 0 &&
            (!isDebit || formData.description.trim() !== "") &&
            Object.keys(errors).length === 0,
    };
};
