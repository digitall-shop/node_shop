import React from "react";

export interface NodeDto {
    id: number;
    nodeName: string;
    sshHost: string;
    sshPort: number;
    sshUsername: string;
    method: number;
    serverPort: number;
    price: number;
    status: number;
    isAvailableForPurchase: boolean;
    xrayContainerImage: string | null;
    provisioningStatus?: string | number;
    provisioningMessage?: string | null;
    agentVersion?: string | null;
    targetAgentVersion?: string | null;
    lastSeenUtc?: string | null;
    installMethod?: string | number;
    marzbanEndpoint?: string | null;
    isEnabled?: boolean;
}

export interface InitiateNode {
    "nodeId": number;
    "panelId": number;
}
export type NodeStatus = 0 | 1 | 2;

export interface Props {
    node: { nodeName: string; price: number; status: NodeStatus };
    footer?: React.ReactNode;
    isConnected?: boolean;
    onSelect?: () => void;
    selecting?: boolean;
    className?: string;
}