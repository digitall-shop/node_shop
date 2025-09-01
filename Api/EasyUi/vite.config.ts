import {defineConfig} from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
    base: '/EasyUi',
    plugins: [react()],
    server: {
        allowedHosts: [
            '0f5b1ae8916b.ngrok-free.app',
        ],
    },
})

