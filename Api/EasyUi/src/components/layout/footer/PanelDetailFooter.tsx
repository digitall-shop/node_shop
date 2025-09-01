import {useState, useEffect} from 'react';
import {useLocation, useNavigate, useParams} from 'react-router-dom';
import {motion, AnimatePresence} from 'framer-motion';
import {useTranslation} from 'react-i18next';

interface NavLinkItem {
    to: string;
    iconClass: string;
    label: string;
}

const PanelDetailFooter = () => {
    const {t} = useTranslation();
    const location = useLocation();
    const navigate = useNavigate();
    const {panelId} = useParams<{ panelId: string }>();
    const basePath = `/panel/${panelId}`;

    const allLinks: NavLinkItem[] = [
        {to: '', iconClass: 'fa-cube', label: t('panelDetails.nav.node')},
        {to: 'host', iconClass: 'fa-server', label: t('panelDetails.nav.host')},
        {to: 'shop', iconClass: 'fa-shopping-cart', label: t('panelDetails.nav.shop')},
    ];

    const getCurrentLink = () => {
        const currentPath = location.pathname;

        return allLinks.slice().reverse().find(link => currentPath.endsWith(link.to)) || allLinks[0];
    };

    const [centerItem, setCenterItem] = useState<NavLinkItem>(getCurrentLink());

    useEffect(() => {
        const currentLink = getCurrentLink();
        if (currentLink.to !== centerItem.to) {
            setCenterItem(currentLink);
        }
    }, [location.pathname]);

    const sideLinks = allLinks.filter((l) => l.to !== centerItem.to);

    const handleNav = (link: NavLinkItem) => {
        const destination = link.to ? `${basePath}/${link.to}` : basePath;
        navigate(destination);
    };

    if (sideLinks.length < 2) {
        return null;
    }

    return (
        <footer
            className="md:hidden fixed bottom-0 left-0 right-0 z-50 h-20 bg-gray-100 dark:bg-gray-900 border-t border-gray-300 dark:border-gray-700">
            <div className="relative h-full grid grid-cols-3 items-center px-4 max-w-md mx-auto">

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
                        className="absolute -top-10 left-1/2 w-20 h-20 rounded-full bg-cyan-500 text-white flex flex-col justify-center items-center shadow-xl shadow-cyan-500/30 border-4 border-gray-900 dark:border-black z-10"
                    >
                        <i className={`fas ${centerItem.iconClass} text-2xl`}/>
                        <span className="text-sm font-bold mt-1">{centerItem.label}</span>
                    </motion.button>
                </AnimatePresence>
            </div>
        </footer>
    );
};

export default PanelDetailFooter;
