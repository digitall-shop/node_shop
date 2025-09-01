import {ContainerProvisionStatus} from "./instance/status.ts";

export const statusConfig = {
    [ContainerProvisionStatus.Running]:        { labelKey: "containerStatus.running",        icon: "fa-check-circle",         color: "text-green-400",  bg: "bg-green-500/10" },
    [ContainerProvisionStatus.Provisioning]:   { labelKey: "containerStatus.provisioning",   icon: "fa-spinner fa-spin",      color: "text-amber-400",  bg: "bg-amber-500/10" },
    [ContainerProvisionStatus.Stopped]:        { labelKey: "containerStatus.stopped",        icon: "fa-stop-circle",          color: "text-yellow-300", bg: "bg-yellow-500/10" },
    [ContainerProvisionStatus.Failed]:         { labelKey: "containerStatus.failed",         icon: "fa-exclamation-triangle", color: "text-red-400",    bg: "bg-red-500/10" },
    [ContainerProvisionStatus.Pending]:        { labelKey: "containerStatus.pending",        icon: "fa-clock",                color: "text-gray-400",   bg: "bg-gray-500/10" },
    [ContainerProvisionStatus.Deleting]:       { labelKey: "containerStatus.deleting",       icon: "fa-trash",                color: "text-orange-300", bg: "bg-orange-500/10" },
    [ContainerProvisionStatus.Deleted]:        { labelKey: "containerStatus.deleted",        icon: "fa-check",                color: "text-gray-300",   bg: "bg-gray-500/10" },
    [ContainerProvisionStatus.PausedBySystem]: { labelKey: "containerStatus.pausedBySystem", icon: "fa-pause-circle",         color: "text-sky-300",    bg: "bg-sky-500/10" },
    [ContainerProvisionStatus.PausedByUser]:   { labelKey: "containerStatus.pausedByUser",   icon: "fa-pause-circle",         color: "text-yellow-300", bg: "bg-yellow-500/10" },
} as const;