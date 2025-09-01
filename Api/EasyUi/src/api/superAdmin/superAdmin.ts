// src/api/superAdmin/superAdmin.ts
import type {BaseResponse} from "../../models/common/common";
import type {SubUserData} from "../../models/superAdmin/superAdmin";
import api from "../common/axiosInstance";

export const getSubUserInfo = async (): Promise<SubUserData> => {
    const { data } = await api.get<BaseResponse<SubUserData>>('/user/user');
    if (!data?.data) throw new Error("پاسخ سرور فاقد داده است.");
    return data.data;
};
