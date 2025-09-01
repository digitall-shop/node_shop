import {useQuery} from "@tanstack/react-query";
import {getPanelNodes} from "../api/instance/instance.ts";
import type {PanelNodeDto} from "../models/instance/instance.ts";


export const usePanelNodes = (panelId?: number) =>
    useQuery<PanelNodeDto[]>({
        queryKey: ["panelNodes", panelId],
        queryFn: () => getPanelNodes(panelId as number),
        enabled: typeof panelId === "number" && Number.isFinite(panelId) && panelId > 0,
    });
