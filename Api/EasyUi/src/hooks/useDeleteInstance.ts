import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import type {DeleteInstanceResponse} from "../models/instance/instance.ts";
import { deleteInstance } from "../api/instance/instance.ts";


export function useDeleteInstance() {
    const [id, setId] = useState<number | null>(null);

    const q = useQuery<DeleteInstanceResponse>({
        queryKey: ["instance", "delete", id],
        queryFn: () => deleteInstance(id as number),
        enabled: id !== null,
        staleTime: 0,
        gcTime: 0,
        retry: false,
    });

    const run = async (instanceId: number) => {
        setId(instanceId);
        const res = await q.refetch();
        setId(null);
        return res.data;
    };

    return { ...q, run };
}
