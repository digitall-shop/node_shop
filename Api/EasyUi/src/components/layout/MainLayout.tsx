import {Outlet} from 'react-router-dom';
import HeaderComponent from './header/HeaderComponent.tsx';
import MainMenuFooter from './footer/MainMenuFooter.tsx';

const MainLayout = () => (
    <>
        <HeaderComponent/>
        <main className="pt-8 pb-24 container mx-auto px-4">
            <Outlet/>
        </main>
        <MainMenuFooter/>
    </>
);

export default MainLayout;
