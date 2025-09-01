import {useQuery} from "@tanstack/react-query";
import type {UserBalance} from "../models/user/user";
import {getUserBalance} from "../api/user/user.ts";

export const useUserBalance = (userId?: string | number) =>
    useQuery<number, Error>({
        queryKey: ["user-balance", userId],

        queryFn: async () => {
            if (userId == null) throw new Error("No user id");
            const res: UserBalance = await getUserBalance(userId);
            if (res.isSuccess === false) {

                throw new Error(res.message || "Failed to fetch balance");
            }
            return res.data;
        },
        enabled: userId != null,
        staleTime: 60_000,
        refetchOnWindowFocus: false,
    });
