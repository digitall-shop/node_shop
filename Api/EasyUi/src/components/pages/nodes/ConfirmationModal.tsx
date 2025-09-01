import {motion, AnimatePresence} from "framer-motion";
import {useTranslation, Trans} from "react-i18next";
import {useDirection} from "../../../hooks/useDirection"; // Path might need adjustment
import type {SlimNode} from "../panelSetting/PanelDetailPage.tsx";

interface Props {
    isOpen: boolean;
    onClose: () => void;
    onConfirm: () => void;
    node: SlimNode | null;
    isSubmitting: boolean;
}

const ConfirmationModal = ({isOpen, onClose, onConfirm, node, isSubmitting}: Props) => {
    const {t, i18n} = useTranslation();
    const dir = useDirection();

    const formatPrice = (n: number) => {
        const locale = i18n.language === 'fa' ? 'fa-IR' : 'en-US';
        return new Intl.NumberFormat(locale).format(n);
    };

    if (!isOpen || !node) return null;

    return (
        <AnimatePresence>
            <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
                <motion.div
                    dir={dir.direction} // Set text direction
                    initial={{opacity: 0, scale: 0.9, y: 20}}
                    animate={{opacity: 1, scale: 1, y: 0}}
                    exit={{opacity: 0, scale: 0.9, y: 20}}
                    transition={{duration: 0.3, ease: "easeInOut"}}
                    className="bg-gray-900/70 border border-gray-700 rounded-2xl p-6 w-full max-w-sm shadow-2xl shadow-cyan-500/10"
                >
                    <div className="flex flex-col items-center text-center">
                        <div
                            className="h-16 w-16 mb-4 rounded-full bg-cyan-500/10 flex items-center justify-center border border-cyan-500/20">
                            <i className="fas fa-server text-3xl text-cyan-400"></i>
                        </div>
                        <h2 className="text-xl font-bold text-white mb-2">
                            {t("panelNodesPage.confirmationModal.title")}
                        </h2>
                        <p className="text-gray-400 mb-6">
                            <Trans
                                i18nKey="panelNodesPage.confirmationModal.message"
                                values={{nodeName: node.nodeName, price: formatPrice(node.price)}}
                                components={{
                                    1: <strong className="text-cyan-300 font-mono"/>,
                                    3: <strong className="text-cyan-300 font-mono"/>
                                }}
                            />
                        </p>
                    </div>

                    <div className="grid grid-cols-2 gap-3">
                        <motion.button
                            whileHover={{scale: 1.05}}
                            whileTap={{scale: 0.95}}
                            onClick={onClose}
                            disabled={isSubmitting}
                            className="py-2.5 rounded-lg bg-gray-700/50 hover:bg-gray-700/80 text-white font-semibold transition-colors disabled:opacity-50"
                        >
                            {t("buttons.close")}
                        </motion.button>
                        <motion.button
                            whileHover={{scale: 1.05}}
                            whileTap={{scale: 0.95}}
                            onClick={onConfirm}
                            disabled={isSubmitting}
                            className="py-2.5 rounded-lg bg-cyan-500 hover:bg-cyan-600 text-white font-bold transition-colors disabled:opacity-50 disabled:cursor-wait flex items-center justify-center gap-2"
                        >
                            {isSubmitting
                                ? t("panelNodesPage.confirmationModal.activating")
                                : t("panelNodesPage.confirmationModal.confirm")}
                        </motion.button>
                    </div>
                </motion.div>
            </div>
        </AnimatePresence>
    );
};

export default ConfirmationModal;