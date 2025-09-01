import {useDirection} from "../../hooks/useDirection.ts";
import {ErrorMessage, Field} from "formik";

export const InputField = ({
                        name,
                        label,
                        placeholder,
                        type = 'text',
                    }: {
    name: string;
    label: string;
    placeholder?: string;
    type?: string;
}) => {
    const dir = useDirection();
    return (
        <div>
            <label
                htmlFor={name}
                className={dir.classes('block text-sm font-medium text-gray-400 mb-1', dir.styles.text.start)}
            >
                {label}
            </label>
            <Field
                id={name}
                name={name}
                type={type}
                placeholder={placeholder}
                className={dir.classes(
                    'w-full px-4 py-2.5 rounded-lg bg-gray-800/70 border border-gray-600 text-sm focus:ring-2 focus:ring-cyan-500 focus:border-cyan-500 outline-none transition-all text-white',
                    dir.styles.text.start
                )}
            />
            <ErrorMessage
                name={name}
                component="div"
                className={dir.classes('text-red-500 text-xs mt-1', dir.styles.text.start)}
            />
        </div>
    );
};