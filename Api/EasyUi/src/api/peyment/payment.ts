
import api from "../common/axiosInstance";
import type { BaseResponse } from "../../models/common/common";
import type {
    AcceptPaymentPayload,
    AcceptPaymentResponse,
    GetPaymentRequestsParams,
    NewPaymentRequest,
    NewPaymentResponseData, PagedResult,
    PaymentRequest,
    RejectPaymentPayload,
} from "../../models/peyment/payment.ts";
import {isSuperAdmin} from "../../models/auth.utils.ts";

export const addPaymentRequest = async (payload: NewPaymentRequest): Promise<NewPaymentResponseData> => {
    const { data } = await api.post<BaseResponse<NewPaymentResponseData>>("/payment", payload);
    if (!data?.data) throw new Error("ایجاد درخواست پرداخت ناموفق بود. پاسخ دریافتی فاقد داده است.");
    return data.data;
};

export const acceptPaymentRequest = async ({
                                               paymentRequestId,
                                               receipt,
                                           }: AcceptPaymentPayload): Promise<AcceptPaymentResponse | null> => {
    const formData = new FormData();
    formData.append("receipt", receipt);
    const { data } = await api.post<BaseResponse<AcceptPaymentResponse>>(
        `/payment/${paymentRequestId}/accept`,
        formData,
        { headers: { "Content-Type": "multipart/form-data" } }
    );
    return data.data;
};

export const approvePaymentRequest = async (id: number): Promise<NewPaymentResponseData> => {
    if (!isSuperAdmin()) throw new Error("Forbidden: فقط سوپرادمین می‌تواند تأیید کند.");
    const { data } = await api.post<BaseResponse<NewPaymentResponseData>>(`/payment/${id}/approve`);
    if (!data?.data) throw new Error("تأیید درخواست پرداخت ناموفق بود.");
    return data.data;
};

export const rejectPaymentRequest = async (id: number, payload: RejectPaymentPayload): Promise<NewPaymentResponseData> => {
    if (!isSuperAdmin()) throw new Error("Forbidden: فقط سوپرادمین می‌تواند رد کند.");
    const { data } = await api.post<BaseResponse<NewPaymentResponseData>>(`/payment/${id}/reject`, payload);
    if (!data?.data) throw new Error("رد درخواست پرداخت ناموفق بود.");
    return data.data;
};

export const getAllPaymentRequestsForAdmin = async (
    params: GetPaymentRequestsParams = {}
): Promise<PagedResult<PaymentRequest>> => {
    const { data } = await api.get<BaseResponse<PagedResult<PaymentRequest>>>(`/payment/admin`, { params });
    if (!data?.data) throw new Error("دریافت لیست پرداخت‌ها (ادمین) ناموفق بود.");
    return data.data;
};

export const getAllUserPaymentRequests = async (
    params: GetPaymentRequestsParams = {}
): Promise<PagedResult<PaymentRequest>> => {
    const { data } = await api.get<BaseResponse<PagedResult<PaymentRequest>>>(`/payment/user`, { params });
    if (!data?.data) throw new Error("دریافت لیست پرداخت‌ها (کاربر) ناموفق بود.");
    return data.data;
};

export const getPaymentRequestByIdForAdmin = async (id: number): Promise<PaymentRequest> => {
    const { data } = await api.get<BaseResponse<PaymentRequest>>(`/payment/admin/${id}`);
    if (!data?.data) throw new Error("درخواست پرداخت مورد نظر یافت نشد.");
    return data.data;
};

export const getPaymentRequestByIdForUser = async (id: number): Promise<PaymentRequest> => {
    const { data } = await api.get<BaseResponse<PaymentRequest>>(`/payment/user/${id}`);
    if (!data?.data) throw new Error("درخواست پرداخت مورد نظر یافت نشد.");
    return data.data;
};

export const getPaymentRequests = async (
    params: GetPaymentRequestsParams = {}
): Promise<PagedResult<PaymentRequest>> => {
    return isSuperAdmin() ? getAllPaymentRequestsForAdmin(params) : getAllUserPaymentRequests(params);
};

export const getPaymentRequestById = async (id: number): Promise<PaymentRequest> => {
    return isSuperAdmin() ? getPaymentRequestByIdForAdmin(id) : getPaymentRequestByIdForUser(id);
};