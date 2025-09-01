import {type ChangeEvent, useState} from 'react';
import {ErrorMessage, Field, Form, Formik} from 'formik';
import * as Yup from 'yup';
import {motion} from 'framer-motion';
import {useTranslation} from 'react-i18next';
import {useDirection} from '../../../hooks/useDirection.ts';
import type {PanelCreateDto} from '../../../models/panel/panel.ts';
import {InputField} from "../../ui/InputField.tsx";
import {PasswordField} from "../../ui/PasswordField.tsx";


const AllPanels = ({
                       initialValues,
                       onSubmit,
                       isPending,
                       submitButtonText,
                       formTitle,
                       formDescription,
                       onClose,
                   }: {
    initialValues: Omit<PanelCreateDto, 'userId'>;
    onSubmit: (values: Omit<PanelCreateDto, 'userId'>) => void;
    isPending: boolean;
    submitButtonText: string;
    formTitle: string;
    formDescription: string;
    onClose?: () => void;
}) => {
    const {t} = useTranslation();
    const dir = useDirection();

    const [useSsl, setUseSsl] = useState<boolean>(() =>
        initialValues.url ? initialValues.url.startsWith('https://') : true
    );

    const schema = Yup.object().shape({
        name: Yup.string().required(t('createPanelForm.validation.nameRequired')),
        url: Yup.string().required(t('createPanelForm.validation.urlRequired')),
        userName: Yup.string().required(t('createPanelForm.validation.usernameRequired')),
        password: Yup.string().required(t('createPanelForm.validation.passwordRequired')),
    });

    const handleSubmitWrapper = (values: Omit<PanelCreateDto, 'userId'>) => {
        const protocol = useSsl ? 'https://' : 'http://';
        const finalUrl = protocol + values.url.replace(/^https?:\/\//i, '');
        onSubmit({...values, url: finalUrl, ssl: useSsl});
    };

    const handleToggleChange = (e: ChangeEvent<HTMLInputElement>) => {
        setUseSsl(e.target.checked);
    };

    return (
        <div className="animate-fade-in">
            <div className={dir.classes('flex items-center gap-4 mb-6', dir.isRtl)}>
                <div className="bg-cyan-100 dark:bg-cyan-900/50 p-3 rounded-xl shadow-lg">
                    <i className="fas fa-server text-cyan-500 dark:text-cyan-300 text-2xl" aria-hidden="true"></i>
                </div>
                <div>
                    <h2 className="text-lg font-bold text-gray-100">
                        {formTitle || t('createPanelForm.title')}
                    </h2>
                    <p className={dir.classes('text-gray-400 text-sm', dir.styles.text.start)}>
                        {formDescription || t('createPanelForm.formDescription')}
                    </p>
                </div>
            </div>

            <Formik
                initialValues={{
                    ...initialValues,
                    url: (initialValues.url ?? '').replace(/^https?:\/\//i, ''),
                }}
                validationSchema={schema}
                onSubmit={handleSubmitWrapper}
                enableReinitialize
            >
                {() => (
                    <Form className="space-y-4">
                        <InputField
                            name="name"
                            label={t('createPanelForm.labels.panelName')}
                            placeholder={t('createPanelForm.placeholders.panelName')}
                        />

                        <div>
                            <label
                                htmlFor="url"
                                className={dir.classes('block text-sm font-medium text-gray-400 mb-1', dir.styles.text.start)}
                            >
                                {t('createPanelForm.labels.panelAddress')}
                            </label>
                            <div
                                dir="ltr"
                                className="flex items-center w-full rounded-lg bg-gray-800/70 border border-gray-600 focus-within:ring-2 focus-within:ring-cyan-500 transition-all"
                            >
                <span className="px-3 text-gray-400 text-sm select-none">
                  {useSsl ? 'https://' : 'http://'}
                </span>
                                <Field
                                    id="url"
                                    name="url"
                                    type="text"
                                    placeholder={useSsl ? 'example.com' : '192.168.1.1'}
                                    className={dir.classes(
                                        'w-full py-2.5 bg-transparent border-none focus:ring-0 text-gray-100 placeholder-gray-400',
                                        'text-left'
                                    )}
                                />
                            </div>
                            <ErrorMessage
                                name="url"
                                component="div"
                                className={dir.classes('text-red-500 text-xs mt-1', dir.styles.text.start)}
                            />
                        </div>

                        <label className="inline-flex items-center mb-1 cursor-pointer select-none">
                            <input
                                type="checkbox"
                                checked={useSsl}
                                onChange={handleToggleChange}
                                className="sr-only peer"
                                aria-label={useSsl ? t('createPanelForm.ssl.on') : t('createPanelForm.ssl.off')}
                            />
                            <div className="relative w-9 h-5 bg-gray-200 rounded-full
                              peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-blue-300
                              dark:peer-focus:ring-blue-800 dark:bg-gray-700
                              after:content-[''] after:absolute after:top-[2px] after:start-[2px]
                              after:bg-white after:border after:border-gray-300 after:rounded-full
                              after:h-4 after:w-4 after:transition-all
                              peer-checked:bg-blue-600 dark:peer-checked:bg-blue-600
                              peer-checked:after:translate-x-full rtl:peer-checked:after:-translate-x-full">
                            </div>
                            <span className="ms-3 text-sm font-medium  dark:text-gray-500">
                {useSsl ? t('createPanelForm.ssl.on') : t('createPanelForm.ssl.off')}
              </span>
                        </label>

                        <InputField
                            name="userName"
                            label={t('createPanelForm.labels.username')}
                            placeholder={t('createPanelForm.placeholders.username', 'admin')}
                        />

                        <PasswordField
                            name="password"
                            label={t('createPanelForm.labels.password')}
                            placeholder={t('createPanelForm.placeholders.password', '••••••••')}
                        />

                        <motion.button
                            whileHover={{scale: 1.02}}
                            whileTap={{scale: 0.98}}
                            type="submit"
                            disabled={isPending}
                            aria-label={isPending ? t('createPanelForm.buttons.creating') : t('createPanelForm.buttons.create')}
                            className="w-full py-3 bg-cyan-600 hover:bg-cyan-700 text-white font-semibold rounded-lg shadow-md
                         focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-offset-gray-900 focus:ring-cyan-500
                         transition-all disabled:bg-gray-500 disabled:cursor-not-allowed"
                        >
                            {isPending ? t('createPanelForm.buttons.creating') : (submitButtonText || t('createPanelForm.buttons.create'))}
                        </motion.button>

                        {onClose && (
                            <motion.button
                                type="button"
                                onClick={onClose}
                                whileHover={{scale: 1.02}}
                                whileTap={{scale: 0.98}}
                                className="w-full mt-2 py-2.5 bg-gray-600 hover:bg-gray-700 text-white font-semibold rounded-lg shadow-md
                           focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-offset-gray-900 focus:ring-gray-500 transition-all"
                            >
                                {t('buttons.close')}
                            </motion.button>
                        )}
                    </Form>
                )}
            </Formik>
        </div>
    );
};

export default AllPanels;
