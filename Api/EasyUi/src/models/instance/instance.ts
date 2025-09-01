import type {BaseResponse} from "../common/common.ts";

export interface PanelNodeDto {
    id: number;
    status: number;
    createDate: string;
    nodeName: string;
}

export interface ManualTheInstance {
    isSuccess: boolean;
    statusCode: number;
    jsonValidationMessage?: string | null;
    message?: string;
    data?: number | null;
}


export type GetPanelNodesResponse = BaseResponse<PanelNodeDto[]>;

export type DeleteInstanceResponse = BaseResponse<null>;