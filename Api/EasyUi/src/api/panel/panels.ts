import api from '../common/axiosInstance';
import type {BaseResponse} from '../../models/common/common';
import type {PanelDto, PanelCreateDto, PanelUpdateDto} from '../../models/panel/panel';

export const getPanels = async (userId?: number): Promise<PanelDto[]> => {
    const response = await api.get<BaseResponse<PanelDto[]>>('/panel', {
        params: {userId},
    });
    return response.data.data ?? [];
};

export const getPanelUser = async (userId?: number): Promise<PanelDto[]> => {
    const response = await api.get<BaseResponse<PanelDto[]>>('/panel/user', {
        params: {userId},
    });
    return response.data.data ?? [];
};

export const getPanelById = async (id: number): Promise<PanelDto> => {
    const response = await api.get<BaseResponse<PanelDto>>(`/panel/${id}`);
    if (!response.data.data) {
        throw new Error('پنل پیدا نشد یا پاسخ نامعتبر بود.');
    }
    return response.data.data;
};

export const createPanel = async (payload: PanelCreateDto): Promise<PanelDto> => {
    const response = await api.post<BaseResponse<PanelDto>>('/panel', payload);

    if (!response.data.data) {
        throw new Error('پنل ایجاد نشد. پاسخ دریافتی نامعتبر بود.');
    }

    return response.data.data;
};

export const updatePanel = async (
    id: number,
    payload: PanelUpdateDto
): Promise<PanelDto> => {
    const response = await api.patch<BaseResponse<PanelDto>>(`/panel/${id}`, payload);

    if (!response.data.data) {
        throw new Error('به‌روزرسانی پنل ناموفق بود. پاسخ دریافتی نامعتبر است.');
    }

    return response.data.data;
};

export const softDeletePanel = async (id: number): Promise<void> => {
    const response = await api.delete<BaseResponse<null>>(`/panel/${id}`);

    if (!response.data.isSuccess) {
        throw new Error(response.data.message || 'حذف نرم پنل ناموفق بود.');
    }
};
