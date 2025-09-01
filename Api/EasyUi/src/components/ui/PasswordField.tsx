import {useState} from "react";
import {useTranslation} from "react-i18next";
import {useDirection} from "../../hooks/useDirection.ts";
import {ErrorMessage, Field} from "formik";
import {FaEye, FaEyeSlash} from "react-icons/fa";

export const PasswordField = ({
                           name,
                           label,
                           placeholder,
                       }: {
    name: string;
    label: string;
    placeholder?: string;
}) => {
    const [show, setShow] = useState(false);
    const { t } = useTranslation();
    const dir = useDirection();

    return (
        <div>
            <label
                htmlFor={name}
                className={dir.classes('block text-sm font-medium text-gray-400 mb-1', dir.styles.text.start)}
            >
                {label}
            </label>

            <div className="relative">
                <Field
                    id={name}
                    name={name}
                    type={show ? 'text' : 'password'}
                    placeholder={placeholder}
                    className={dir.classes(
                        'w-full py-2.5 rounded-lg bg-gray-800/70 border border-gray-600 text-sm focus:ring-2 focus:ring-cyan-500 focus:border-cyan-500 outline-none transition-all text-white placeholder-gray-400',
                        dir.isRtl ? 'pl-12 pr-4' : 'pl-4 pr-12',
                        dir.styles.text.start
                    )}
                />

                <button
                    type="button"
                    onClick={() => setShow(s => !s)}
                    className="absolute inset-y-0 end-0 flex items-center z-20 px-3 cursor-pointer text-gray-500 hover:text-gray-800 focus:outline-none"
                    aria-label={show ? t('createPanelForm.password.hide', 'Hide password') : t('createPanelForm.password.show', 'Show password')}
                >
                    {show ? <FaEyeSlash className="h-4 w-4" /> : <FaEye className="h-4 w-4" />}
                </button>
            </div>

            <ErrorMessage
                name={name}
                component="div"
                className={dir.classes('text-red-500 text-xs mt-1', dir.styles.text.start)}
            />
        </div>
    );
};