import {motion} from "framer-motion";
import {FaUserCircle} from "react-icons/fa";
import {useTranslation} from "react-i18next";

export const EmptyState = () => {
    const {t} = useTranslation();
    return (
        <motion.div initial={{opacity: 0}} animate={{opacity: 1}}
                    className="text-center py-10 px-6 bg-gray-800/20 rounded-lg col-span-full">
            <FaUserCircle className="mx-auto text-4xl text-gray-500 mb-4"/>
            <h3 className="font-bold text-lg text-white">{t("usersPage.empty.title")}</h3>
            <p className="text-gray-400">{t("usersPage.empty.desc")}</p>
        </motion.div>
    );
};
