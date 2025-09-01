import type {InitiateNode, NodeDto} from "../../models/nodes/node.ts";
import type { BaseResponse } from "../../models/common/common.ts";
import api from "../common/axiosInstance.ts";

export const getNodes = async (): Promise<NodeDto[]> => {
    try {
        const response = await api.get<BaseResponse<NodeDto[]>>('/node/nodes');
        return response.data.data ?? [];
    } catch (error) {
        console.error("Failed to fetch nodes:", error);
        return [];
    }
};
export const getInitiateNode = async (): Promise<InitiateNode[]> => {
    try {
        const response = await api.get<BaseResponse<InitiateNode[]>>("/node/initiate-node");
        return response.data.data ?? [];
    } catch (error) {
        console.error("Failed to fetch nodes:", error);
        return [];
    }
};

export const initiateNode = async (
    panelId: number,
    nodeId: number
): Promise<BaseResponse<InitiateNode | null>> => {
    try {
        const res = await api.post<BaseResponse<InitiateNode>>("/node/initiate-node", {
            panelId,
            nodeId,
        });
        return res.data;
    } catch (error: any) {
        if (error?.response?.data) return error.response.data as BaseResponse<InitiateNode | null>;
        return {
            isSuccess: false,
            statusCode: -1,
            jsonValidationMessage: null,
            message: "Failed to initiate node",
            data: null,
        };
    }
};

export const getAdminNodes = async (): Promise<NodeDto[]> => {
    try {
        const response = await api.get<BaseResponse<NodeDto[]>>('/node/admin/nodes');
        return response.data.data ?? [];
    } catch (e) {
        console.error('Failed to fetch admin nodes', e);
        return [];
    }
};
