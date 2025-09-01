export const ContainerProvisionStatus = {
    Pending: 0,
    Provisioning: 1,
    Running: 2,
    Stopped: 3,
    Failed: 4,
    Deleting: 5,
    Deleted: 6,
    PausedBySystem: 7,
    PausedByUser: 8,
} as const;

export type ContainerProvisionStatusCode =
    (typeof ContainerProvisionStatus)[keyof typeof ContainerProvisionStatus];

export const canTogglePause = (s: number): boolean =>
    s === ContainerProvisionStatus.Running ||
    s === ContainerProvisionStatus.PausedByUser;

export const isPausedBySystem = (s: number): boolean =>
    s === ContainerProvisionStatus.PausedBySystem;
