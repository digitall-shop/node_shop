import React from "react";
import BrandComponent from "../BrandComponent.tsx";
import NavbarComponent from "./NavbarComponent.tsx";

const HeaderComponent: React.FC = () => {
    return (
        <header className="bg-white dark:bg-gray-900/80 backdrop-blur-md shadow-md sticky top-0 z-50">
            <div className="container mx-auto px-4 py-3 flex justify-between items-center">
                <BrandComponent/>
                <NavbarComponent/>
            </div>
        </header>
    );
};

export default HeaderComponent;