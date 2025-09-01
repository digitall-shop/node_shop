import type {
    BroadcastMessageRequest,
    BroadcastMessageResponse,
    SendUserMessagePathParams,
    SendUserMessageRequest,
    SendUserMessageResponse,
} from "../../models/sendMessage/SendMessage";
import axiosInstance from "../common/axiosInstance.ts";

export async function apiBroadcastMessage(
    payload: BroadcastMessageRequest,
    options?: { signal?: AbortSignal }
): Promise<BroadcastMessageResponse> {
    const {data} = await axiosInstance.post<BroadcastMessageResponse>(
        "/broadcast",
        payload,
        {signal: options?.signal}
    );
    return data;
}

export function apiBroadcastToAll(
    text: string,
    opts?: Omit<BroadcastMessageRequest, "text" | "userIds">,
    options?: { signal?: AbortSignal }
): Promise<BroadcastMessageResponse> {
    const body: BroadcastMessageRequest = {
        text,
        userIds: null,
        ...opts,
    };
    return apiBroadcastMessage(body, options);
}

export function apiBroadcastToUsers(
    text: string,
    userIds: number[],
    opts?: Omit<BroadcastMessageRequest, "text" | "userIds">,
    options?: { signal?: AbortSignal }
): Promise<BroadcastMessageResponse> {
    const body: BroadcastMessageRequest = {
        text,
        userIds,
        ...opts,
    };
    return apiBroadcastMessage(body, options);
}

export async function apiSendMessageToUser(
    userId: SendUserMessagePathParams["userId"],
    payload: SendUserMessageRequest,
    options?: { signal?: AbortSignal }
): Promise<SendUserMessageResponse> {
    const {data} = await axiosInstance.post<SendUserMessageResponse>(
        `/broadcast/user/${userId}`,
        payload,
        {signal: options?.signal}
    );
    return data;
}

export function apiSendTextToUser(
    userId: SendUserMessagePathParams["userId"],
    text: string,
    opts?: Omit<SendUserMessageRequest, "text">,
    options?: { signal?: AbortSignal }
): Promise<SendUserMessageResponse> {
    const body: SendUserMessageRequest = {text, ...opts};
    return apiSendMessageToUser(userId, body, options);
}