import api from "../common/axiosInstance.ts";
import type {
    DeleteInstanceResponse,
    GetPanelNodesResponse,
    ManualTheInstance,
    PanelNodeDto
} from "../../models/instance/instance.ts";
import {ContainerProvisionStatus} from "../../models/instance/status.ts";


export const getPanelNodes = async (panelId: number): Promise<PanelNodeDto[]> => {
    const res = await api.get<GetPanelNodesResponse>(`/instance/panel/${panelId}`);
    return res.data.data ?? [];
};


export const togglePauseByUser = async (instanceId: number, status: number): Promise<ManualTheInstance> => {
    const isPaused = status === ContainerProvisionStatus.PausedByUser;
    const path = isPaused ? `/instance/${instanceId}/unpause` : `/instance/${instanceId}/pause`;
    const {data} = await api.post<ManualTheInstance>(path);
    return data;
};

export async function deleteInstance(instanceId: number): Promise<DeleteInstanceResponse> {
    const {data} = await api.delete<DeleteInstanceResponse>(`/instance/delete/${instanceId}`);
    return data;
}