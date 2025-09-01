import { useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import type {GetUsersParams, PaginatedUsers} from "../models/user/user.ts";
import {getUsers} from "../api/user/user.ts";


export const ITEMS_PER_PAGE = 12;

export const useUsersQuery = (filters: Partial<GetUsersParams>, page: number) => {
    const queryFilters: GetUsersParams = useMemo(
        () => ({
            ...filters,
            Skip: (page - 1) * ITEMS_PER_PAGE,
            Take: ITEMS_PER_PAGE,
            OrderBy: "userId",
            OrderDir: "desc",
        }),
        [page, filters]
    );

    const q = useQuery<PaginatedUsers>({
        queryKey: ["users", queryFilters],
        queryFn: () => getUsers(queryFilters),
        staleTime: 30000,
        refetchOnWindowFocus: false,
    });

    return { ...q, queryFilters };
};
