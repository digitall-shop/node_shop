import React, {useState} from 'react';
import {useTranslation} from 'react-i18next';
import {useDirection} from '../../hooks/useDirection';
import {useTelegramUser} from '../../hooks/useTelegramUser';
import {usePanels, useSoftDeletePanel} from '../../hooks/usePanels';
import type {PanelDto} from '../../models/panel/panel';

import PanelCard from './panel/PanelCard';
import CreatePanelModal from './panel/CreatePanelModal';
import EditPanelModal from './panel/EditPanelModal';
import {showConfirm, showSuccessToast, showErrorToast, extractErrorMessage} from '../../utils/swal';

const PanelPage: React.FC = () => {
    const {t} = useTranslation();
    const dir = useDirection();
    const {user, loading} = useTelegramUser();

    const {data: panels, isLoading, isError} = usePanels(user?.id);
    const {mutate: deletePanel} = useSoftDeletePanel();

    const [isCreateModalOpen, setCreateModalOpen] = useState(false);
    const [isEditModalOpen, setEditModalOpen] = useState(false);
    const [selectedPanel, setSelectedPanel] = useState<PanelDto | null>(null);
    const [expandedPanelId, setExpandedPanelId] = useState<number | null>(null);

    const handleTogglePanel = (panelId: number) => {
        setExpandedPanelId(prev => (prev === panelId ? null : panelId));
    };

    const handleEdit = (panel: PanelDto) => {
        setSelectedPanel(panel);
        setEditModalOpen(true);
    };

    const handleDelete = (panel: PanelDto) => {
        const position = dir.isRtl ? 'top-start' : 'top-end';
        showConfirm({
            title: t('deletePanel.confirmTitle', {panelName: panel.name}),
            text: t('deletePanel.confirmText'),
            confirmText: t('deletePanel.confirmButton'),
            cancelText: t('deletePanel.cancelButton'),
            isRtl: dir.isRtl,
        }).then((res) => {
            if (res.isConfirmed) {
                deletePanel(panel.id, {
                    onSuccess: () => {
                        showSuccessToast(t('deletePanel.successTitle'), position);
                    },
                    onError: (error) => {
                        const msg = extractErrorMessage(error, t('deletePanel.errorText'));
                        showErrorToast(t('deletePanel.errorTitle'), position, {text: msg});
                    },
                });
            }
        });
    };

    if (loading) {
        return (
            <div className="container mx-auto px-4 py-8">
                <p className="text-center text-gray-400">{t('panelManagement.loading')}</p>
            </div>
        );
    }

    if (!user) {
        return (
            <div className="container mx-auto px-4 py-8">
                <p className="text-center text-red-500">{t('panelManagement.unauthorized') || 'برای مدیریت پنل‌ها باید وارد شوید.'}</p>
            </div>
        );
    }

    return (
        <div className="container mx-auto px-4 py-8">
            <div
                className={dir.classes('flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3 mb-6', dir.isRtl && 'sm:flex-row-reverse')}>
                <h1 className="text-2xl font-bold text-white truncate">{t('panelManagement.title')}</h1>
                <button
                    onClick={() => setCreateModalOpen(true)}
                    className={dir.classes('w-fit bg-cyan-500 hover:bg-cyan-600 text-white font-bold py-2 px-4 rounded-lg flex items-center gap-2 transition-all shadow-lg shadow-cyan-500/20', dir.isRtl)}
                >
                    <i className="fas fa-plus"/>{t('panelManagement.addPanelButton')}
                </button>
            </div>

            {isLoading && <div
                className="self-start sm:self-auto w-5 h-5 border-2 border-cyan-400 border-t-transparent justify-self-center rounded-full animate-spin"></div>}
            {isError && <p className="text-center text-red-500">{t('panelManagement.error')}</p>}

            <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 xl:grid-cols-3">
                {panels && panels.length > 0 ? (
                    panels.map((panel) => (
                        <PanelCard
                            key={panel.id}
                            panel={panel}
                            isExpanded={expandedPanelId === panel.id}
                            onToggle={() => handleTogglePanel(panel.id)}
                            onEdit={() => handleEdit(panel)}
                            onDelete={() => handleDelete(panel)}
                        />
                    ))
                ) : (
                    !isLoading && !isError && (
                        <div className="sm:col-span-2 xl:col-span-3 text-center py-10 px-6 bg-gray-800/20 rounded-lg">
                            <i className="fas fa-folder-open text-4xl text-gray-500 mb-4"/>
                            <h3 className="font-bold text-lg text-white">{t('panelManagement.empty.title')}</h3>
                            <p className="text-gray-400">{t('panelManagement.empty.message')}</p>
                        </div>
                    )
                )}
            </div>

            <CreatePanelModal isOpen={isCreateModalOpen} onClose={() => setCreateModalOpen(false)}/>
            <EditPanelModal isOpen={isEditModalOpen} onClose={() => setEditModalOpen(false)} panel={selectedPanel}/>
        </div>
    );
};

export default PanelPage;
