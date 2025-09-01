import { Outlet } from 'react-router-dom';
import { useTelegramUser } from "../../hooks/useTelegramUser.ts";

const FullScreenLoader = () => (
    <div className="flex items-center justify-center h-screen bg-gray-900">
        <div className="animate-spin rounded-full h-12 w-12 border-4 border-white border-t-transparent" />
    </div>
);

const ProtectedRoute = () => {
    const { user, loading } = useTelegramUser();

    if (loading) {
        return <FullScreenLoader />;
    }

    if (!user) {
        return (
            <div className="flex items-center justify-center h-screen text-red-500 text-xl bg-gray-900 font-sans">
                دسترسی غیرمجاز - کاربر از طریق تلگرام وارد نشده!
            </div>
        );
    }

    return <Outlet />;
};

export default ProtectedRoute;
