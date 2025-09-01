import {Outlet, useNavigate} from 'react-router-dom';
import {useTranslation} from 'react-i18next';
import {useDirection} from '../../hooks/useDirection';
import PanelDetailFooter from "./footer/PanelDetailFooter.tsx";
import HeaderComponent from "./header/HeaderComponent.tsx";
import {ArrowLeft, ArrowRight} from "lucide-react";

const PanelDetailLayout = () => {
    const {t} = useTranslation();
    const dir = useDirection();
    const navigate = useNavigate();
    const BackIcon = dir.isRtl ? ArrowRight : ArrowLeft;
    const cn = (...v: Array<string | false | undefined>) => v.filter(Boolean).join(' ');

    return (
        <>
            <HeaderComponent/>

            <main className="pt-8 pb-24 container mx-auto px-4">
                <div className={cn("mb-6 flex", dir.isRtl ? "justify-start" : "justify-start")}>
                    <button
                        onClick={() => navigate('/panel')}
                        className={cn(
                            "inline-flex items-center gap-2 text-sm text-indigo-400 hover:text-indigo-300 transition-colors",
                            dir.isRtl && "flex-row"
                        )}
                        aria-label={t('panelDetails.backButton')}
                    >
                        <BackIcon className=" size-4 "/>
                        {t('panelDetails.backButton')}
                    </button>
                </div>
                <Outlet/>
            </main>
            <PanelDetailFooter/>
        </>
    );
};

export default PanelDetailLayout;
