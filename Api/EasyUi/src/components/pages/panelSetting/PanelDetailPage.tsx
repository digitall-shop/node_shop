import {useState, useEffect, useMemo} from "react";
import {motion, AnimatePresence} from "framer-motion";
import {useTranslation} from "react-i18next";
import {useParams} from "react-router-dom";
import {useQuery} from "@tanstack/react-query";

import PanelNodeCard from "../nodes/PanelNodeCard.tsx";
import ServerNodeCard from "../nodes/ServerNodeCard.tsx";
import ConfirmationModal from "../nodes/ConfirmationModal.tsx";

import {usePanelNodes} from "../../../hooks/usePanelNodes.ts";
import {getNodes, initiateNode} from "../../../api/nodes/node.ts";
import type {NodeStatus} from "../../../models/nodes/node.ts";

import {togglePauseByUser, deleteInstance} from "../../../api/instance/instance.ts";

import {
    showSuccessToast,
    showErrorToast,
    extractErrorMessage,
} from "../../../utils/swal.ts";
import type {DeleteInstanceResponse} from "../../../models/instance/instance.ts";
import {useDirection} from "../../../hooks/useDirection.ts";

export type SlimNode = { id: number; nodeName: string; price: number; status: NodeStatus };

const PanelDetailPage = () => {
    const {t} = useTranslation();
    const dir = useDirection();
    const params = useParams();
    const panelId = Number(params.id ?? params.panelId);
    const isValidId = Number.isFinite(panelId) && panelId > 0;

    const [togglingId, setTogglingId] = useState<number | null>(null);
    const [deletingId, setDeletingId] = useState<number | null>(null);
    const [deleteTarget, setDeleteTarget] = useState<{ id: number; name: string } | null>(null);

    const {
        data: connectedNodes,
        isLoading: isLoadingConnected,
        isError,
        refetch,
    } = usePanelNodes(isValidId ? panelId : undefined);


    const [connectedList, setConnectedList] = useState<any[]>([]);
    useEffect(() => setConnectedList(connectedNodes ?? []), [connectedNodes]);

    const [allNodes, setAllNodes] = useState<SlimNode[]>([]);
    const [isLoadingAll, setLoadingAll] = useState(false);

    const [selectedNode, setSelectedNode] = useState<SlimNode | null>(null);
    const [isSubmitting, setSubmitting] = useState(false);

    const {
        data: deleteResp,
        isSuccess: deleteQSuccess,
        isError: deleteQError,
        error: deleteQErrorObj,
    } = useQuery<DeleteInstanceResponse>({
        queryKey: ["instance", "delete", deleteTarget?.id ?? null],
        queryFn: () => deleteInstance(deleteTarget!.id),
        enabled: !!deleteTarget,
        retry: false,
        staleTime: 0,
        gcTime: 0,
    });

    useEffect(() => {
        if (!deleteTarget) return;

        if (deleteQSuccess) {
            if (deleteResp?.isSuccess) {
                setConnectedList((prev) =>
                    prev.filter((n) => Number(n.id ?? n.instanceId) !== deleteTarget.id)
                );
                showSuccessToast(t("panelNodesPage.alerts.deleteSuccess", {name: deleteTarget.name}));
            } else {
                showErrorToast(deleteResp?.message || t("panelNodesPage.alerts.unknownServerError"));
            }
            setDeletingId(null);
            setDeleteTarget(null);
        } else if (deleteQError) {
            showErrorToast(
                extractErrorMessage(deleteQErrorObj, t("panelNodesPage.alerts.unexpectedError"))
            );
            setDeletingId(null);
            setDeleteTarget(null);
        }
    }, [deleteQSuccess, deleteQError, deleteResp, deleteQErrorObj, deleteTarget, t]);

    useEffect(() => {
        if (!isValidId) return;
        setLoadingAll(true);
        getNodes()
            .then((data) =>
                setAllNodes(
                    data.map((n) => ({
                        id: Number(n.id),
                        nodeName: n.nodeName,
                        price: n.price,
                        status: n.status as NodeStatus,
                    }))
                )
            )
            .catch(console.error)
            .finally(() => setLoadingAll(false));
    }, [isValidId, refetch]);

    const availableNodes = useMemo(() => {
        const connectedNodeNames = new Set((connectedList ?? []).map((n: any) => n.nodeName));
        return allNodes.filter((n) => !connectedNodeNames.has(n.nodeName));
    }, [allNodes, connectedList]);

    const handleTogglePause = async (node: any) => {
        const instanceId = Number(node.id ?? node.instanceId);
        if (!Number.isFinite(instanceId)) return;

        try {
            setTogglingId(instanceId);
            const res = await togglePauseByUser(instanceId, node.status);
            if (res.isSuccess) {
                await refetch?.();
                showSuccessToast(t("panelNodesPage.alerts.statusUpdateSuccess", {name: node.nodeName}));
            } else {
                showErrorToast(res.message || t("panelNodesPage.alerts.unknownServerError"));
            }
        } catch (e) {
            showErrorToast(extractErrorMessage(e, t("panelNodesPage.alerts.unexpectedError")));
        } finally {
            setTogglingId(null);
        }
    };

    const handleDelete = (node: any) => {
        const instanceId = Number(node.id ?? node.instanceId);
        if (!Number.isFinite(instanceId)) return;
        setDeletingId(instanceId);
        setDeleteTarget({id: instanceId, name: String(node.nodeName ?? instanceId)});
    };

    const handleSelectNode = (node: SlimNode) => setSelectedNode(node);

    const handleConfirmActivation = async () => {
        if (!selectedNode) return;
        setSubmitting(true);
        try {
            const res = await initiateNode(panelId, selectedNode.id);
            if (res.isSuccess) {
                await refetch?.();
                // Changed this line to use translation
                showSuccessToast(t("panelNodesPage.alerts.activationSuccess", {name: selectedNode.nodeName}));
            } else {
                showErrorToast(res.message || t("panelNodesPage.alerts.unknownServerError"));
            }
        } catch (error) {
            showErrorToast(extractErrorMessage(error, t("panelNodesPage.alerts.unexpectedError")));
        } finally {
            setSubmitting(false);
            setSelectedNode(null);
        }
    };

    const containerVariants = {
        hidden: {opacity: 0},
        visible: {opacity: 1, transition: {staggerChildren: 0.1}},
    };

    return (
        <div className="container mx-auto px-4 py-8">
            <div className="mb-12">
                <h1 className={dir.classes("text-2xl font-bold text-white mb-2", dir.styles.text.start)}>{t("panelNodesPage.title")}</h1>
                <p className={dir.classes("text-gray-400 mb-6", dir.styles.text.start)}>{t("panelNodesPage.subtitle")}</p>

                {isLoadingConnected && (
                    <p className="text-center text-gray-400">
                        {t("panelNodesPage.connectedNodes.loading")}
                    </p>
                )}
                {isError && (
                    <p className="text-center text-red-500">
                        {t("panelNodesPage.connectedNodes.error")}
                    </p>
                )}

                {!isLoadingConnected && !isError && (
                    <motion.div layout className="grid gap-4">
                        <AnimatePresence>
                            {(connectedList ?? []).length > 0 ? (
                                connectedList.map((node: any) => {
                                    const instanceId = Number(node.id ?? node.instanceId);
                                    const hasInstanceId = Number.isFinite(instanceId);
                                    return (
                                        <PanelNodeCard
                                            key={`${node.nodeName}-${node.createDate}-${instanceId}`}
                                            node={node}
                                            toggling={hasInstanceId && togglingId === instanceId}
                                            deleting={hasInstanceId && deletingId === instanceId}
                                            onTogglePause={hasInstanceId ? () => handleTogglePause(node) : undefined}
                                            onDelete={hasInstanceId ? () => handleDelete(node) : undefined}
                                        />
                                    );
                                })
                            ) : (
                                <div
                                    className="text-center py-10 px-6 bg-gray-800/30 rounded-lg border border-dashed border-gray-700">
                                    <i className="fas fa-folder-open text-4xl text-gray-500 mb-4"/>
                                    <h3 className="font-bold text-lg text-white">
                                        {t("panelNodesPage.connectedNodes.empty.title")}
                                    </h3>
                                    <p className="text-gray-400">
                                        {t("panelNodesPage.connectedNodes.empty.description")}
                                    </p>
                                </div>
                            )}
                        </AnimatePresence>
                    </motion.div>
                )}
            </div>

            <div>
                <h2 className={dir.classes("text-2xl font-bold text-white mb-2", dir.styles.text.start)}>
                    {t("panelNodesPage.availableNodes.title")}
                </h2>
                <p className={dir.classes("text-gray-400 mb-6", dir.styles.text.start)}>{t("panelNodesPage.availableNodes.subtitle")}</p>

                {isLoadingAll && (
                    <p className="text-center text-gray-400">
                        {t("panelNodesPage.availableNodes.loading")}
                    </p>
                )}

                {!isLoadingAll && (
                    <motion.div
                        variants={containerVariants}
                        initial="hidden"
                        animate="visible"
                        className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3"
                    >
                        {allNodes.map((node) => {
                            const isConnected = !availableNodes.some((an) => an.id === node.id);
                            return (
                                <ServerNodeCard
                                    key={node.id}
                                    node={node}
                                    isConnected={isConnected}
                                    onSelect={() => handleSelectNode(node)}
                                />
                            );
                        })}
                    </motion.div>
                )}
            </div>

            <ConfirmationModal
                isOpen={!!selectedNode}
                onClose={() => setSelectedNode(null)}
                onConfirm={handleConfirmActivation}
                node={selectedNode}
                isSubmitting={isSubmitting}
            />
        </div>
    );
};

export default PanelDetailPage;