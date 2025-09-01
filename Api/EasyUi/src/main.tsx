import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import './index.css';
import '@fortawesome/fontawesome-free/css/all.min.css';
import './i18n';

import { QueryClientProvider } from '@tanstack/react-query';
import { queryClient } from './queryClient';
import {TelegramProvider} from "./context/TelegramProvider.tsx";

ReactDOM.createRoot(document.getElementById('root')!).render(
    <React.StrictMode>
        <TelegramProvider>
            <QueryClientProvider client={queryClient}>
                <App />
            </QueryClientProvider>
        </TelegramProvider>
    </React.StrictMode>
);