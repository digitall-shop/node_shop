import { createContext } from "react";
import type { TelegramUser } from "./TelegramTypes";

export interface TelegramContextType { user: TelegramUser | null; loading: boolean }
export const TelegramContext = createContext<TelegramContextType>({ user: null, loading: true });
