import React from 'react';
import {useTranslation} from 'react-i18next';
import {useUpdatePanel} from '../../../hooks/usePanels.ts';
import type {PanelDto, PanelUpdateDto} from '../../../models/panel/panel.ts';
import ModalWrapper from '../../ui/ModalWrapper.tsx';
import AllPanels from './PanelForm.tsx';
import {useDirection} from '../../../hooks/useDirection';
import { showSuccessToast, showErrorToast, extractErrorMessage } from '../../../utils/swal';

interface EditPanelModalProps {
    isOpen: boolean;
    onClose: () => void;
    panel: PanelDto | null;
}

const EditPanelModal: React.FC<EditPanelModalProps> = ({isOpen, onClose, panel}) => {
    const {t} = useTranslation();
    const dir = useDirection();
    const toastPosition = dir.isRtl ? 'top-start' : 'top-end';

    const {mutate: updatePanel, isPending} = useUpdatePanel();

    const handleSubmit = (values: PanelUpdateDto) => {
        if (!panel) return;
        updatePanel(
            { id: panel.id, payload: values },
            {
                onSuccess: () => {
                    showSuccessToast(t('updatePanelForm.alerts.successTitle'), toastPosition);
                    onClose();
                },
                onError: (error: unknown) => {
                    const msg = extractErrorMessage(error, t('updatePanelForm.alerts.defaultError'));
                    showErrorToast(t('updatePanelForm.alerts.errorTitle'), toastPosition, { text: msg });
                },
            }
        );
    };

    if (!panel) return null;

    return (
        <ModalWrapper isOpen={isOpen} onClose={onClose}>
            <AllPanels
                isPending={isPending}
                onSubmit={handleSubmit}
                initialValues={panel}
                formTitle={t('updatePanelForm.title')}
                formDescription={t('updatePanelForm.formDescription')}
                submitButtonText={t('updatePanelForm.buttons.update')}
                onClose={onClose}
            />
        </ModalWrapper>
    );
};

export default EditPanelModal;
