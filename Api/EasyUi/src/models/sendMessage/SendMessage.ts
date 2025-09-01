import type {BaseResponse} from '../common/common';

export type ParseMode = 'Markdown' | 'MarkdownV2' | 'HTML';

export interface BroadcastMessageRequest {
    text: string;
    parseMode?: ParseMode | (string & {});
    disableWebPagePreview?: boolean;
    userIds?: number[] | null;
}

export interface BroadcastMessageResult {
    totalTargets: number;
    sent: number;
    failed: number;
    failedUserIds: number[];
}

export interface SendUserMessagePathParams {
    userId: number;
}

export interface SendUserMessageRequest {
    text: string;
    parseMode?: ParseMode | (string & {});
    disableWebPagePreview?: boolean;
}

export type SendUserMessageResponse = BaseResponse<null>;
export type BroadcastMessageResponse = BaseResponse<BroadcastMessageResult>;
