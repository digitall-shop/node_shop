import {useMutation} from "@tanstack/react-query";
import {acceptPaymentRequest} from "../api/peyment/payment.ts";
import type {AcceptPaymentPayload, AcceptPaymentResponse} from "../models/peyment/payment.ts";


export function useReceiptUpload(onSuccess?: () => void) {
    const mut = useMutation<AcceptPaymentResponse | null, Error, AcceptPaymentPayload>({
        mutationFn: acceptPaymentRequest,
        onSuccess,
    });

    return {
        uploading: mut.isPending,
        error: mut.error?.message ?? null,
        send: (paymentRequestId: number, file: File) =>
            mut.mutate({paymentRequestId, receipt: file}),
    };
}
