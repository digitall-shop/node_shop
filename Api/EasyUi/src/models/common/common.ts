export interface BaseResponse<T> {
    data: T | null;
    isSuccess: boolean;
    statusCode: number;
    message: string | null;
    jsonValidationMessage: string | null;
}