export interface PanelCreateDto {
    userId?: number;
    name: string;
    url: string;
    userName: string;
    password: string;
    ssl?: boolean;
}

export interface PanelUpdateDto {
    name?: string;
    url?: string;
    userName?: string;
    password?: string;
    ssl?: boolean;
}

export interface PanelDto {
    id: number;
    name: string;
    url: string;
    userName: string;
    userId: number;
    password: string;
    sshCertificateKey: string;
    xrayPort: number;
    apiPort: number;
    serverPort: number;
    token: string;
    panelIpAddress: string;
}
