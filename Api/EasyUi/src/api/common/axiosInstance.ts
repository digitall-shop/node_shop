import axios, {
    type AxiosError,
    type AxiosInstance,
    type AxiosResponse,
} from 'axios';
import type { BaseResponse } from '../../models/common/common.ts';

const baseURL = import.meta.env.VITE_API_BASE_URL;
const ApiBaseUrl = `${baseURL}`;
if (!baseURL) throw new Error("VITE_API_BASE_URL تعریف نشده است.");

const api: AxiosInstance = axios.create({
    baseURL:ApiBaseUrl,
    timeout: 10000,
});

api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

api.interceptors.response.use(
    (response: AxiosResponse<BaseResponse<unknown>>) => {
        const base = response.data;

        if (typeof base === 'object' && 'isSuccess' in base && !base.isSuccess) {
            const message = base.message || 'خطای نامشخص از سمت سرور';
            return Promise.reject(new Error(message));
        }

        return response;
    },
    (error: AxiosError<BaseResponse<unknown>>) => {
        const status = error.response?.status;
        const res = error.response?.data;

        if (status === 404) {
            console.warn("آدرس یافت نشد.");
        }

        const fallbackMessage = error.message || "خطایی در ارتباط با سرور رخ داد.";
        const apiMessage = res?.message || fallbackMessage;

        return Promise.reject(new Error(apiMessage));
    }
);

export default api;
