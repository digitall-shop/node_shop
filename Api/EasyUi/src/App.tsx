// src/App.tsx (Updated)

import {Routes, Route, BrowserRouter} from 'react-router-dom';
import {useAppSetup} from './hooks/useAppSetup';
import MainLayout from "./components/layout/MainLayout";
import PanelDetailLayout from "./components/layout/PanelDetailLayout";
import ProtectedRoute from './components/auth/ProtectedRoute';
import RoleGate from './components/auth/RoleGate'; // <-- 1. IMPORT
import NodeBackground from "./components/NodeBackground/NodeBackground.tsx";
import Home from "./components/pages/Home.tsx";
import ProfileMenu from "./components/profile/ProfileMenu.tsx";
import PanelPage from "./components/pages/PanelPage.tsx";
import Payment from "./components/pages/Payment.tsx";
import ShopPage from "./components/pages/panelSetting/ShopPage.tsx";
import PanelDetailPage from "./components/pages/panelSetting/PanelDetailPage.tsx";
import HostPage from "./components/pages/panelSetting/HostPage.tsx";
import Transactions from "./components/pages/Transactions.tsx";
import WaitingForPayment from "./components/pages/WaitingForPayment.tsx";
import AdminPayments from "./components/pages/AdminPayments.tsx";
import MyPayments from "./components/pages/MyPayments.tsx";
import AllBanks from "./components/pages/AllBanks.tsx";
import Users from "./components/pages/Users.tsx";
import SendMessage from './components/pages/SendMessage.tsx';
import AdminNodeHealth from "./components/pages/AdminNodeHealth.tsx";

const InitializingLoader = () => (
    <div className="flex items-center justify-center h-screen bg-gray-900">
        <div className="animate-spin rounded-full h-12 w-12 border-4 border-white border-t-transparent" />
    </div>
);

function App() {
    const { isInitialized } = useAppSetup();
    if (!isInitialized) {
        return <InitializingLoader />;
    }


    return (
        <BrowserRouter basename="/EasyUi">
            <div className="relative min-h-screen text-white overflow-x-hidden">
                <NodeBackground/>
                <Routes>
                    <Route element={<ProtectedRoute/>}>
                        {/* 2. Wrap your main layouts with RoleGate */}
                        <Route element={<RoleGate/>}>
                            <Route element={<MainLayout/>}>
                                <Route path="/" element={<Home/>}/>
                                <Route path="/profile" element={<ProfileMenu/>}/>
                                <Route path="/panel" element={<PanelPage/>}/>
                                <Route path="/payment" element={<Payment/>}/>
                                <Route path="/transaction" element={<Transactions/>}/>

                                <Route path="/paymentConfirmation" element={<WaitingForPayment/>}/>
                                <Route path="/allPayment" element={<AdminPayments/>}/>
                                <Route path="/banks" element={<AllBanks/>}/>
                                <Route path="/users" element={<Users/>}/>
                                <Route path="/sendMessage" element={<SendMessage/>}/>
                                <Route path="/admin/nodes" element={<AdminNodeHealth/>}/>

                                <Route path="/myPayments" element={<MyPayments/>}/>
                            </Route>

                            <Route path="/panel/:panelId" element={<PanelDetailLayout/>}>
                                <Route index element={<PanelDetailPage/>}/>
                                <Route path="host" element={<HostPage/>}/>
                                <Route path="shop" element={<ShopPage/>}/>
                            </Route>
                        </Route>
                    </Route>
                </Routes>
            </div>
        </BrowserRouter>
    );
}

export default App;