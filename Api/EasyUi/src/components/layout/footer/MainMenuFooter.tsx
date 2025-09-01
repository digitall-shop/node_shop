import {useState, useEffect} from 'react';
import {useLocation, useNavigate} from 'react-router-dom';
import {motion, AnimatePresence} from 'framer-motion';
import {useTranslation} from 'react-i18next';

interface NavLinkItem {
    to: string;
    iconClass: string;
    label: string;
}

const MainMenuFooter = () => {
    const {t} = useTranslation();
    const location = useLocation();
    const navigate = useNavigate();


    const allLinks: NavLinkItem[] = [
        {to: '/profile', iconClass: 'fa-user', label: t('footerNav.profile')},
        {to: '/', iconClass: 'fa-home', label: t('footerNav.home')},
        {to: '/panel', iconClass: 'fa-network-wired', label: t('footerNav.panel')},
    ];

    const [centerItem, setCenterItem] = useState<NavLinkItem>(
        allLinks.find((l) => l.to === location.pathname) || allLinks[1]
    );

    useEffect(() => {
        const found = allLinks.find((l) => l.to === location.pathname);
        if (found && found.to !== centerItem.to) {
            setCenterItem(found);
        }
    }, [location.pathname, centerItem.to, allLinks]);

    const sideLinks = allLinks.filter((l) => l.to !== centerItem.to);

    const handleNav = (link: NavLinkItem) => {
        navigate(link.to);
    };


    if (sideLinks.length < 2) {
        return null;
    }

    return (
        <footer
            className="md:hidden fixed bottom-0 left-0 right-0 z-50 h-16 bg-gray-100 dark:bg-gray-900 border-t border-gray-300 dark:border-gray-700">
            <div className="relative h-full grid grid-cols-3 items-center max-w-md mx-auto">
                <div className="flex justify-center">
                    <button
                        key={sideLinks[0].to}
                        onClick={() => handleNav(sideLinks[0])}
                        className="flex flex-col items-center text-gray-700 dark:text-gray-300 gap-1"
                    >
                        <i className={`fas ${sideLinks[0].iconClass} text-xl`}/>
                        <span className="text-xs">{sideLinks[0].label}</span>
                    </button>
                </div>

                <div></div>

                <div className="flex justify-center">
                    <button
                        key={sideLinks[1].to}
                        onClick={() => handleNav(sideLinks[1])}
                        className="flex flex-col items-center text-gray-700 dark:text-gray-300 gap-1"
                    >
                        <i className={`fas ${sideLinks[1].iconClass} text-xl`}/>
                        <span className="text-xs">{sideLinks[1].label}</span>
                    </button>
                </div>

                <AnimatePresence mode="wait">
                    <motion.button
                        key={centerItem.to}
                        onClick={() => handleNav(centerItem)}
                        initial={{scale: 0.7, opacity: 0, x: "-50%"}}
                        animate={{scale: 1, opacity: 1, x: "-50%"}}
                        exit={{scale: 0.7, opacity: 0, x: "-50%"}}
                        transition={{duration: 0.3}}
                        className="absolute -top-10 left-1/2 w-20 h-20 rounded-full bg-cyan-400/50 text-white flex flex-col justify-center items-center shadow-xl shadow-cyan-500/30 border-4 border-gray-900 dark:border-black z-10"
                    >
                        <i className={`fas ${centerItem.iconClass} text-2xl`}/>
                        <span className="text-sm font-bold mt-1">{centerItem.label}</span>
                    </motion.button>
                </AnimatePresence>
            </div>
        </footer>
    );
};

export default MainMenuFooter;
