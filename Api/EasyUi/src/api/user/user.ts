import api from "../common/axiosInstance.ts";
import type {
    BlockUserPathParams,
    BlockUserResponse,
    GetUsersParams, UnblockUserPathParams,
    UnblockUserResponse,
    UserBalance
} from "../../models/user/user.ts";
import type { BaseResponse } from "../../models/common/common.ts";
import type { User, PaginatedUsers } from "../../models/user/user.ts";
import { isSuperAdmin } from "../../models/auth.utils.ts";
import axiosInstance from "../common/axiosInstance.ts";


export const getUserBalance = async (userId: string | number): Promise<UserBalance> => {
    try {
        const response = await api.get<UserBalance>(`/user/balance/${userId}`);
        return response.data;
    } catch (error) {
        console.error("User Not Found:", error);
        throw error;
    }
};


const getAllUsersForAdmin = async (
    filters: GetUsersParams = {}
): Promise<PaginatedUsers> => {
    const { data } = await api.get<BaseResponse<PaginatedUsers>>(
        "/user",
        { params: filters }
    );

    if (!data?.data) {
        throw new Error("Users list response was null");
    }

    return data.data;
};

export const getUsers = async (
    filters: GetUsersParams = {}
): Promise<PaginatedUsers> => {
    if (isSuperAdmin()) {
        return getAllUsersForAdmin(filters);
    }
    throw new Error("Permission Denied: Only admins can get the list of users.");
};

export const getUserById = async (userId: number): Promise<User> => {

    const { data } = await api.get<BaseResponse<User>>(`/user/user/${userId}`);

    if (!data?.data) {
        throw new Error(`User with ID ${userId} response was null`);
    }

    return data.data;
};

export async function apiBlockUser(
    userId: BlockUserPathParams["userId"],
    options?: { signal?: AbortSignal }
): Promise<BlockUserResponse> {
    const { data } = await axiosInstance.post<BlockUserResponse>(
        `/user/${userId}/block`,
        undefined,
        { signal: options?.signal }
    );
    return data;
}


export async function apiUnblockUser(
    userId: UnblockUserPathParams["userId"],
    options?: { signal?: AbortSignal }
): Promise<UnblockUserResponse> {
    const { data } = await axiosInstance.post<UnblockUserResponse>(
        `/user/${userId}/unblock`,
        undefined,
        { signal: options?.signal }
    );
    return data;
}

export function apiSetUserBlocked(
    userId: number,
    blocked: boolean,
    options?: { signal?: AbortSignal }
) {
    return blocked ? apiBlockUser(userId, options) : apiUnblockUser(userId, options);
}