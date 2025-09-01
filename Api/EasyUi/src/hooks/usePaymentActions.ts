import {useMutation, useQueryClient} from "@tanstack/react-query";
import {useState} from "react";
import {approvePaymentRequest, rejectPaymentRequest} from "../api/peyment/payment.ts";

export function usePaymentActions() {
    const qc = useQueryClient();
    const [busyId, setBusyId] = useState<number | null>(null);
    const [rejectModal, setRejectModal] = useState<{ open: boolean; id: number | null }>({open: false, id: null});

    const invalidate = () => qc.invalidateQueries({queryKey: ["admin-payment-requests"]});

    const approveMut = useMutation({
        mutationFn: (id: number) => approvePaymentRequest(id),
        onMutate: (id) => setBusyId(id),
        onSettled: () => setBusyId(null),
        onSuccess: invalidate,
    });

    const rejectMut = useMutation({
        mutationFn: ({id, description}: { id: number; description: string }) =>
            rejectPaymentRequest(id, {description} as any),
        onMutate: ({id}) => setBusyId(id),
        onSettled: () => setBusyId(null),
        onSuccess: () => {
            setRejectModal({open: false, id: null});
            invalidate();
        },
    });

    return {
        busyId,
        rejectModal, setRejectModal,
        approve: (id: number) => approveMut.mutate(id),
        approving: approveMut.isPending,
        reject: (id: number, description: string) => rejectMut.mutate({id, description}),
        rejecting: rejectMut.isPending,
    };
}
