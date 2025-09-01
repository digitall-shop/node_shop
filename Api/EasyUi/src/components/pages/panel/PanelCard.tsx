import React from 'react';
import {Link} from 'react-router-dom';
import {useTranslation} from 'react-i18next';
import {useDirection} from '../../../hooks/useDirection.ts';
import type {PanelDto} from '../../../models/panel/panel.ts';

interface PanelCardProps {
    panel: PanelDto;
    isExpanded: boolean;
    onToggle: () => void;
    onEdit: () => void;
    onDelete: () => void;
}

const PanelCard: React.FC<PanelCardProps> = ({panel, isExpanded, onToggle, onEdit, onDelete}) => {
    const {t} = useTranslation();
    const dir = useDirection();
    const detailsId = `panel-${panel.id}-details`;

    return (
        <div
            className="bg-gray-800/50 backdrop-blur-sm border border-gray-700/60 rounded-xl overflow-hidden shadow-lg transition-colors">
            <button
                onClick={onToggle}
                aria-expanded={isExpanded}
                aria-controls={detailsId}
                className={dir.classes(
                    'w-full px-4 py-3 flex justify-between items-center hover:bg-gray-700/40 transition-colors',
                    dir.styles.text.start
                )}
            >
                <div className="min-w-0 flex-1">
                    <h3 className="font-bold text-white truncate" title={panel.name}>{panel.name}</h3>
                    <p className="text-sm text-gray-400 truncate" title={panel.url}>{panel.url}</p>
                </div>
                <i
                    className={
                        'fas fa-chevron-down text-xs text-gray-400 ms-4 transform transition-transform duration-200 ' +
                        (isExpanded ? 'rotate-180' : '')
                    }
                />
            </button>

            {isExpanded && (
                <div id={detailsId} className="border-t border-gray-700/50 p-4 space-y-4">
                    <div
                        className={dir.classes('flex justify-between items-center gap-3', dir.isRtl && 'flex-row-reverse')}>
                        <div className={dir.styles.text.start}>
                            <p className="text-xs text-gray-400">{t('panelManagement.card.NameLabel')}</p>
                            <p className="font-mono text-white break-words">{panel.name}</p>
                        </div>

                        <div className={dir.classes('flex items-center gap-2', dir.isRtl && 'flex-row-reverse')}>
                            <button
                                onClick={onEdit}
                                className="h-9 w-9 flex items-center justify-center rounded-full bg-blue-500/20 hover:bg-blue-500/40 text-blue-400 transition-colors"
                                aria-label={t('panelManagement.card.ariaEdit')}
                                type="button"
                            >
                                <i className="fas fa-pencil-alt text-sm"/>
                            </button>

                            <button
                                onClick={onDelete}
                                className="h-9 w-9 flex items-center justify-center rounded-full bg-red-500/20 hover:bg-red-500/40 text-red-400 transition-colors"
                                aria-label={t('panelManagement.card.ariaDelete')}
                                type="button"
                            >
                                <i className="fas fa-trash-alt text-sm"/>
                            </button>

                            <Link to={`/panel/${panel.id}`} aria-label={t('panelManagement.card.ariaSettings')}>
                                <div
                                    className="h-9 w-9 flex items-center justify-center rounded-full bg-cyan-500/20 hover:bg-cyan-500/40 text-cyan-400 transition-colors">
                                    <i className="fas fa-cog text-sm"/>
                                </div>
                            </Link>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default PanelCard;