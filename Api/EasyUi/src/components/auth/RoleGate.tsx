// src/components/auth/RoleGate.tsx

import { Outlet } from 'react-router-dom';
import { useState, useMemo } from 'react';
import { getRoles } from '../../models/auth.utils';
import CheckingSuperAdminPage from "./CheckingSuperAdminPage.tsx";

/**
 * A layout component that gates access to its children based on
 * whether a user role has been set in the current session.
 */
const RoleGate = () => {
    const initialRoles = useMemo(() => getRoles(), []);
    const [hasRole, setHasRole] = useState(initialRoles.length > 0);

    const handleRoleSet = () => {
        setHasRole(true);
    };
    return hasRole ? <Outlet /> : <CheckingSuperAdminPage onRoleSet={handleRoleSet} />;
};

export default RoleGate;