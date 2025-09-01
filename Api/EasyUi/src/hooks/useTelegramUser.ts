import { useContext } from 'react';
import {TelegramContext, type TelegramContextType} from '../context/TelegramContext';

export const useTelegramUser = (): TelegramContextType => {
    return useContext(TelegramContext);
};
