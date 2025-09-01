import {motion} from "framer-motion";

export const SkeletonUserCard: React.FC = () => (
    <motion.div
        initial={{opacity: 0.35}}
        animate={{opacity: 1}}
        transition={{repeat: Infinity, repeatType: "reverse", duration: 0.9}}
        className="bg-gray-800/60 border border-gray-700 rounded-xl p-3 sm:p-4"
    >
        <div className="animate-pulse">
            <div className="h-6 w-48 bg-gray-700 rounded mb-2"/>
            <div className="h-4 w-24 bg-gray-700 rounded"/>
        </div>
    </motion.div>
);
