import {useMutation, useQuery, useQueryClient} from '@tanstack/react-query';
import {
    createPanel,
    getPanelById,
    getPanels,
    getPanelUser,
    softDeletePanel,
    updatePanel
} from '../api/panel/panels';
import type {PanelCreateDto, PanelDto, PanelUpdateDto} from '../models/panel/panel';
import {isSuperAdmin} from '../models/auth.utils';

export const usePanels = (userId: number | undefined) => {
    const isAdmin = isSuperAdmin();

    const queryFn = () => (isAdmin ? getPanels(userId) : getPanelUser(userId));

    const queryKey = ['panels', isAdmin ? 'admin' : 'user', userId];

    return useQuery<PanelDto[], Error>({
        queryKey: queryKey,
        queryFn: queryFn,
        enabled: !!userId,
    });
};

export const usePanelById = (panelId: number | undefined) => {
    return useQuery<PanelDto, Error>({
        queryKey: ['panel', panelId],
        queryFn: () => getPanelById(panelId!),
        enabled: !!panelId,
    });
};


export const useCreatePanel = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: PanelCreateDto) => createPanel(data),
        onSuccess: () => {
            queryClient.invalidateQueries({queryKey: ['panels']});
        },
    });
};

export const useUpdatePanel = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({id, payload}: { id: number, payload: PanelUpdateDto }) => updatePanel(id, payload),
        onSuccess: (data, variables) => {
            queryClient.invalidateQueries({queryKey: ['panels']});
            queryClient.setQueryData(['panel', variables.id], data);
        },
    });
};

export const useSoftDeletePanel = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (id: number) => softDeletePanel(id),
        onSuccess: () => {
            queryClient.invalidateQueries({queryKey: ['panels']});
        },
    });
};