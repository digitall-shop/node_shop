import React from "react";
import {useTranslation} from "react-i18next";
import {useQueryClient} from "@tanstack/react-query";
import {useTelegramUser} from "../../../hooks/useTelegramUser";
import {useCreatePanel} from "../../../hooks/usePanels";
import type {PanelCreateDto} from "../../../models/panel/panel";
import ModalWrapper from "../../ui/ModalWrapper";
import PanelForm from "./PanelForm";

import {useDirection} from "../../../hooks/useDirection";
import {showSuccessToast, showErrorModal, extractErrorMessage} from "../../../utils/swal";

interface CreatePanelModalProps {
    isOpen: boolean;
    onClose: () => void;
}

const CreatePanelModal: React.FC<CreatePanelModalProps> = ({isOpen, onClose}) => {
    const {t} = useTranslation();
    const dir = useDirection();
    const toastPosition = dir.isRtl ? "top-start" : "top-end";

    const {user, loading} = useTelegramUser();
    const queryClient = useQueryClient();
    const {mutate: createPanel, isPending} = useCreatePanel();

    const handleSubmit = (values: Omit<PanelCreateDto, "userId">) => {
        if (!user?.id) {
            showErrorModal(
                t("createPanelForm.alerts.errorTitle"),
                t("createPanelForm.alerts.userAuthError"),
                {confirmText: t("common.ok"), isRtl: dir.isRtl}
            );
            return;
        }

        createPanel(
            {...values, userId: user.id},
            {
                onSuccess: () => {
                    showSuccessToast(t("createPanelForm.alerts.successTitle"), toastPosition);
                    queryClient.invalidateQueries({queryKey: ["panels", user.id]});
                    onClose();
                },
                onError: (error) => {
                    const errorMessage = extractErrorMessage(error, t("createPanelForm.alerts.defaultError"));
                    showErrorModal(
                        t("createPanelForm.alerts.errorTitle"),
                        errorMessage,
                        {confirmText: t("common.ok"), isRtl: dir.isRtl}
                    );
                },
            }
        );
    };

    return (
        <ModalWrapper isOpen={isOpen} onClose={onClose}>
            <PanelForm
                isPending={isPending || loading}
                onSubmit={handleSubmit}
                initialValues={{name: "", url: "", userName: "", password: "", ssl: true}}
                formTitle={t("createPanelForm.title")}
                formDescription={t("createPanelForm.formDescription")}
                submitButtonText={t("createPanelForm.buttons.create")}
                onClose={onClose}
            />
        </ModalWrapper>
    );
};

export default CreatePanelModal;
