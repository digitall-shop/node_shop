import type {BaseResponse} from "../common/common.ts";

export interface UserBalance {
    data: number;
    isSuccess: boolean;
    statusCode: number;
    jsonValidationMessage?: string | null;
    message?: string;
}

export interface User {
    id: number;
    userName: string | null;
    firstName: string | null;
    lastName: string | null;
    isSuperAdmin: boolean;
    balance: number;
    isBlocked: boolean;
}

export interface PaginatedUsers {
    page: number;
    take: number;
    totalCount: number;
    items: User[];
}

export interface GetUsersParams {
    userId?: number;
    userName?: string;
    dateFrom?: string;
    dateTo?: string;
    fromBalance?: number;
    toBalance?: number;
    orderBy?: string;
    orderDir?: 'asc' | 'desc';
    skip?: number;
    take?: number;
}

export interface BlockUserPathParams {
    userId: number;
}
export interface UnblockUserPathParams {
    userId: number;
}

export type BlockUserResponse = BaseResponse<null>;
export type UnblockUserResponse = BaseResponse<null>;


export type GetUserByIdResponse = BaseResponse<User>;

export type GetAllUsersResponse = BaseResponse<PaginatedUsers>;